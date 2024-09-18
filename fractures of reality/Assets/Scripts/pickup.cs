using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    enum PickupType { Health, Mana}
    [Header("-----MainStats-----")]
    [SerializeField] PickupType type;
    [Header("----- Sounds -----")]
    [SerializeField] AudioClip[] AudioHealth;
    [Range(0, 1)][SerializeField] float AudioHealthVol = 0.5f;

    [SerializeField] AudioClip[] AudioAmmo;
    [Range(0, 1)][SerializeField] float AudioAmmoVol = 0.5f;

    public int healthBonus = 25;
    public int AmmoBonus = 10;


    private void OnTriggerEnter(Collider other)
    {
        PlayerWeapon weapon = other.GetComponent<PlayerWeapon>();
        // changed code to a switch statement -WC
        switch (type){
            default:
                Debug.LogError("Error in Pickup Type");
                break;

            case PickupType.Health:
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
                break;
                //Changed code to refer to PlayerWeapon "weapon" to reduce repeated code - WC
            case PickupType.Mana:
                if (weapon)
                {
                    if (weapon.Ammo < weapon.maxAmmo)
                    {
                        gameManager.instance.playAudio(AudioAmmo[Random.Range(0, AudioAmmo.Length)], AudioAmmoVol);
                        Destroy(gameObject);
                        weapon.Ammo += AmmoBonus;
                        if (weapon.Ammo > weapon.maxAmmo)
                        {
                            weapon.Ammo = weapon.maxAmmo;
                        }
                    }

                }
                break;

        }
    }
}
