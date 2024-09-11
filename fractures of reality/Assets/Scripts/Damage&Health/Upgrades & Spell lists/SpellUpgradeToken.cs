using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUpgradeToken : MonoBehaviour
{
    [SerializeField] DamageEngine.ElementType elementType;

    private void OnDestroy()
    {
        gameManager.instance.playerWeapon.UpgradedList(elementType);
    }
}
