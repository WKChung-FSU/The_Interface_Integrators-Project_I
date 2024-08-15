using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    // all you need to do is flash
    void takeDamage(int amount, DamageEngine.damageType DamageType = 0)
    {

    }
}
