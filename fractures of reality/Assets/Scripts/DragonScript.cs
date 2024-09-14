using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonScript : MonoBehaviour
{
    [Header("Battle Info")]
    [SerializeField] AICore aiCore;
    bool fightStarted = false;
    [SerializeField] int phaseTimer = 20;
    //use a scriptable game object that contains all the spells the dragon can cast.
    [SerializeField] DragonSpellList normalSpells;
    [SerializeField] DragonSpellList fireSpells;
    [SerializeField] DragonSpellList lightningSpells;
    [SerializeField] DragonSpellList iceSpells;
    [SerializeField] DragonSpellList earthSpells;
    [SerializeField] DragonSpellList waterSpells;
    [SerializeField] enum BattlePhase { attack, defend };
    int currentPhase;
    private bool isChangingPhases = false;
    private bool canAttack;
    private bool bHellPhase;

    [Header("Attributes")]
    private DestructibleHealthCore healthCore;
    private DamageEngine.ElementType currentType;
    [SerializeField] Renderer model;
    private List<DamageEngine.ElementType> typeAllowances = new List<DamageEngine.ElementType>();
    private bool currentlySummoning = false;

    [Header("Bullet Hell phase info")]
    [SerializeField] List<Transform> NorthHazard = new List<Transform>();
    [SerializeField] List<Transform> EastHazard = new List<Transform>();
    [SerializeField] List<Transform> WestHazard = new List<Transform>();

    [Header("Add phase information")]
    [SerializeField] List<Transform> spawnLocations = new List<Transform>();
    [SerializeField] List<GameObject> spawnsPool = new List<GameObject>();

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

        //choose to either do bullet-hell or summon adds
        int rando = Random.Range(0, (int)Time.time) % 1;
        if (rando == 0)
        {
            Debug.Log("Summon Phase first");
            bHellPhase = false;
            //summon phase
        }
        else
        {
            Debug.Log("Bullet Hell Phase first");
            bHellPhase = true;
            //bullet-hell phase
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if (fightStarted == false)            uncomment this when you have a trigger for starting the fight
        //    return;

        aiCore.SmoothAnimations();

        if (isChangingPhases == false)
        {
            isChangingPhases = true;
            StartCoroutine(NextPhase());
        }
        switch (currentPhase)
        {
            case (int)BattlePhase.attack:
                {
                    //attack the player
                    if (canAttack == true)
                    StartCoroutine(aiCore.shoot());
                    break;
                }
            case (int)BattlePhase.defend:
                {
                    Debug.Log("Summon or BHell phase");
                    if (bHellPhase)
                    {
                        //logic for bullet hell phase
                    }
                    else
                    {
                        //logic for summoning adds phase
                        if(currentlySummoning == false)
                        {
                            currentlySummoning = true;
                            StartCoroutine(SummonFromPool());

                        }
                    }
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
        //gets called by the animation
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
        canAttack = true;

        // else
        // {
        //     UpdateType(DamageEngine.ElementType.Normal);
        //     SetSpells(normalSpells);
        //
        // }

        //TODO: add something to the healthcore that I can call here to change the icon on demand
        //Debug.Log(healthCore.ElementType);
        //dragon can now be hurt,
        //go to next phase
    }

    //go to the next phase
    IEnumerator NextPhase()
    {
        //isChangingPhases = true;
        aiCore.animator.SetTrigger("NextPhase"); //NextPhase calls the animation that calls TypeSwap
        canAttack = false;

        //figure out what phase you are currently in,
        //enter the next phase,
        //if you are in the last phase, go to the first phase

        //play animation for swapping phases

        switch (currentPhase)
        {
            case (int)BattlePhase.attack:
                {
                    currentPhase++;
                    break;
                }
            case (int)BattlePhase.defend:
                {
                    //TypeSwap();
                    bHellPhase = !bHellPhase;
                    currentPhase = 0;
                    break;
                }
        }
        Debug.Log((BattlePhase)currentPhase);
        yield return new WaitForSeconds(phaseTimer);
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
        aiCore.spellsList.Clear();
        for (int i = 0; i < spells.GetSpellListSize(); i++)
        {
            aiCore.spellsList.Add(spells.spellList[i]);
        }
        model.material = spells.GetMaterial();
        healthCore.SetColorOriginal(spells.GetMaterial());
    }

    IEnumerator SummonFromPool()
    {
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            Instantiate(spawnsPool[Random.Range(0, spawnsPool.Count)], spawnLocations[i].transform.position, spawnLocations[i].transform.rotation);
        }
        yield return new WaitForSeconds(phaseTimer/2);
        currentlySummoning = false;
    }

    public void StartFinalBattle()
    {
        fightStarted = true;
    }
}
