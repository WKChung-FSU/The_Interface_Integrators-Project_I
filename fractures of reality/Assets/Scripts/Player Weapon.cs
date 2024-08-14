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

    #region Menu 
    [SerializeField] int MenuLimit;
    [SerializeField] Transform SpellLaunchPos;
    [SerializeField] GameObject Spell1;
    [SerializeField] GameObject Spell2;
    [SerializeField] int Spell2CostMultiplier;
    [SerializeField] int Spell3CostMultiplier;
    #endregion

    #region wepon Stats
    [SerializeField] int Weapon3Damage;
    [SerializeField] int Weapon3Range;

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
            StartCoroutine(WeaponMenuSystem());
    }

    #region Public Getters
    public
    int GetCurrentAmmo()
    {
        return CurrAmmo;
    }
    int GetMaxAmmo()
    {
        return MaxAmmo;
    }
    bool GetOutOfAmmo()
    {
        return OutOfAmmo;
    }


    WeaponMenu GetCurrentWeapon()
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

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Weapon3Range, ~ignoreMask))
                {
                    #region Debug
                    Debug.Log(hit.collider.name);
                    #endregion

                    IDamage damage = hit.collider.GetComponent<IDamage>();

                    if (damage != null && !OutOfAmmo)
                    {
                        DamageEngine.instance.CalculateDamage(hit.collider, Weapon3Damage, DamageEngine.damageType.Lightning);
                        CurrAmmo--;
                        AmmoTest();
                    }
                }
                break;
        }

        yield return new WaitForSeconds(FireRate);
        isShooting = false;

    }
    IEnumerator WeaponMenuSystem()
    {
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


        yield return new WaitForSeconds(3);
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

}
