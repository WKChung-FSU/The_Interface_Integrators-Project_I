using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleHealthCore : MonoBehaviour, IDamage
{
    [SerializeField] int Hp;
    [SerializeField] Renderer model;
    [SerializeField] DamageEngine.ElementType enemyType;
    int MaxHealth;
    Color colorOriginal;
    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = Hp;
        colorOriginal = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }
    public DamageEngine.ElementType EnemyType
    {
        get
        {
            return enemyType;
        }
    }

    public int EnemyHP
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
    public void takeDamage(int amount, DamageEngine.ElementType type)
    {
        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}
