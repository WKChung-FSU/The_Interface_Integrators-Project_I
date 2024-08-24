using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour, IDamage
{
    [Header("-----RequiredFields-----")]
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] LineRenderer lightningVisual;
    [SerializeField] Transform SpellLaunchPos;

    #region Spells
    [Header("-----PlayerSpells-----")]
    //all should be summonable
    [SerializeField] List<GameObject> primarySpells = new List<GameObject>();
    [Header("remember element 2 Must Be lightning Bolt")]
    [SerializeField] List<GameObject> secondarySpells = new List<GameObject>();
    [SerializeField] List<int> SpellCost = new List<int>();
    [Range(0.05f, 2)][SerializeField] float FireRate;

    [Range(1, 100)][SerializeField]int MaxAmmo;
    bool OutOfAmmo;
    // remove(depreciated)
    public enum WeaponMenu { MagicMissile, Fireball, Lightning }

    int CurrAmmo;
    bool isShooting;
    // remove(depreciated)
    WeaponMenu weaponMenu;

    int currentWeapon;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        CurrAmmo = MaxAmmo;
        //gameManager.instance.ammoMax = MaxAmmo;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(ShootPrimary());

        if (Input.GetButton("Shoot 2") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(ShootSecondary());

        if (Input.GetButtonDown("Switch Weapon") || Input.GetAxis("Mouse ScrollWheel") != 0)
            //No longer coroutine -CM
            WeaponMenuSystem();

        if (Input.GetButtonDown("Reload"))
        {
            CurrAmmo = MaxAmmo;
        }
    }

    #region Public Getters

    public int GetCurrentAmmo()
    {
        return CurrAmmo;
    }
    public int GetMaxAmmo()
    {
        return MaxAmmo;
    }
    public bool GetOutOfAmmo()
    {
        return OutOfAmmo;
    }

    // remove(depreciated)
    public int GetCurrentWeapon()
    {
      
        return currentWeapon;
    }

    private
    #endregion
        IEnumerator ShootPrimary()
    {
        isShooting = true;

        switch (currentWeapon)
        {
            // All primary spells are summons
            default:
                Instantiate(primarySpells[currentWeapon], SpellLaunchPos.position, SpellLaunchPos.rotation);
                break;
        }
        yield return new WaitForSeconds(FireRate);
        isShooting = false;
    }


    IEnumerator ShootSecondary()
    {
        isShooting = true;

        RaycastHit hit;
        switch (currentWeapon)
        {
            default:
              
                if (((CurrAmmo - SpellCost[currentWeapon]) >= 0) && secondarySpells[currentWeapon] != null)
                {
                    Instantiate(secondarySpells[currentWeapon], SpellLaunchPos.position, SpellLaunchPos.rotation);
                    CurrAmmo -= SpellCost[currentWeapon];
                }
                else if (secondarySpells[currentWeapon] == null)
                {
                    Debug.Log("Something Failed in ShootSecondary");
                }
               

                break;
            //lightning spell
            case 2:
               
                {
                    #region Debug
                    //Debug.Log(hit.collider.name);
                    #endregion
                    if ((CurrAmmo - SpellCost[currentWeapon]) >= 0)
                    {
                        AttackCore SpellCore = secondarySpells[currentWeapon].GetComponent<AttackCore>();
                        hit = SpellCore.CastHitScanAttack(ignoreMask);
                        //test -CM
                        //Visual of lightning being cast 
                        lightningVisual.useWorldSpace = true;
                        lightningVisual.SetPosition(0, SpellLaunchPos.position);
                        lightningVisual.SetPosition(1, hit.point);
                        CurrAmmo-=SpellCost[currentWeapon];
                    }

                    //lightning delay
                    //coconut.jpeg
                    //if lightning delay is here it won't show unless you can deal damage to whatever you are looking at 
             
                    //if this is here it will always show the visual
                    if (!OutOfAmmo)
                    {
                        StartCoroutine(LightningDelay());
                    }
                    AmmoTest();
                }
                break;
        }

        yield return new WaitForSeconds(FireRate);
        isShooting = false;

    }
    void WeaponMenuSystem()
    {
        //changed from IEnumerator to void -CM
        if ((Input.GetAxis("Switch Weapon") > 0 || Input.GetAxis("Mouse ScrollWheel") > 0) && currentWeapon < primarySpells.Count - 1)
        {
            currentWeapon++;
        }
        else if ((Input.GetAxis("Switch Weapon") < 0 || Input.GetAxis("Mouse ScrollWheel") < 0) && currentWeapon > 0)
        {
            currentWeapon--;
        }
        gameManager.instance.UpdateWeaponIconUI();

    }
    void AmmoTest()
    {
        if (CurrAmmo > 0)
        {
            OutOfAmmo = false;
        }
        else
        {
            OutOfAmmo = true;
            CurrAmmo = 0;
        }
    }

    IEnumerator LightningDelay()
    {
        lightningVisual.enabled = true;
        yield return new WaitForSeconds(0.1f);
        lightningVisual.enabled = false;
    }

}
