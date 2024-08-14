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
    public playerControler playerScript;
    public PlayerWeapon playerWeapon;
    int enemyCount;
    int healthCount;
    public int healthMax;
    int ammoCount;
    public int ammoMax;
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
        playerScript = player.GetComponent<playerControler>();
        playerWeapon = player.GetComponent<PlayerWeapon>();

        hudEnabled = true;
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
    }
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("f0");

        if (enemyCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);
        }
    }
    public void youLose()
    {
        statePause();
        menuActive = menuLose;
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

    public void DamageFlashScreen()
    {
        StartCoroutine(DamageFlashTimer());
    }

    public void UpdateWeaponIconUI()
    {
        int weapon = (int)playerWeapon.GetCurrentWeapon();
        //disable all weapon icons first
        wMagicMissileIcon.SetActive(false);
        wFireballIcon.SetActive(false);
        wLightningIcon.SetActive(false);

        switch (weapon)
        {
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
        }
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
        healthCount = playerScript.PlayerHP;
        healthCountText.text = healthCount.ToString("F0");
        gameManager.instance.healthBar.fillAmount = (float)healthCount / healthMax;

        //ammo bar
        ammoCount = playerWeapon.GetCurrentAmmo();
        ammoCountText.text = ammoCount.ToString("F0");
        gameManager.instance.ammoBar.fillAmount = (float)ammoCount / ammoMax;
    }

    IEnumerator DamageFlashTimer()
    {
        gameManager.instance.damageFlashScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageFlashScreen.SetActive(false);
    }

    #endregion
}
