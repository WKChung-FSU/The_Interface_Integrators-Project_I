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
    CapsuleCollider bodyCollider;

    [Header("Bullet Hell phase info")]
    [SerializeField] List<GameObject> NorthHazard = new List<GameObject>();
    [SerializeField] List<GameObject> EastHazard = new List<GameObject>();
    [SerializeField] List<GameObject> WestHazard = new List<GameObject>();
    [SerializeField] GameObject northIndicator, eastIndicator, westIndicator;
    int hazardSafeZoneIterator;
    [SerializeField] float timeBetweenIndicatorAndShot;
    [SerializeField] int bHellTimesPerPhase;

    [Header("Add phase information")]
    [SerializeField] List<Transform> spawnLocations = new List<Transform>();
    [SerializeField] List<GameObject> spawnsPool = new List<GameObject>();

    [Header("On Kill")]
    [SerializeField] GameObject walkway;

    // Start is called before the first frame update
    void Start()
    {
        healthCore = transform.GetComponent<DestructibleHealthCore>();
        currentType = healthCore.ElementType;
        bodyCollider = GetComponent<CapsuleCollider>();

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
        EnableInvuln();
    }

    // Update is called once per frame
    void Update()
    {
        if (fightStarted == false)   //uncomment this when you have a trigger for starting the fight
            return;

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
                        if (currentlySummoning == false)
                        {
                            currentlySummoning = true;
                            StartCoroutine(BulletHellPhase());
                        }
                    }
                    else
                    {
                        //logic for summoning adds phase
                        if (currentlySummoning == false)
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

        //  check it's type and apply it's spells

        //check to see what the next type to use even is
        //get a random position in your type allowance list, everything there is what's allowed
        DamageEngine.ElementType nextType = (DamageEngine.ElementType)Random.Range(0, typeAllowances.Count);

       // DamageEngine.ElementType idkman = (DamageEngine.ElementType)nextType;
        switch (nextType)
        {
            case DamageEngine.ElementType.fire:
        //  set spells the dragon can use,
        //  Change the dragons materials
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
        UpdateType(nextType);
        canAttack = true;

    }

    //go to the next phase
    IEnumerator NextPhase()
    {
        //isChangingPhases = true;
        canAttack = false;
        StopCoroutine(aiCore.shoot());
        aiCore.animator.SetTrigger("NextPhase"); //NextPhase calls the animation that calls TypeSwap

        //figure out what phase you are currently in,
        //enter the next phase,
        //if you are in the last phase, go to the first phase

        //play animation for swapping phases

        switch (currentPhase)
        {
            case (int)BattlePhase.attack:
                {
                    currentPhase++;
                    //DisableInvuln();  disabling this because it's not fun.
                    break;
                }
            case (int)BattlePhase.defend:
                {
                    //EnableInvuln();   disabling this because it's not fun.
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
        yield return new WaitForSeconds(1);
        for (int i = 0; i < spawnLocations.Count; i++)
        {
            Instantiate(spawnsPool[Random.Range(0, spawnsPool.Count)], spawnLocations[i].transform.position, spawnLocations[i].transform.rotation);
        }
        yield return new WaitForSeconds(phaseTimer / 2);
        currentlySummoning = false;
    }

    IEnumerator BulletHellPhase()
    {
        //choose direction of hazard
        int randomPos = Random.Range(0, (int)Time.time) % 3;
        hazardSafeZoneIterator = Random.Range(0, (int)Time.time) % NorthHazard.Count; // they all have the same amount of hazards, not the best solution
        switch (randomPos)
        {
            case 0:
                {
                    //west
                    //enable the wall of hazards
                    EnableHazardList(ref WestHazard);
                    //notify the player where this is happening
                    westIndicator.SetActive(true);
                    //delay
                    yield return new WaitForSeconds(timeBetweenIndicatorAndShot);
                    //now shoot from that direction
                    StartCoroutine(HazardShoot(WestHazard));
                    yield return new WaitForSeconds(1);
                    DisableHazardList(ref WestHazard);
                    westIndicator.SetActive(false);
                    //maybe here play sounds and make pretty
                    break;
                }
            case 1:
                {
                    //north
                    EnableHazardList(ref NorthHazard);
                    northIndicator.SetActive(true);
                    yield return new WaitForSeconds(timeBetweenIndicatorAndShot);
                    StartCoroutine(HazardShoot(NorthHazard));
                    yield return new WaitForSeconds(1);
                    DisableHazardList(ref NorthHazard);
                    northIndicator.SetActive(false);
                    //above
                    break;
                }
            case 2:
                {
                    //east
                    EnableHazardList(ref EastHazard);
                    eastIndicator.SetActive(true);
                    yield return new WaitForSeconds(timeBetweenIndicatorAndShot);
                    StartCoroutine(HazardShoot(EastHazard));
                    yield return new WaitForSeconds(1);
                    DisableHazardList(ref EastHazard);
                    eastIndicator.SetActive(false);
                    //above
                    break;
                }
        }
        yield return new WaitForSeconds((phaseTimer / bHellTimesPerPhase) - 0.9f);
        currentlySummoning = false;
    }

    void EnableHazardList(ref List<GameObject> hazardList)
    {

        for (int i = 0; i < hazardList.Count; i++)
        {
            if (i != hazardSafeZoneIterator)
                hazardList[i].SetActive(true);
        }
    }
    void DisableHazardList(ref List<GameObject> hazardList)
    {
        for (int i = 0; i < hazardList.Count; i++)
        {
            hazardList[i].SetActive(false);
        }
    }

    IEnumerator HazardShoot(List<GameObject> hazardList)
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < hazardList.Count; i++)
        {
            if (i != hazardSafeZoneIterator)
                Instantiate(aiCore.spellsList[0], hazardList[i].transform.position, hazardList[i].transform.localRotation);
        }
    }

    void EnableInvuln()
    {
        bodyCollider.enabled = false;
    }

    void DisableInvuln()
    {
        bodyCollider.enabled = true;
    }

    private void OnDestroy()
    {
        //This is where the dragon dies, allow the player to exit
        walkway.SetActive(true);
    }

    public void StartFinalBattle()
    {
        fightStarted = true;
        DisableInvuln();
    }
}
