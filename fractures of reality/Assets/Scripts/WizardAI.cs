using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardAI : MonoBehaviour , IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;

    [SerializeField] int HP;
   

    [SerializeField] GameObject spell;
    [SerializeField] GameObject enemyMinion;
    [SerializeField] float shootRate;

    int numOfSummons;

    bool isShooting;
    bool playerInRange;

    Color colorOrig;
    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
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
        if(HP <= HP / 2)
        {
            shootRate = shootRate * 2;
        }
    }

    public void takeDamage(int amount, DamageEngine.damageType type)
    {
        HP -= amount;

        if(HP < 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.2f);
        model.material.color = colorOrig;
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
        //Whatever enemy is set as the game object will be summoned
        Instantiate(enemyMinion, shootPos.position, transform.rotation);
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
