using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{
    [SerializeField] GameObject thisEnemy;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int animationSpeedTransition = 80;
    //[SerializeField] Renderer model;
    [SerializeField] float attackRate;
    [SerializeField] DamageEngine.ElementType mDamage;
    [SerializeField] int meleeDamage;
    UnityEngine.Collider attackTarget;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject spell;


    [SerializeField] bool isMelee;
    [SerializeField] bool canRoam = true;

    bool isAttacking;
    bool playerInRange;

    Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animationSpeedTransition));

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
