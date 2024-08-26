using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEditor.SearchService;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    #region Player
    public GameObject player;
    public DestructibleHealthCore playerScript;
    public PlayerWeapon playerWeapon;
    CharacterController playerController;
    int enemyCount;
    Vector3 startPosition;
    CheckpointSystem lastCheckPoint;
    #endregion

    #region UI
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
    public bool isPaused;
    public bool hudEnabled;
    public bool playerDead;

    //weapon icons
    [SerializeField] GameObject wMagicMissileIcon;
    [SerializeField] GameObject wFireballIcon;
    [SerializeField] GameObject wLightningIcon;

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

        //update health bar
        UpdateHUD();

        //check if switch weapon button is pressed
        //then call weapon swap UI code
        //if (Input.GetButtonDown("Switch Weapon"))
        //{
        //    UpdateWeaponIconUI();
        //}
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
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("f0");

        if (enemyCount <= 0)
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
        int weapon = playerWeapon.GetCurrentWeapon();
        //disable all weapon icons first
        wMagicMissileIcon.SetActive(false);
        wFireballIcon.SetActive(false);
        wLightningIcon.SetActive(false);

        switch (weapon)
        {
            default:
                {
                    wMagicMissileIcon.SetActive(true);
                    break;
                }
            case 0: //magic missile
                {
                    //enable the correct icon
                    wMagicMissileIcon.SetActive(true);
                    break;
                }
            case 1: //Fireball
                {
                    wFireballIcon.SetActive(true);
                    break;
                }
            case 2: //Lightning
                {
                    wLightningIcon.SetActive(true);
                    break;
                }
                //TODO: Add the rest of the spells (Ice, water, earth, air)
        }
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

    #endregion


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
