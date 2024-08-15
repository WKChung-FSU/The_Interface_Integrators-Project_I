using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour, IDamage
{
    #region Test Variables


    #endregion
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] LineRenderer lightningVisual;

    #region Menu 
    [SerializeField] int MenuLimit;
    [SerializeField] Transform SpellLaunchPos;
    [SerializeField] GameObject Spell1;
    [SerializeField] GameObject Spell2;
   
    #endregion

    #region wepon Stats
    [SerializeField] int Spell2CostMultiplier;
    [SerializeField] int spell3Damage;
    [SerializeField] int spell3Range;
    [SerializeField] int Spell3CostMultiplier;
    [SerializeField] float FireRate;
    [SerializeField] int MaxAmmo;
    [SerializeField] bool OutOfAmmo;

    public enum WeaponMenu { MagicMissile, Fireball, Lightning }

    int CurrAmmo;
    bool isShooting;
    WeaponMenu weaponMenu;
    //0= magic missile, 1=fireball, 2=lightning
    int currentWeapon;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        CurrAmmo = MaxAmmo;
        gameManager.instance.ammoMax = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(Shoot());
        if (Input.GetButtonDown("Switch Weapon"))
            //No longer coroutine -CM
            WeaponMenuSystem();

        if (Input.GetButtonDown("Reload"))
        {
            CurrAmmo=MaxAmmo;
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


    public WeaponMenu GetCurrentWeapon()
    {
        weaponMenu = (WeaponMenu)currentWeapon;
        return weaponMenu;
    }

    private
    #endregion
    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        switch (currentWeapon)
        {
            //Basic spell
            case 0:
                if ((CurrAmmo) > 0)
                {
                    Instantiate(Spell1, SpellLaunchPos.position, SpellLaunchPos.rotation);
                    CurrAmmo--;
                }
                break;

            //Fireball
            case 1:
                if ((CurrAmmo - Spell2CostMultiplier) >= 0)
                {
                    Instantiate(Spell2, SpellLaunchPos.position, SpellLaunchPos.rotation);
                    CurrAmmo -= Spell2CostMultiplier;
                }
                break;

            //lightning spell
            case 2:


                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, spell3Range, ~ignoreMask))
                {
                    #region Debug
                    Debug.Log(hit.collider.name);
                    #endregion
                    if ((CurrAmmo - Spell3CostMultiplier) >= 0) { 
                        //test -CM
                        //Visual of lightning being cast 
                        lightningVisual.useWorldSpace = true;
                    lightningVisual.SetPosition(0, SpellLaunchPos.position);
                    lightningVisual.SetPosition(1, hit.point);
                        CurrAmmo--;
                    }
                    IDamage damage = hit.collider.GetComponent<IDamage>();
                    if (damage != null)
                    {
                        DamageEngine.instance.CalculateDamage(hit.collider, spell3Damage, DamageEngine.damageType.Lightning);
                        //lightning delay
                        //coconut.jpeg

                        //if lightning delay is here it won't show unless you can deal damage to whatever you are looking at 
                    }
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
        if (Input.GetAxis("Switch Weapon") > 0 || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentWeapon++;
        }
        else if (Input.GetAxis("Switch Weapon") < 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentWeapon--;
        }

        if (currentWeapon < 0)
        {
            currentWeapon = (MenuLimit - 1);
        }
        else if (currentWeapon > (MenuLimit - 1))
        {
            currentWeapon = 0;
        }


        gameManager.instance.UpdateWeaponIconUI();
        //yield return new WaitForSeconds(3);
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
