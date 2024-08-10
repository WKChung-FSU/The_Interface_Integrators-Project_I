using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    //causes damage to object or player, will default to basic magic bullet
    void takeDamage(int amount,DamageEngine.damageType DamageType=0)
    {

    }

}
