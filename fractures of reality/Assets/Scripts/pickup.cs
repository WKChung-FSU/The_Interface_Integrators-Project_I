using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [Header("----- Sounds -----")]
    [SerializeField] AudioClip[] AudioHealth;
    [Range(0, 1)][SerializeField] float AudioHealthVol = 0.5f;

    [SerializeField] AudioClip[] AudioAmmo;
    [Range(0, 1)][SerializeField] float AudioAmmoVol = 0.5f;

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
                    gameManager.instance.playAudio(AudioHealth[Random.Range(0, AudioHealth.Length)], AudioHealthVol);
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
                    gameManager.instance.playAudio(AudioAmmo[Random.Range(0, AudioAmmo.Length)], AudioAmmoVol);
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
