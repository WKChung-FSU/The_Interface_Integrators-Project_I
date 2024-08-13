using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour, IDamage
{
    #region Test Variables
  

    #endregion
    #region wepon Stats

    [SerializeField] LayerMask ignoreMask;
    [SerializeField] DamageEngine.damageType Weapon1Type;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeField] int MaxAmmo;
    [SerializeField] int Spell2Multiplier;
    [SerializeField] bool OutOfAmmo;
     int CurrAmmo;
    bool isShooting;
    #endregion

    #region Menu 
    [SerializeField] int MenuLimit;
    [SerializeField] Transform SpellLaunchPos;
    [SerializeField] GameObject Spell2;
    //0= basic, 1=fireball
    public int currentWeapon;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        CurrAmmo = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(Shoot());
        if(Input.GetButtonDown("Switch Weapon"))
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
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
                {
                    Debug.Log(hit.collider.name);

                    IDamage damage = hit.collider.GetComponent<IDamage>();

                    if (damage != null && !OutOfAmmo)
                    {
                        damage.takeDamage(shootDamage, Weapon1Type);
                        CurrAmmo--;
                        AmmoTest();
                    }
                };
                break;
            //Fireball
            case 1:
                if ((CurrAmmo - Spell2Multiplier) > 0) { 
                Instantiate(Spell2, SpellLaunchPos.position, transform.rotation);
                CurrAmmo -= Spell2Multiplier;
                }
                break;
                
            case 2:


            break;
        }
       
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    
    }
    IEnumerator WeaponMenuSystem()
    {
        if (Input.GetAxis("Switch Weapon") > 0)
        {
            currentWeapon++;
        }
        else if (Input.GetAxis("Switch Weapon") < 0)
        {
            currentWeapon++;
        }

        if (currentWeapon < 0)
        {
            currentWeapon = (MenuLimit - 1);
        }
        else if (currentWeapon>(MenuLimit-1)) 
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
            CurrAmmo= 0;
        }
    }

    }
