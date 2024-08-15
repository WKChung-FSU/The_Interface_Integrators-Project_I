using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour,IDamage
{
    [SerializeField] GameObject thisEnemy;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    DestructibleHealthCore health;

    [SerializeField] int Hp;
    [SerializeField] DamageEngine.EnemyType enemyType;
    [SerializeField] GameObject spell;
    [SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;
    Color colorOriginal;

    void Start()
    {
        health = thisEnemy.GetComponent<DestructibleHealthCore>();
    }


    public DamageEngine.EnemyType EnemyType
    {
        get
        {
            return enemyType;
        }
    }

    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
             if (!isShooting) {
                 StartCoroutine(shoot());
             }
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(spell, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }


}

