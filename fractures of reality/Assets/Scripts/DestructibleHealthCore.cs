using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleHealthCore : MonoBehaviour, IDamage
{

    public enum statusEffect { Burning, Wet, Electrocuted, FrostBitten, WindBorn, Stunned }
    [SerializeField] int Hp = 20;
    [SerializeField] Renderer model;
    [SerializeField] DamageEngine.ElementType elementType;
    [SerializeField] public bool IsMandatory = true;
    [SerializeField] bool isPlayer=false;

    Dictionary<statusEffect, bool> statusEffects;
    int MaxHealth;
    Color colorOriginal;
    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = Hp;
        colorOriginal = model.material.color;
       // if (isPlayer)
          //  gameManager.instance.healthMax = MaxHealth;
         if (IsMandatory)
            gameManager.instance.updateGameGoal(1);
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
    public void damageEffect(int amount, DamageEngine.ElementType type)
    {
        if (!isPlayer)
        {
                Debug.Log(amount);
            if (amount == 0)
                StartCoroutine(flashColor(Color.grey));
            else if (amount > 0)
                StartCoroutine(flashColor(Color.red));
            else
                StartCoroutine(flashColor(Color.green));
        }
    }

    IEnumerator flashColor(Color color)
    {
        model.material.color = color;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}
