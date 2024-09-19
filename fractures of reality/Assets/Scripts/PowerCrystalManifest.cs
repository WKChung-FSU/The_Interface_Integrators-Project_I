using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PowerCrystalManifest : ScriptableObject
{
    [SerializeField] int amountOfCrystals = 5;
    [SerializeField] List<DamageEngine.ElementType>CrystalsDestroyed = new List<DamageEngine.ElementType>();
    private PlayerWeapon weapon;
    

    public void SetDestroyedCrystalOfType(DamageEngine.ElementType type)
    {
        if (!CrystalsDestroyed.Contains(type))
        {
            CrystalsDestroyed.Add(type);

        weapon = gameManager.instance.playerWeapon;
        weapon.UpgradedList(type);
        weapon.UpdateSpellList();
        CrystalDestroyed();
        gameManager.instance.UpdateCrystalText();
        }
    }

    public List<DamageEngine.ElementType> DestroyList
    {
        get { return CrystalsDestroyed; }
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
        CrystalsDestroyed.Clear();
        amountOfCrystals = 5;
    }
}
