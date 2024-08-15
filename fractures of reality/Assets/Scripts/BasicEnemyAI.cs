using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{
    [SerializeField] GameObject thisEnemy;
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] Renderer model;
    [SerializeField] float attackRate;
    [SerializeField] DamageEngine.damageType mDamage;
    [SerializeField] int meleeDamage;
    UnityEngine.Collider attackTarget;
    DestructibleHealthCore health;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject spell;


    [SerializeField] bool isMelee;

    bool isAttacking;
    bool playerInRange;
    void Start()
    {
        health = thisEnemy.GetComponent<DestructibleHealthCore>();
    }

    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isAttacking && playerInRange)
            {
                if (isMelee)
                {
                    StartCoroutine(MeleeAttack());
                }
                else
                {
                    StartCoroutine(shoot());
                }
            }

        }
    }
    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        DamageEngine.instance.CalculateDamage(attackTarget, meleeDamage, mDamage);
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    IEnumerator shoot()
    {
        isAttacking = true;
        Instantiate(spell, shootPos.position, transform.rotation);
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
