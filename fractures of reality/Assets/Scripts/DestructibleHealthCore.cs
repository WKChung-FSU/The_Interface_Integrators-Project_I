using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleHealthCore : MonoBehaviour, IDamage
{

    public enum statusEffect { normal, fireBurning, LightningShocked, IceFrozen, Earth, Windborn, WaterWet }
    [SerializeField] int Hp = 20;
    [SerializeField] Collider mObjectCollider;
    [SerializeField] DamageEngine.ElementType elementType;
    [SerializeField] public bool IsMandatory = true;
    [SerializeField] bool isPlayer = false;
    [SerializeField] int burnTickDelay = 1;
    [SerializeField] Renderer modelColor;
    [SerializeField] TMP_Text textHP;

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

        if(!isPlayer)
        {
            textHP.text = HP.ToString("F0");
        }
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
        if (statusDictionary.ContainsKey(type))
        {
            if (statusDictionary[type] == false)
            {
                statusDictionary[type] = true;
            }
        }
        else
        {
            statusDictionary.Add(type, true);
        }

        // if stacking multiple different effects becomes an issue, put the following switch in an else statement

        switch (type)
        {
            case DamageEngine.ElementType.Normal:
                {

                    break;
                }
            case DamageEngine.ElementType.fire:
                {
                    if (burnTick >= 5)
                    {
                        burnTick = 0;
                        ClearStatusEffect(DamageEngine.ElementType.fire);
                        return;
                    }
                    StartCoroutine(BurnDelay(amount));
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

                    break;
                }

        }
        damageColor(amount);
    }

    void ClearStatusEffect(DamageEngine.ElementType effect)
    {
        statusDictionary[effect] = false;
    }

    private void damageColor(int amount)
    {
        if (!isPlayer)
        {
            //Debug.Log(amount);
            if (amount == 0)
                StartCoroutine(flashColor(Color.grey));
            else if (amount > 0)
                StartCoroutine(flashColor(Color.red));
            else
            {
                StartCoroutine(flashColor(Color.green));
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
            }
            else if (amount > 0)
            {
                Color transparentRed = new Color(1, 0, 0, 0.2f);
                gameManager.instance.DamageFlashScreen(transparentRed);
            }
            else
            {
                Color transparentGreen = new Color(0,1,0,0.2f);
                gameManager.instance.DamageFlashScreen(transparentGreen);
            }
        }
    }

    private IEnumerator flashColor(Color color)
    {
        modelColor.material.color = color;
        yield return new WaitForSeconds(0.1f);
        modelColor.material.color = colorOriginal;
    }

    private IEnumerator BurnDelay(int amount)
    {
        yield return new WaitForSeconds(burnTickDelay);
        DamageEngine.instance.CalculateDamage(mObjectCollider, 1, DamageEngine.ElementType.fire);

    }
}
