using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    // all you need to do is flash, does not need any arguments by default
    void damageEffect(int amount=0, DamageEngine.ElementType Element = 0)
    {

    }
}
