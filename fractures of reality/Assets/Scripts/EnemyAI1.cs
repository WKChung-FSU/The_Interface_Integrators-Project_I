using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI1 : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] int Hp;
    [SerializeField] DamageEngine.EnemyType enemyType;
    [SerializeField] float attackRate;
    [SerializeField] DamageEngine.damageType mDamage;
    [SerializeField] int meleeDamage;
    UnityEngine.Collider attackTarget;

    bool isAttacking;
    bool playerInRange;
    bool giveDam;
    Color colorOriginal;

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

    public DamageEngine.EnemyType EnemyType
    {
        get
        {
            return enemyType;
        }
    }

    void Start()
    {
        colorOriginal = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isAttacking && playerInRange)
            {
                StartCoroutine(MeleeAttack());
            }

            if (Hp <= Hp / 2)
            {
                attackRate = attackRate * 1.5f;
            }
        }
    }

    public void takeDamage(int amount, DamageEngine.damageType type)
    {
        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        DamageEngine.instance.CalculateDamage(attackTarget, meleeDamage, mDamage);
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            attackTarget = other;
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            attackTarget = null;
            playerInRange = false;
        }

    }
}

