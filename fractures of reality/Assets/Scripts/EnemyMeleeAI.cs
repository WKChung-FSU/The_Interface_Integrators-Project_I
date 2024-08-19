using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] Renderer model;
    [SerializeField] float attackRate;
    [SerializeField] DamageEngine.ElementType mDamage;
    [SerializeField] int meleeDamage;
    UnityEngine.Collider attackTarget;
    bool isAttacking;
    bool playerInRange;
    bool giveDam;

    void Start()
    {
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
            
        }
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

