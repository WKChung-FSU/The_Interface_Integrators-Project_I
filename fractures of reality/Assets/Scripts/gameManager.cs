using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Audio;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    //public Slider slider;     wtf is slider????
    //public GameObject LoadingScreen;
    public enum GameGoal { KillAllEnemies, ReachGoal, AcquireObjects }
    [SerializeField] GameGoal CurrentGoal;
    [SerializeField] bool PauseLock;

   
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
    [SerializeField] GameObject menuWeap;
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
    [SerializeField] GameObject reticle;
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
    [System.Serializable]
    struct WinMenuScoresObjects
    {
        [SerializeField] public GameObject SkeletonsKilledLabel;
        [SerializeField] public TMP_Text SkeletonsKilledAmount;
        [SerializeField] public TMP_Text SkeletonsKilledValue;
        [SerializeField] public GameObject BeholderKilledLabel;
        [SerializeField] public TMP_Text BeholderKilledAmount;
        [SerializeField] public TMP_Text BeholderKilledValue;
        [SerializeField] public GameObject NecromancersKilledLabel;
        [SerializeField] public TMP_Text NecromancersKilledAmount;
        [SerializeField] public TMP_Text NecromancersKilledValue;
        [SerializeField] public GameObject DragonKilledLabel;
        [SerializeField] public TMP_Text DragonKilledAmount;
        [SerializeField] public TMP_Text DragonKilledValue;
        [SerializeField] public GameObject CrystalsDestroyedLabel;
        [SerializeField] public TMP_Text CrystalsDestroyedAmount;
        [SerializeField] public TMP_Text CrystalsDestroyedValue;
        [SerializeField] public GameObject playerDeathsLabel;
        [SerializeField] public TMP_Text playerDeathsAmount;
        [SerializeField] public TMP_Text playerDeathsValue;
        [SerializeField] public TMP_Text totalScoreValue;
        [SerializeField] public Button winMenuMainMenuButton;
        [SerializeField] public Button winMenuRestartButton;
        [SerializeField] public Button winMenuQuitButton;

    }
    [SerializeField] WinMenuScoresObjects wMenuObj;
    [SerializeField] public ScoreKeeper scoreKeeper;


    private int score;
    private int timesDied;

    #endregion
    bool StopSpawning;

    // Start is called before the first frame update
    void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        StopSpawning = false;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<DestructibleHealthCore>();
        CharacterController = player.GetComponent<CharacterController>();
        playerWeapon = player.GetComponent<PlayerWeapon>();
        playerController = player.GetComponent<playerController>();
        hudEnabled = true;
        startPosition = player.transform.position;
        GoalTextUpdate();
        enemiesInMeleeRangeOfPlayer.Capacity = enemiesAllowedToCrowdThePlayer;

        //PlayerController.PlayerKeys.Clear();
        //Debug.LogWarning("GM start key clear");
        //crystalManifest.ResetManifest();

    }
    private void Start()
    {
        if (PauseLock)
        {
            StartCoroutine(MainMenu());
        }


    }
    private void OnApplicationQuit()
    {
        //TODO: remember to add a save to this...
        crystalManifest.ResetManifest();
        scoreKeeper.ResetScoreKeeper();
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
            if (menuActive == null)
            {
                stateWeapOn();
                menuActive = menuWeap;
                menuActive.SetActive(true);
               
               
            }
            else if (menuActive == menuWeap)
            {
                stateWeapOff();
               
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
    public void stateWeapOn()
    {
        Time.timeScale = 0.1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        playerWeapon.enabled = false;
        Debug.Log("WeaponMenu active");
    }
    public void stateWeapOff()
    {
        
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        playerWeapon.enabled = true;
        Debug.Log("WeaponMenu inactive");

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
    public void switchWeapon(int selection)
    {
        if (selection == 0)
        {
            playerWeapon.InstantWeaponSwitch(DamageEngine.ElementType.Normal);
            UpdateWeaponIconUI();
            playerWeapon.UpdateSpellList();
            stateWeapOff();
            
        }
        else if (selection == 1)
        {
            playerWeapon.InstantWeaponSwitch(DamageEngine.ElementType.fire);
            UpdateWeaponIconUI();
            playerWeapon.UpdateSpellList();
            stateWeapOff();
            
        }
        else if (selection == 2)
        {
            playerWeapon.InstantWeaponSwitch(DamageEngine.ElementType.Lightning);
            UpdateWeaponIconUI();
            playerWeapon.UpdateSpellList();
            stateWeapOff();

        }
        else if (selection == 3)
        {
            playerWeapon.InstantWeaponSwitch(DamageEngine.ElementType.Ice);
            UpdateWeaponIconUI();
            playerWeapon.UpdateSpellList();
            stateWeapOff();
        }
        else if (selection == 4)
        {
            playerWeapon.InstantWeaponSwitch(DamageEngine.ElementType.Earth);
            UpdateWeaponIconUI();
            playerWeapon.UpdateSpellList();
            stateWeapOff();
        }       
        else if (selection == 5)
        {
            playerWeapon.InstantWeaponSwitch(DamageEngine.ElementType.Water);
            UpdateWeaponIconUI();
            playerWeapon.UpdateSpellList();
            stateWeapOff();
        }
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
        scoreKeeper.ResetScoreKeeper();
    }

    private void CalculateScore()
    {
        //first disable the buttons at the bottom
        //also maybe remove controls from the player?
        reticle.SetActive(false);
        StartCoroutine(ShowAllStats());
    }
    IEnumerator ShowAllStats()
    {
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        //skeletons
        StartCoroutine(ShowStat(wMenuObj.SkeletonsKilledLabel, wMenuObj.SkeletonsKilledAmount, wMenuObj.SkeletonsKilledValue, scoreKeeper.getSkeles(), scoreKeeper.skeleValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        //Beholders
        StartCoroutine(ShowStat(wMenuObj.BeholderKilledLabel, wMenuObj.BeholderKilledAmount, wMenuObj.BeholderKilledValue, scoreKeeper.getBeholders(), scoreKeeper.BeholdersValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        //Necromancers
        StartCoroutine(ShowStat(wMenuObj.NecromancersKilledLabel, wMenuObj.NecromancersKilledAmount, wMenuObj.NecromancersKilledValue, scoreKeeper.getNecromancers(), scoreKeeper.NecromancersValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        //Dragon
        StartCoroutine(ShowStat(wMenuObj.DragonKilledLabel, wMenuObj.DragonKilledAmount, wMenuObj.DragonKilledValue, scoreKeeper.getDragon(), scoreKeeper.DragonsKilledValue));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);

        //crystals multiplier
        StartCoroutine(ShowStat(wMenuObj.CrystalsDestroyedLabel, wMenuObj.CrystalsDestroyedAmount, wMenuObj.CrystalsDestroyedValue,crystalManifest.GetAmountOfCrystals(),1));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        //player
        StartCoroutine(ShowStat(wMenuObj.playerDeathsLabel, wMenuObj.playerDeathsAmount, wMenuObj.playerDeathsValue, timesDied, scoreKeeper.playerDeathScoreDeduction));
        yield return new WaitForSecondsRealtime(nextScoreDelay * 3);
        wMenuObj.winMenuMainMenuButton.interactable = true;
        wMenuObj.winMenuRestartButton.interactable = true;
        wMenuObj.winMenuQuitButton.interactable = true;
        //crystal info

        //players die

        //flash the total
        //enable those last few buttons

    }
    IEnumerator ShowStat(GameObject label, TMP_Text amount, TMP_Text value, int amountKilled, int valueOfEnemy)
    {
        //the part where the point amount for this enemy is added up
        //I would like this to start at 0 and then build up to (value) in a short duration over time
        //yield return new WaitForSeconds(nextScoreDelay);
        label.SetActive(true); 
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        amount.text = amountKilled.ToString();
        amount.enabled = true;
        yield return new WaitForSecondsRealtime(nextScoreDelay);
        if (label == wMenuObj.playerDeathsLabel)
        {
            value.text = (amountKilled * valueOfEnemy).ToString();
            score += (amountKilled * valueOfEnemy);
        }
        else if (label == wMenuObj.CrystalsDestroyedLabel)
        {
            float crystalValue = 0.1f;
            value.text = "x " + ((amountKilled * crystalValue) + 1).ToString();
            float fScore = score * ((crystalValue * amountKilled) + 1);
            score = (int)fScore;
        }
        else
        {
            value.text = "+ " + (amountKilled * valueOfEnemy).ToString();
            score += (amountKilled * valueOfEnemy);
        }
        value.enabled = true;
        
        wMenuObj.totalScoreValue.text = score.ToString("f0");
        yield return new WaitForSecondsRealtime(nextScoreDelay);

    }
    #endregion

    #region Tooltip methods

    public void ToolTip(TMP_Text textObj)
    {
        toolTipText.text = textObj.text;
        toolTipPanel.SetActive(true);
        reticle.SetActive(false);
    }
    public void ClearToolTip()
    {
        toolTipText.text = string.Empty;
        toolTipPanel.SetActive(false);
        reticle.SetActive(true);
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
        reticle.SetActive(true);
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
