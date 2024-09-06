using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class DestructibleHealthCore : MonoBehaviour, IDamage
{

    public enum statusEffect { normal, fireBurning, LightningShocked, IceFrozen, Earth, Windborn, WaterWet }
    [Range(0, 500)][SerializeField] int Hp = 100;
    [SerializeField] Collider mObjectCollider;
    [SerializeField] DamageEngine.ElementType elementType;
    [SerializeField] public bool IsMandatory = true;
    [SerializeField] bool isPlayer = false;
    [SerializeField] Renderer modelColor;
    [SerializeField] TMP_Text textHP;
    [SerializeField] GameObject enemyHud;
    [SerializeField] DamageParticlesList particles;
    [Header("-----Item Spawning-----")]
    [SerializeField] bool SpawnsItemOnDeath;
    [SerializeField] GameObject[] deathSpawnItems;

    private playerController player;
    private NavMeshAgent target;
    private float speedCache;
    int sprintModCache;

    private TypeIcon iconType;

    public Dictionary<DamageEngine.ElementType, bool> statusDictionary = new Dictionary<DamageEngine.ElementType, bool>();


    int MaxHealth;
    Color colorOriginal;

    private int burnTick = 0;

    // Start is called before the first frame update
    void Start()
    {
        //modelColor = mObject.GetComponent<Renderer>().material.color;
        MaxHealth = Hp;
        colorOriginal = modelColor.material.color;
        // if (isPlayer)
        //  gameManager.instance.healthMax = MaxHealth;
        if (IsMandatory)
            gameManager.instance.updateGameGoal(1);

        if (isPlayer == false && textHP != null)
        {
            textHP.text = HP.ToString("F0");
            //Tell the hud object to change the icon
            enemyHud.GetComponent<TypeIcon>().EnableElementTypeGraphic(elementType);
        }

        //add each type to the status effect dictionary and set it false

        for (int i = 0; i <= 6; i++)
        {
            statusDictionary.Add((DamageEngine.ElementType)i, false);
        }

        //if has navmesh, it's an enemy, else it's the player
        if (GetComponent<NavMeshAgent>() != null)
        {
            target = GetComponent<NavMeshAgent>();
            speedCache = target.speed;
        }
        else if (GetComponent<playerController>())
        {
            player = GetComponent<playerController>();
            sprintModCache = player.GetSprintMod();
            speedCache = player.GetOriginalSpeed();
        }
    }
    public DamageEngine.ElementType ElementType
    {
        get
        {
            return elementType;
        }
        set
        {
            elementType = value;
        }

    }

    public int HP
    {
        get
        {

            return Hp;
        }

        set
        {
            Hp = value;
        }
    }
    public int HPMax
    {
        get
        {
            return MaxHealth;
        }

        set
        {
            MaxHealth = value;
        }
    }

    public GameObject GETDeathSpawnItems()
    {

        return deathSpawnItems[UnityEngine.Random.Range(0, deathSpawnItems.Length)];

    }
    public bool GETSpawnsItemOnDeath()
    {
        return SpawnsItemOnDeath;
    }

    public void SetNewElementType(DamageEngine.ElementType type)
    {
        elementType = type;
        if (textHP != null)
        {
            enemyHud.GetComponent<TypeIcon>().EnableElementTypeGraphic(elementType);
        }
    }

    //The flash of color upon damage and status effect being applied
    public void damageEffect(int amount, DamageEngine.ElementType type)
    {
        // updated the key generator

        //if (statusDictionary[type] == false)
        //{
        //    statusDictionary[type] = true;
        //}

        // if stacking multiple different effects becomes an issue, put the following switch in an else statement

        switch (type)
        {
            case DamageEngine.ElementType.Normal:
                {

                    break;
                }
            case DamageEngine.ElementType.fire:
                {

                    if (statusDictionary[DamageEngine.ElementType.Water] == true)
                    {
                        ClearStatusEffect(DamageEngine.ElementType.Water);
                        break;
                    }
                    Instantiate(particles.burnParticle, mObjectCollider.transform);
                    SetStatusEffect(DamageEngine.ElementType.fire);

                    if (burnTick <= 5)
                    {
                        StartCoroutine(FireBurn());
                    }
                    else
                    {
                        burnTick = 0;
                    }
                    break;
                }
            case DamageEngine.ElementType.Lightning:
                {
                    //if you are wet and not lightning
                    if (statusDictionary[DamageEngine.ElementType.Water] == true && statusDictionary[DamageEngine.ElementType.Lightning] == false)
                    {
                        SetStatusEffect(DamageEngine.ElementType.Lightning);
                        Instantiate(DamageEngine.instance.lightningAOE, mObjectCollider.transform.position, mObjectCollider.transform.rotation);
                    }
                    else
                    {
                        SetStatusEffect(DamageEngine.ElementType.Lightning);
                    }

                    //check those around you
                    //  if they are wet and not lightning
                    //      cast the line renderer from you to the others
                    //          make them lightning
                    //          repeat

                    break;
                }
            case DamageEngine.ElementType.Ice:
                {
                    //if you are fire
                    if (statusDictionary[DamageEngine.ElementType.fire] == true)
                    {
                        //extinguish and you are now wet
                        ClearStatusEffect(DamageEngine.ElementType.fire);
                        SetStatusEffect(DamageEngine.ElementType.Water);
                        break;
                    }

                    else if (statusDictionary[DamageEngine.ElementType.Ice] == false)
                    {
                        //if target has a navmesh agent, it's an enemy
                        //  use the navmesh speed to change the enemy speed
                        if (target != null)
                        {
                            //enemy hit
                            StartCoroutine(SlowTarget(target));
                        }
                        //or if it has a playercontroller, it's the player
                        else
                        {
                            //  the player controller has a speed variable
                            StartCoroutine(SlowPlayer());
                        }
                        SetStatusEffect(DamageEngine.ElementType.Ice);
                        Instantiate(particles.iceParticle, mObjectCollider.transform);

                    }
                    break;
                }
            case DamageEngine.ElementType.Earth:
                {
                    //if you are wet
                    //  do big damage
                    //if you are lightning
                    //  not anymore
                    break;
                }
            case DamageEngine.ElementType.Wind:
                {
                    //if you are on fire
                    //  deal big damage
                    //if you are wet
                    //  not anymore
                    break;
                }
            case DamageEngine.ElementType.Water:
                {
                    //if you are on fire
                    if (statusDictionary[DamageEngine.ElementType.fire] == true)
                    {
                        burnTick = 0;
                        ClearStatusEffect(DamageEngine.ElementType.fire);
                        Debug.Log("You were on fire but now you are not");
                        break;
                    }
                    //otherwise
                    else
                    {
                        SetStatusEffect(DamageEngine.ElementType.Water);
                        Instantiate(particles.wetParticle, mObjectCollider.transform);
                    }
                    break;
                }
        }
        damageColor(amount);
    }

    //SetStatusEffect sets the effect and clears it after 5 seconds
    void SetStatusEffect(DamageEngine.ElementType effect)
    {
        //if you already have that status effect, go back
        if (statusDictionary[effect] == true)
            return;

        //otherwise
        statusDictionary[effect] = true;
        //Debug.Log(effect + " = true");
        StartCoroutine(EffectDuration(effect, 5));
    }
    IEnumerator EffectDuration(DamageEngine.ElementType effect, int duration)
    {
        yield return new WaitForSeconds(duration);
        ClearStatusEffect(effect);
    }

    void ClearStatusEffect(DamageEngine.ElementType effect)
    {
        statusDictionary[effect] = false;
        Debug.Log(effect + " = false");
    }

    public void ClearALLStatusEffects()
    {
        for (int i = 0; i <= 6; i++)
        {
            statusDictionary[(DamageEngine.ElementType)i] = false;
        }
        burnTick = 5;
    }
    public void ResetAllStatuses()
    {
        StopAllCoroutines();
        ClearALLStatusEffects();
        //reset variables now
        HP = HPMax;
        player.SetBaseSpeed(player.GetOriginalSpeed());
        player.SetSprintMod(sprintModCache);
    }

    private IEnumerator flashColor(Color color)
    {
        modelColor.material.color = color;
        yield return new WaitForSeconds(0.1f);
        modelColor.material.color = colorOriginal;
    }

    IEnumerator FireBurn()
    {

        yield return new WaitForSeconds(1);

        if (statusDictionary[DamageEngine.ElementType.Water] == false && statusDictionary[DamageEngine.ElementType.fire] == true)
        {
            burnTick++;
            DamageEngine.instance.CalculateDamage(mObjectCollider, 1, DamageEngine.ElementType.fire);
            Instantiate(particles.burnParticle, mObjectCollider.transform);
        }
    }

    IEnumerator SlowTarget(NavMeshAgent target)
    {
        target.speed /= 10;
        yield return new WaitForSeconds(5);
        target.speed = speedCache;
    }

    IEnumerator SlowPlayer()
    {
        player.SetSprintMod(1);
        player.SetBaseSpeed(player.GetBaseSpeed() / 10);
        yield return new WaitForSeconds(5);
        player.SetBaseSpeed(player.GetOriginalSpeed());
        if (Input.GetButton("Sprint"))
        {
            player.SetBaseSpeed(player.GetBaseSpeed() * 2);
        }
        player.SetSprintMod(sprintModCache);
    }




    private void damageColor(int amount)
    {
        if (!isPlayer)
        {
            //Debug.Log(amount);
            if (amount == 0)
            {
                StartCoroutine(flashColor(Color.grey));
                //I'm commenting this out because it's obnoxious
                //Instantiate(particles.blockParticle, mObjectCollider.transform);
            }
            else if (amount > 0)
            {
                StartCoroutine(flashColor(Color.red));
            }
            else
            {
                StartCoroutine(flashColor(Color.green));
                Instantiate(particles.healParticle, mObjectCollider.transform);
            }

            if (textHP != null)
            {
                textHP.text = HP.ToString("F0");
            }

            //TODO: Particles
        }
        else
        {
            if (amount == 0)
            {
                //Color transparentGrey = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                //gameManager.instance.DamageFlashScreen(transparentGrey);
                //I'm commenting this out because it's obnoxious
                //Instantiate(particles.blockParticle, mObjectCollider.transform);
            }
            else if (amount > 0)
            {
                Color transparentRed = new Color(1, 0, 0, 0.2f);
                gameManager.instance.DamageFlashScreen(transparentRed);
                Instantiate(particles.damageParticle, mObjectCollider.transform);
            }
            else
            {
                Color transparentGreen = new Color(0, 1, 0, 0.2f);
                gameManager.instance.DamageFlashScreen(transparentGreen);
                Instantiate(particles.healParticle, mObjectCollider.transform);
            }
        }
    }
}