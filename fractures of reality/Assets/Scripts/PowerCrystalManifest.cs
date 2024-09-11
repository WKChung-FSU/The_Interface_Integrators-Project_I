using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PowerCrystalManifest : ScriptableObject
{
    [SerializeField] public bool fireCrystalDestroyed = false;
    [SerializeField] public bool lightningCrystalDestroyed = false;
    [SerializeField] public bool iceCrystalDestroyed = false;
    [SerializeField] public bool earthCrystalDestroyed = false;
    [SerializeField] public bool waterCrystalDestroyed = false;

    private int amountOfCrystals = 5;

    public void SetDestroyedCrystalOfType(DamageEngine.ElementType type)
    {
        switch (type)
        {
            case DamageEngine.ElementType.fire:
                {
                    fireCrystalDestroyed = true;
                    break;
                }
            case DamageEngine.ElementType.Lightning:
                {
                    lightningCrystalDestroyed = true;
                    break;
                }
            case DamageEngine.ElementType.Ice:
                {
                    iceCrystalDestroyed = true;
                    break;
                }
            case DamageEngine.ElementType.Earth:
                {
                    earthCrystalDestroyed = true;
                    break;
                }
            case DamageEngine.ElementType.Water:
                {
                    waterCrystalDestroyed = true;
                    break;
                }
        }
        CrystalDestroyed();
    }

    public int GetAmountOfCrystals()
    {
        return amountOfCrystals;
    }
    public void CrystalDestroyed()
    {
        amountOfCrystals--;
    }

    public void ResetManifest()
    {
        fireCrystalDestroyed = false;
        lightningCrystalDestroyed = false;
        iceCrystalDestroyed = false;
        earthCrystalDestroyed = false;
        waterCrystalDestroyed = false;

        amountOfCrystals = 5;
    }
}
