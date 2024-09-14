using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonScript : MonoBehaviour
{
    [Header("Battle Info")]
    bool fightStarted = false;
    [SerializeField] int phaseTimer = 20;
    //use a scriptable game object that contains all the spells the dragon can cast.
    [SerializeField] DragonSpellList normalSpells;
    [SerializeField] DragonSpellList fireSpells;
    [SerializeField] DragonSpellList lightningSpells;
    [SerializeField] DragonSpellList iceSpells;
    [SerializeField] DragonSpellList earthSpells;
    [SerializeField] DragonSpellList waterSpells;
    [SerializeField] enum BattlePhase { attack, defend, summon };
    int currentPhase;
    Transform shootPosition;
    private bool isChangingPhases = false;
    [SerializeField] int attackDelay;
    [SerializeField] Transform shootPos;
    private List<GameObject> currentSpellList = new List<GameObject>();

    [Header("Attributes")]
    private DestructibleHealthCore healthCore;
    private DamageEngine.ElementType currentType;
    //scriptable game object that has all the dragon's materials
    [SerializeField] Renderer model;
    //private Dictionary<DamageEngine.ElementType, bool> typeAllowance = new Dictionary<DamageEngine.ElementType, bool>();
    private List<DamageEngine.ElementType> typeAllowances = new List<DamageEngine.ElementType>();


    // Start is called before the first frame update
    void Start()
    {
        healthCore = transform.GetComponent<DestructibleHealthCore>();
        currentType = healthCore.ElementType;

        SetSpells(normalSpells);

        typeAllowances.Add(DamageEngine.ElementType.Normal);
        if (gameManager.instance.crystalManifest.fireCrystalDestroyed == false)
            typeAllowances.Add(DamageEngine.ElementType.fire);
        if (gameManager.instance.crystalManifest.lightningCrystalDestroyed == false)
            typeAllowances.Add(DamageEngine.ElementType.Lightning);
        if (gameManager.instance.crystalManifest.iceCrystalDestroyed == false)
            typeAllowances.Add(DamageEngine.ElementType.Ice);
        if (gameManager.instance.crystalManifest.earthCrystalDestroyed == false)
            typeAllowances.Add(DamageEngine.ElementType.Earth);
        if (gameManager.instance.crystalManifest.waterCrystalDestroyed == false)
            typeAllowances.Add(DamageEngine.ElementType.Water);

    }

    // Update is called once per frame
    void Update()
    {
        //if (fightStarted == false)            uncomment this when you have a trigger for starting the fight
        //    return;

        if (isChangingPhases == false)
        {
            StartCoroutine(NextPhase());
        }
        switch (currentPhase)
        {
            case (int)BattlePhase.attack:
                {
                    //attack the player
                    break;
                }
            case (int)BattlePhase.defend:
                {

                    break;
                }
            case (int)BattlePhase.summon:
                {
                    //choose to either do bullet-hell or summon adds
                    break;
                }
        }
    }

    //TODO: methods for handling finite state machine logic
    //TODO: Methods for each state

    //enter state
    //exit state
    //update state
    //get next state
    //

    void TypeSwap()
    {
        //dragon cannot be hurt
        //check to see what the next type to use even is
        //  set spells the dragon can use,
        //  Change the dragons materials

        //get a random position in your type allowance list, everything there is what's allowed
        //  check it's type and apply it's spells

        int nextType = Random.Range(0, typeAllowances.Count);

        DamageEngine.ElementType idkman = (DamageEngine.ElementType)nextType;
        switch (idkman)
        {
            case DamageEngine.ElementType.fire:
                SetSpells(fireSpells);
                break;
            case DamageEngine.ElementType.Lightning:
                SetSpells(lightningSpells);
                break;
            case DamageEngine.ElementType.Ice:
                SetSpells(iceSpells);
                break;
            case DamageEngine.ElementType.Earth:
                SetSpells(earthSpells);
                break;
            case DamageEngine.ElementType.Water:
                SetSpells(waterSpells);
                break;
            default:
                SetSpells(normalSpells);
                break;
        }
        UpdateType(idkman);
        // else
        // {
        //     UpdateType(DamageEngine.ElementType.Normal);
        //     SetSpells(normalSpells);
        //
        // }

        //TODO: add something to the healthcore that I can call here to change the icon on demand
        Debug.Log(healthCore.ElementType);
        //dragon can now be hurt,
        //go to next phase
    }

    //go to the next phase
    IEnumerator NextPhase()
    {
        isChangingPhases = true;
        yield return new WaitForSeconds(phaseTimer);
        //figure out what phase you are currently in,
        //enter the next phase,
        //if you are in the last phase, go to the first phase

        switch (currentPhase)
        {
            case (int)BattlePhase.attack:
                {
                    currentPhase++;
                    break;
                }
            case (int)BattlePhase.defend:
                {
                    TypeSwap();
                    break;
                }
            case (int)BattlePhase.summon:
                {
                    currentPhase = 0;
                    break;
                }
        }
        Debug.Log((BattlePhase)currentPhase);
        isChangingPhases = false;
    }

    void UpdateType(DamageEngine.ElementType type)
    {
        healthCore.ElementType = type;
        healthCore.SetElementalTypeGraphic(type);
        currentType = healthCore.ElementType;

    }

    void SetSpells(DragonSpellList spells)
    {
        currentSpellList.Clear();
        for (int i = 0; i < spells.GetSpellListSize(); i++)
        {
            currentSpellList.Add(spells.spellList[i]);
        }
        model.material = spells.GetMaterial();
    }

    public void StartFinalBattle()
    {
        fightStarted = true;
    }
}
