using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardAI : MonoBehaviour
{   [SerializeField] GameObject thisEnemy;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform spawnPoint;
    DestructibleHealthCore health;

    [SerializeField] GameObject spell;
    [SerializeField] GameObject enemyMinion;
    [SerializeField] float shootRate;

    int numOfSummons;

    bool isShooting;
    bool playerInRange;

    
    void Start()
    {
        health = thisEnemy.GetComponent<DestructibleHealthCore>();
    }

    void Update()
    {
        if(playerInRange)
        {
            
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
        }
        //as this is the boss for this mini level we will increase the shoot rate
        if(!playerInRange)
        {
            agent.SetDestination(spawnPoint.position);
        }
        
    }

    IEnumerator shoot()
    {
        isShooting = true;
        //Will summon 3 enemies to fight for him before shooting
        if (numOfSummons > 2)
        {
            //the spell type will be shot at the player
            Instantiate(spell, shootPos.position, transform.rotation);

            yield return new WaitForSeconds(shootRate);
        }
        else
        {
            //counting the number of summons
            numOfSummons++;
            summonEnemy();
            //takes longer to summon enemies
            yield return new WaitForSeconds(shootRate * 2);
        }

        isShooting = false;
    }

    public void summonEnemy()
    {
        Transform summonPos = shootPos;
        summonPos.position.Set(
          summonPos.position.x + 1,summonPos.position.y, summonPos.position.z + 1);
        //Whatever enemy is set as the game object will be summoned
        Instantiate(enemyMinion, summonPos.position, transform.rotation);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            playerInRange = false; 
        }
    }
}
