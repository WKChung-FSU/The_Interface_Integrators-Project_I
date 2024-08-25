using System.Collections;
using System.Collections.Generic;
using System.Net;
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

        if (isPlayer == false && textHP != null)
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
                    if (statusDictionary[DamageEngine.ElementType.Water] == true)
                    {
                        ClearStatusEffect(DamageEngine.ElementType.Water);
                        break;
                    }
                    Instantiate(particles.burnParticle, mObjectCollider.transform);

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
                    //if you are wet, deal big damage

                    //also check those around you
                    //  if they are wet and not lightning
                    //      cast the line renderer from you to the others
                    //          make them lightning
                    //          repeat
                    //      
                    break;
                }
            case DamageEngine.ElementType.Ice:
                {
                    //if you are wet
                    //  freeze
                    //or if you are fire
                    //  extinguish and you are now wet
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
                    SetStatusEffect(DamageEngine.ElementType.Water);
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
        Debug.Log(effect + " = true");
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

        if (statusDictionary[DamageEngine.ElementType.Water] == false)
        {
            burnTick++;
            DamageEngine.instance.CalculateDamage(mObjectCollider, 1, DamageEngine.ElementType.fire);
            Instantiate(particles.burnParticle, mObjectCollider.transform);
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

            if(textHP != null)
            { 
                textHP.text = HP.ToString("F0");
            }

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
}