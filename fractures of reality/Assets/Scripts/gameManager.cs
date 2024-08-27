using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public enum GameGoal { KillAllEnemies, ReachGoal, AcquireObjects}
    [SerializeField] GameGoal CurrentGoal;
    #region Player
    [Header("----- Player Attributes -----")]
    public GameObject player;
    public DestructibleHealthCore playerScript;
    public PlayerWeapon playerWeapon;
    CharacterController playerController;
    int GoalCount;
    Vector3 startPosition;
    CheckpointSystem lastCheckPoint;
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
    [SerializeField] string[] goalText=new string[3];
    public bool isPaused;
    public bool hudEnabled;

    [Header("----- sounds -----")]
    [SerializeField] AudioSource AudioSource;
    
    bool playerDead;

    //weapon icons
    [SerializeField] TypeIcon wCurrentSpellIcon;

    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<DestructibleHealthCore>();
        playerController= player.GetComponent<CharacterController>();
        playerWeapon = player.GetComponent<PlayerWeapon>();

        hudEnabled = true;
        startPosition = player.transform.position;
        GoalTextUpdate();
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
        GoalTextUpdate();
        //update health bar
        UpdateHUD();

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

    public void updateGameGoal(int amount=0)
    {
        GoalCount += amount;

        enemyCountValue.text = GoalCount.ToString("f0");

        if (GoalCount <= 0&&CurrentGoal==GameGoal.KillAllEnemies)
        {
            youWin();
        }
    }
    public void youLose()
    {
        statePause();
        Debug.Log("YOU LOSE MENU OPEN?");
        menuActive = menuLose;
        Debug.Log("menu should appear");
        menuActive.SetActive(isPaused);
        playerDead = true;
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
        playerController.enabled = false;
        player.transform.position = startPosition;
        playerController.enabled = true;

        if (trueRespawn == true)
        {
            playerScript.HP=playerScript.HPMax;
            playerWeapon.ReloadAmmo();
            playerScript.ClearALLStatusEffects();
        }
    }

    public void ChangeGameGoal(GameGoal NewGoal)
    {
        CurrentGoal = NewGoal;
        updateGameGoal();
    }

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



    #endregion


    public void playAudio(AudioClip audio,float volume=0.5f)
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

    #endregion
}
