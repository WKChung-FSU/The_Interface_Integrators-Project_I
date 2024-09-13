using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int mana;
    public float[] position;

    public PlayerData (PlayerWeapon ammo, DestructibleHealthCore player)
    {
        health = player.Hp;
        mana = ammo.Ammo;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
