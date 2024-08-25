using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleHealthCore : MonoBehaviour, IDamage
{

    public enum statusEffect { normal, fireBurning, LightningShocked, IceFrozen, Earth, Windborn, WaterWet }
    [Range(0, 500)][SerializeField] int Hp = 100;
    [SerializeField] Collider mObjectCollider;
    [SerializeField] DamageEngine.ElementType elementType;
    [SerializeField] public bool IsMandatory = true;
    [SerializeField] bool isPlayer = false;
    [SerializeField] int effectTickDelay = 1;
    [SerializeField] Renderer modelColor;
    [SerializeField] TMP_Text textHP;
    [SerializeField] DamageParticlesList particles;

    public Dictionary<DamageEngine.ElementType, bool> statusDictionary = new Dictionary<DamageEngine.ElementType, bool>();
    int MaxHealth;
    Color colorOriginal;

    private int burnTick = 0;
    private int tickDamage = 0;

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

        if (!isPlayer)
        {
            textHP.text = HP.ToString("F0");
        }

        //add each type to the status effect dictionary and set it false
        for (int i = 0; i <= 6; i++)
        {
            statusDictionary.Add((DamageEngine.ElementType)i, false);
        }

        //particles = GameObject.FindAnyObjectByType<DamageParticlesList>();
    }
    public DamageEngine.ElementType ElementType
    {
        get
        {
            return elementType;
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
                    //if you are wet
                    if (burnTick >= 5 || statusDictionary[DamageEngine.ElementType.Water] == true)
                    {
                        burnTick = 0;
                        ClearStatusEffect(DamageEngine.ElementType.Water);
                        //StopCoroutine(EffectTickDelay(DamageEngine.ElementType.Water, tickDamage));
                        break;
                    }
                    SetStatusEffect(DamageEngine.ElementType.fire);
                    if (amount > 0)
                    {
                        Instantiate(particles.burnParticle, mObjectCollider.transform);
                    }
                    tickDamage = 1;
                    StartCoroutine(EffectTickDelay(DamageEngine.ElementType.fire, tickDamage));
                    burnTick++;
                    break;
                }
            case DamageEngine.ElementType.Lightning:
                {

                    break;
                }
            case DamageEngine.ElementType.Ice:
                {

                    break;
                }
            case DamageEngine.ElementType.Earth:
                {

                    break;
                }
            case DamageEngine.ElementType.Wind:
                {

                    break;
                }
            case DamageEngine.ElementType.Water:
                {
                    //if you are on fire
                    if (burnTick >= 5 || statusDictionary[DamageEngine.ElementType.fire] == true)
                    {
                        burnTick = 0;
                        ClearStatusEffect(DamageEngine.ElementType.fire);
                        //ClearStatusEffect(DamageEngine.ElementType.Water);
                        //StopCoroutine(EffectTickDelay(DamageEngine.ElementType.fire, tickDamage));
                        break;
                    }
                    //otherwise
                    SetStatusEffect(DamageEngine.ElementType.Water);
                    tickDamage = 0;
                    StartCoroutine(EffectTickDelay(DamageEngine.ElementType.Water, tickDamage));
                    burnTick++;
                    break;
                }

        }
        damageColor(amount);
    }

    void SetStatusEffect(DamageEngine.ElementType effect)
    {
        statusDictionary[effect] = true;
        Debug.Log(effect + " = true");
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
    }

    private void damageColor(int amount)
    {
        if (!isPlayer)
        {
            //Debug.Log(amount);
            if (amount == 0)
            {
                StartCoroutine(flashColor(Color.grey));
                Instantiate(particles.blockParticle, mObjectCollider.transform);
            }
            else if (amount > 0)
            {
                StartCoroutine(flashColor(Color.red));
                Instantiate(particles.damageParticle, mObjectCollider.transform);
            }
            else
            {
                StartCoroutine(flashColor(Color.green));
                Instantiate(particles.healParticle, mObjectCollider.transform);
            }

            textHP.text = HP.ToString("F0");

            //TODO: Particles
        }
        else
        {
            if (amount == 0)
            {
                Color transparentGrey = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                gameManager.instance.DamageFlashScreen(transparentGrey);
                Instantiate(particles.blockParticle, mObjectCollider.transform);
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

    private IEnumerator flashColor(Color color)
    {
        modelColor.material.color = color;
        yield return new WaitForSeconds(0.1f);
        modelColor.material.color = colorOriginal;
    }

    private IEnumerator EffectTickDelay(DamageEngine.ElementType type, int damage)
    {
        yield return new WaitForSeconds(effectTickDelay);
        DamageEngine.instance.CalculateDamage(mObjectCollider, damage, type);
    }
}

//NOTE TO SELF =====================================================================================================
//I think there is a coroutine issue with timings here
//POTENTIAL FIX
//take the calculate damage call out of the coroutine and place it after the coroutine,
//now in between the coroutine and calculate damage call put an if statement that checks
//if you should still continue with the code or not