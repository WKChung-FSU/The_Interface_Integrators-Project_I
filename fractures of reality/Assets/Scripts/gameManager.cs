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
    [SerializeField] bool PauseLock;

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


    [SerializeField] int enemiesAllowedToCrowdThePlayer = 3;
    private List<GameObject> enemiesInMeleeRangeOfPlayer = new List<GameObject>();


    #endregion

    #region CrystalStates
    [SerializeField] PowerCrystalManifest crystalManifest;
    [SerializeField] TMP_Text CrystalText;
    string CrystalNames;
    #endregion

    #region UI
    [Header("----- Ui settings/Objects -----")]
    [SerializeField] GameObject menuPause;
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject toolTipPanel;
    [SerializeField] TMP_Text toolTipText;
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
    [SerializeField] TMP_Text fractureText;
    public bool isPaused;
    public bool hudEnabled;



    [Header("----- sounds -----")]
    [SerializeField] AudioSource AudioSource;

    bool playerDead;

    //weapon icons
    [SerializeField] TypeIcon wCurrentSpellIcon;

    #endregion

    #region ScoreInfo
    [Header("Score stuff")]
    [SerializeField] GameObject menuWin;
    [SerializeField] float nextScoreDelay = 0.5f;
    //TODO: sound effect here
    [SerializeField] GameObject SkeletonsKilledLabel;
    [SerializeField] TMP_Text SkeletonsKilledAmount;
    [SerializeField] TMP_Text SkeletonsKilledValue;
    [SerializeField] GameObject BeholderKilledLabel;
    [SerializeField] TMP_Text BeholderKilledAmount;
    [SerializeField] TMP_Text BeholderKilledValue;
    [SerializeField] GameObject NecromancersKilledLabel;
    [SerializeField] TMP_Text NecromancersKilledAmount;
    [SerializeField] TMP_Text NecromancersKilledValue;
    [SerializeField] GameObject DragonKilledLabel;
    [SerializeField] TMP_Text DragonKilledAmount;
    [SerializeField] TMP_Text DragonKilledValue;
    [SerializeField] GameObject powerMultLabel;
    [SerializeField] GameObject CrystalsDestroyedLabel;
    [SerializeField] TMP_Text CrystalsDestroyedAmount;
    [SerializeField] TMP_Text CrystalsDestroyedValue;
    [SerializeField] GameObject playerDeathsLabel;
    [SerializeField] TMP_Text playerDeathsAmount;
    [SerializeField] TMP_Text playerDeathsValue;
    [SerializeField] TMP_Text totalScoreValue;
    private int score;
    private int timesDied;

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
        if (PauseLock)
        {
            StartCoroutine(MainMenu());
        }
        entries = new List<RadialMenuEntry>();
    }
    private void OnApplicationQuit()
    {
        // remember to add a save to this...
        gameManager.instance.crystalManifest.ResetManifest();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !PauseLock)
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
        UpdateCrystalText();
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

    public void UpdateCrystalText()
    {
        CrystalText.text = "";
        CrystalNames = "";
        crystalManifest.DestroyList.ForEach(CrystalNameMaker);
        CrystalText.text=CrystalNames;
    }
    
    void CrystalNameMaker(DamageEngine.ElementType crystalElement)
    {
        switch (crystalElement)
        {
            case DamageEngine.ElementType.fire:
                CrystalNames += ("Fire Crystal" + "\n");
                break;
            case DamageEngine.ElementType.Lightning:
                CrystalNames += ("Lightning Crystal" + "\n");
                break;

            case DamageEngine.ElementType.Ice:
                CrystalNames += ("Ice Crystal" + "\n");
                break;

            case DamageEngine.ElementType.Earth:
                CrystalNames += ("Earth Crystal" + "\n");
                break;

            case DamageEngine.ElementType.Water:
                CrystalNames += ("Water Crystal" + "\n");
                break;

            case DamageEngine.ElementType.Normal:
            case DamageEngine.ElementType.Wind_tempHeal:
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
        CalculateScore();
        totalScoreValue.text = score.ToString("f0");
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


    IEnumerator MainMenu()
    {
        yield return new WaitForSeconds(0.01f);
        statePause();
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
        //first disable the buttons at the bottom
        //also maybe remove controls from the player?
        //Time.timeScale = 1f;
        StartCoroutine(ShowAllStats());
        score = score * (1 + crystalManifest.GetAmountOfCrystals());
        score = score + (timesDied * -100);
    }
    IEnumerator ShowAllStats()
    {
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        StartCoroutine(ShowStat(SkeletonsKilledLabel, SkeletonsKilledAmount, SkeletonsKilledValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        StartCoroutine(ShowStat(BeholderKilledLabel, BeholderKilledAmount, BeholderKilledValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        StartCoroutine(ShowStat(NecromancersKilledLabel, NecromancersKilledAmount, NecromancersKilledValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        StartCoroutine(ShowStat(DragonKilledLabel, DragonKilledAmount, DragonKilledValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        powerMultLabel.SetActive(true);
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        StartCoroutine(ShowStat(CrystalsDestroyedLabel, CrystalsDestroyedAmount, CrystalsDestroyedValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        StartCoroutine(ShowStat(playerDeathsLabel, playerDeathsAmount, playerDeathsValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);

        //crystal info

        //players die

        //flash the total
        //enable those last few buttons

    }
    IEnumerator ShowStat(GameObject label, TMP_Text amount, TMP_Text value)
    {
        //the part where the point amount for this enemy is added up
        //I would like this to start at 0 and then build up to (value) in a short duration over time
        //yield return new WaitForSeconds(nextScoreDelay);
        label.SetActive(true); 
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        amount.enabled = true;
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        value.enabled = true;
        yield return new WaitForSecondsRealtime(nextScoreDelay);
    }
    #endregion

    #region Tooltip methods

    public void ToolTip(TMP_Text textObj)
    {
        toolTipText.text = textObj.text;
        toolTipPanel.SetActive(true);
    }
    public void ClearToolTip()
    {
        toolTipText.text = string.Empty;
        toolTipPanel.SetActive(false);
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

    public PowerCrystalManifest PCrystalManifest { 
        get { return crystalManifest; } 
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
        Color BarColor;
        switch (playerWeapon.GetCurrentElement())
        {
            default:
            case DamageEngine.ElementType.Normal:
                BarColor = Color.magenta;
                break;
            case DamageEngine.ElementType.fire:
                BarColor = Color.red;
                break;
            case DamageEngine.ElementType.Lightning:
                BarColor = Color.yellow;
                break;
            case DamageEngine.ElementType.Ice:
                BarColor = Color.white;
                break;
            case DamageEngine.ElementType.Earth:
                BarColor = Color.green;
                break;
            case DamageEngine.ElementType.Water:
                BarColor = Color.blue;
                break;
        }




        if (playerController.FractureStatus && !fractureText.gameObject.activeSelf)
        {
            fractureText.gameObject.SetActive(true);
            fractureText.color = BarColor;
            BarColor = Color.gray;
        }
        else if (fractureText.gameObject.activeSelf && !playerController.FractureStatus)
        {
            fractureText.gameObject.SetActive(false);
        }

        fractureBar.color = BarColor;
        fractureBar.fillAmount = ((float)playerController.fractureBars[playerWeapon.GetCurrentElement()] / (float)playerController.getMaxFractureAmount());
    }


    #endregion
}
