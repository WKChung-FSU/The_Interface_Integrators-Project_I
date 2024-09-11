using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class SpellList : ScriptableObject
{
    [SerializeField] List<GameObject> primarySpells = new List<GameObject>();
    [SerializeField] List<int> SpellCostPrimary = new List<int>();
    [Range(0.05f, 2)][SerializeField] List<float> primaryFireRate=new List<float>();
    [SerializeField] List<GameObject> secondarySpells = new List<GameObject>();
    [SerializeField] List<int> SpellCostSecondary = new List<int>();
    [Range(0.05f, 2)][SerializeField] List<float> secondaryFireRate = new List<float>();

    public List<GameObject> PrimarySpells
    {

    get { return primarySpells; } 
    }
    public List<int> PrimarySpellCost
    {
        get { return SpellCostPrimary; }
    }
    public List<float> PrimaryFireRate
    {
        get { return primaryFireRate; }
    }
    public List<GameObject> SecondarySpells
    {
        get { return secondarySpells; }
    }
    public List<int> SecondarySpellCost
    {
        get { return SpellCostSecondary; }
    }
    public List<float> SecondaryFireRate
    {
        get { return secondaryFireRate; }
    }
}


