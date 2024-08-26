using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
   
    public int healthBonus =25;
    public int AmmoBonus = 10;
    

    [SerializeField] bool Health = true;
    

    private void OnTriggerEnter(Collider other)
    {
        if (Health)
        {
            if (other.GetComponent<DestructibleHealthCore>())
            {
                if (other.GetComponent<DestructibleHealthCore>().HP < other.GetComponent<DestructibleHealthCore>().HPMax)
                {
                    Destroy(gameObject);
                    other.GetComponent<DestructibleHealthCore>().HP += healthBonus;
                    if (other.GetComponent<DestructibleHealthCore>().HP > other.GetComponent<DestructibleHealthCore>().HPMax)
                    {
                        other.GetComponent<DestructibleHealthCore>().HP = other.GetComponent<DestructibleHealthCore>().HPMax;
                    }
                }

            }
        }
        else 
        {
            if (other.GetComponent<PlayerWeapon>())
            {
                if (other.GetComponent<PlayerWeapon>().Ammo < other.GetComponent<PlayerWeapon>().maxAmmo)
                {
                    Destroy(gameObject);
                    other.GetComponent<PlayerWeapon>().Ammo += AmmoBonus;
                    if (other.GetComponent<PlayerWeapon>().Ammo > other.GetComponent<PlayerWeapon>().maxAmmo)
                    {
                        other.GetComponent<PlayerWeapon>().Ammo = other.GetComponent<PlayerWeapon>().maxAmmo;
                    }
                }

            }
        }
        
    }
}
