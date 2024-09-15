using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.AI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public Slider slider;
    public GameObject LoadingScreen;
    public enum GameGoal { KillAllEnemies, ReachGoal, AcquireObjects }
    [SerializeField] GameGoal CurrentGoal;


    #region RadialMenu

    [Header("----- Radial Menu -----")]
    [SerializeField] GameObject EntryPrefab;

    [SerializeField] float Radius = 300f;
    [SerializeField] List<Texture> icons;
    [SerializeField] public GameObject WeapPause;
    List<RadialMenuEntry> entries;
    #endregion

    #region Player
    [Header("----- Player Attributes -----")]
    public GameObject player;
    public DestructibleHealthCore playerScript;
    public PlayerWeapon playerWeapon;
    CharacterController CharacterController;
    playerController playerController;
    int GoalCount;
    Vector3 startPosition;
    CheckpointSystem lastCheckPoint;

    private int score;
    private int timesDied;

    [SerializeField] int enemiesAllowedToCrowdThePlayer = 3;
    private List<GameObject> enemiesInMeleeRangeOfPlayer = new List<GameObject>();


    #endregion

    #region CrystalStates
    [SerializeField] public PowerCrystalManifest crystalManifest;
    #endregion

    #region UI
    [Header("----- Ui settings/Objects -----")]
    [SerializeField] GameObject menuPause;
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuHUD;
    [SerializeField] Image healthBar;
    [SerializeField] Image ammoBar;
    [SerializeField] GameObject damageFlashScreen;
    [SerializeField] TMP_Text healthCountText;
    [SerializeField] TMP_Text ammoCountText;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text enemyCountValue;
    [SerializeField] string[] goalText = new string[3];
    [SerializeField] Image fractureBar;
    public bool isPaused;
    public bool hudEnabled;



    [Header("----- sounds -----")]
    [SerializeField] AudioSource AudioSource;

    bool playerDead;

    //weapon icons
    [SerializeField] TypeIcon wCurrentSpellIcon;

    #endregion
    bool StopSpawning;

    // Start is called before the first frame update
    void Awake()
    {

        StopSpawning = false;
        instance = this;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<DestructibleHealthCore>();
        CharacterController = player.GetComponent<CharacterController>();
        playerWeapon = player.GetComponent<PlayerWeapon>();
        playerController = player.GetComponent<playerController>();
        hudEnabled = true;
        startPosition = player.transform.position;
        GoalTextUpdate();
        enemiesInMeleeRangeOfPlayer.Capacity = enemiesAllowedToCrowdThePlayer;

        //crystalManifest.ResetManifest();

    }
    private void Start()
    {
        entries = new List<RadialMenuEntry>();
    }
    private void OnApplicationQuit()
    {
        gameManager.instance.crystalManifest.ResetManifest();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
                DisableHUD();
            }
            else if (menuActive == menuPause)
            {
                stateUnPaused();
                EnableHUD();
            }

        }
        if (Input.GetButtonDown("WeaponMenu"))
        {
            if (entries.Count == 0)
            {
                Open();
                statePause();
                menuActive = WeapPause;
                menuActive.SetActive(isPaused);

                DisableHUD();
            }
            else
            {
                Close();
                stateUnPaused();
                EnableHUD();
            }
        }
        GoalTextUpdate();
        //update health bar
        UpdateHUD();
        UpdateFractureBar();
        //check if switch weapon button is pressed
        //then call weapon swap UI code
        //if (Input.GetButtonDown("Switch Weapon"))
        //{
        //    UpdateWeaponIconUI();
        //}

    }
    void GoalTextUpdate()
    {
        switch (CurrentGoal)
        {
            case GameGoal.KillAllEnemies:
                enemyCountValue.enabled = true;
                enemyCountText.text = goalText[0];
                break;
            case GameGoal.ReachGoal:
                enemyCountText.text = goalText[1];
                enemyCountValue.enabled = false;
                break;
            case GameGoal.AcquireObjects:
                enemyCountValue.enabled = true;
                enemyCountText.text = goalText[2];
                break;
            default:
                break;
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("paused");
    }
    public void stateUnPaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
        EnableHUD();
        Debug.Log("un Paused");
    }

    public void updateGameGoal(int amount = 0)
    {
        GoalCount += amount;

        enemyCountValue.text = GoalCount.ToString("f0");

        if (GoalCount <= 0)
        {

            switch (CurrentGoal)
            {
                case GameGoal.KillAllEnemies:
                    youWin();
                    break;
                case GameGoal.AcquireObjects:
                    gameGoal(GameGoal.ReachGoal);
                    break;
            }
        }
    }
    public void youLose()
    {
        playerDead = true;
        statePause();
        Debug.Log("YOU LOSE MENU OPEN?");
        menuActive = menuLose;
        Debug.Log("menu should appear");
        menuActive.SetActive(isPaused);
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(isPaused);
    }

    public void EnableHUD()
    {
        hudEnabled = true;
        ToggleHUD();
    }

    public void DisableHUD()
    {
        hudEnabled = false;
        ToggleHUD();
    }

    public void DamageFlashScreen(Color color)
    {
        StartCoroutine(DamageFlashTimer(color));
    }

    public void UpdateWeaponIconUI()
    {
        AttackCore weapon = playerWeapon.GetCurrentWeapon().GetComponent<AttackCore>();
        //disable all weapon icons first
        wCurrentSpellIcon.EnableElementTypeGraphic(weapon.ElementType);
    }


    public void Respawn(bool trueRespawn = false)
    {
        playerScript.TeleportTo(startPosition);
        //clear the crowd list
        enemiesInMeleeRangeOfPlayer.Clear();
        timesDied++;
        if (trueRespawn == true)
        {
            playerWeapon.ReloadAmmo();
            playerScript.StopAllCoroutines();
            playerScript.ResetAllStatuses();
            playerDead = false;
        }
    }

    #region Score methods
    public void AddScore(int add)
    {
        score += add;
    }

    private void ResetScore()
    {
        score = 0;
        timesDied = 0;
    }

    private void CalculateScore()
    {
        score = score + (timesDied * -100);
    }
    #endregion

    #region RadMenu
    void AddEntry(Texture pIcon)
    {
        GameObject entry = Instantiate(EntryPrefab, transform);
        RadialMenuEntry rme = entry.GetComponent<RadialMenuEntry>();


        rme.SetIcon(pIcon);
        entries.Add(rme);
    }

    public void Open()
    {
        for (int i = 0; i < icons.Count; i++)
        {
            AddEntry(icons[i]);
        }
        rearrange();
    }
    public void Close()
    {
        for (int i = 0; i < 6; i++)
        {

            RectTransform rect = entries[i].GetComponent<RectTransform>();
            GameObject entry = entries[i].gameObject;

            Destroy(entry);
        }
        entries.Clear();
    }
    void rearrange()
    {
        float radiansOfSeperation = (Mathf.PI * 2) / entries.Count;
        for (int i = 0; i < entries.Count; i++)
        {
            float x = Mathf.Sin(radiansOfSeperation * i) * Radius;
            float y = Mathf.Cos(radiansOfSeperation * i) * Radius;

            entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        }
    }


    #endregion
    #region Getters and Setter
    public Vector3 StartPosition()
    {
        return startPosition;
    }

    public void StartPosition(Vector3 newPosition)
    {
        startPosition = newPosition;
    }

    public CheckpointSystem Checkpoint()
    {
        return lastCheckPoint;
    }

    public void Checkpoint(CheckpointSystem checkpoint)
    {
        lastCheckPoint = checkpoint;
    }

    public bool PlayerDead
    {
        get
        {
            return playerDead;
        }
        set
        {
            playerDead = value;
        }
    }

    public void gameGoal(GameGoal NewGoal)
    {
        CurrentGoal = NewGoal;
        updateGameGoal();
    }

    public GameGoal gameGoal()
    {
        return CurrentGoal;
    }

    public void AddToPlayerMeleeRangeList(GameObject other)
    {
        enemiesInMeleeRangeOfPlayer.Add(other);
    }

    public void RemoveFromPlayerMeleeRangeList(GameObject other)
    {
        enemiesInMeleeRangeOfPlayer.Remove(other);
    }

    public bool CrowdListContains(GameObject other)
    {
        if (enemiesInMeleeRangeOfPlayer.Contains(other))
            return true;
        else
            return false;
    }

    public int GetCrowdCapacity()
    {
        return enemiesInMeleeRangeOfPlayer.Count;
    }

    public int GetCrowdAllowance()
    {
        return enemiesAllowedToCrowdThePlayer;
    }

    public bool stopSpawning
    {
        get { return StopSpawning; }
        set { StopSpawning = value; }
    }

    public CharacterController PlayerCController
    {
        get { return CharacterController; }
    }
    public playerController PlayerController
    {
        get { return playerController; }
    }
    #endregion


    public void playAudio(AudioClip audio, float volume = 0.5f)
    {
        AudioSource.PlayOneShot(audio, volume);
    }


    #region private functions
    void ToggleHUD()
    {
        //stay DRY
        menuHUD.SetActive(hudEnabled);
    }

    void UpdateHUD()
    {
        //health bar
        healthCountText.text = playerScript.HP.ToString("F0");
        gameManager.instance.healthBar.fillAmount = (float)playerScript.HP / playerScript.HPMax;

        //ammo bar
        ammoCountText.text = playerWeapon.GetCurrentAmmo().ToString("F0");
        gameManager.instance.ammoBar.fillAmount = (float)playerWeapon.GetCurrentAmmo() / playerWeapon.GetMaxAmmo();
    }

    IEnumerator DamageFlashTimer(Color color)
    {
        gameManager.instance.damageFlashScreen.GetComponent<Image>().color = color;
        gameManager.instance.damageFlashScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageFlashScreen.SetActive(false);
    }
    void UpdateFractureBar()
    {
        playerController PlControl = player.GetComponent<playerController>();
        fractureBar.fillAmount = ((float)PlControl.fractureBars[playerWeapon.GetCurrentElement()] / (float)PlControl.getMaxFractureAmount());
    }


    #endregion
}
