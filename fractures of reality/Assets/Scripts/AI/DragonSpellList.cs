using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DragonSpellList : ScriptableObject
{
    [SerializeField] public List<GameObject> spellList = new List<GameObject>();
    [SerializeField] Material dragonMaterialOfThisType;

    public int GetSpellListSize() 
    { 
        return spellList.Count; 
    }

    public Material GetMaterial() 
    { 
        return dragonMaterialOfThisType; 
    }
}
