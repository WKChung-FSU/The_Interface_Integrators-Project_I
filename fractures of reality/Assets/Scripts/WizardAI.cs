using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int animationSpeedTransition;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform summonPos;

    //values for raycast to see player and updated AI
    [SerializeField] Transform headPos;
    [SerializeField] int viewAngle;
    float angleToPlayer;
    Vector3 playerDir;
    [SerializeField] int facePlayerSpeed;

    [SerializeField] GameObject spell;
    [SerializeField] GameObject enemyMinion;
    [SerializeField] float shootRate;

    int numOfSummons;

    Vector3 startingPos;

    bool isShooting;
    bool playerInRange;


    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animationSpeedTransition));

        if (playerInRange && canSeePlayer())
        {

            agent.SetDestination(gameManager.instance.player.transform.position);
            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
        }
        //as this is the boss for this mini level we will increase the shoot rate
        if (!playerInRange)
        {
            agent.SetDestination(startingPos);
        }


    }
    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
                }
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                return true;
            }
        }
        return false;
    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    public IEnumerator shoot()
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
            animator.SetTrigger("CastSkele");   //Use the animator to call the summonEnemy code
            //takes longer to summon enemies
            yield return new WaitForSeconds(shootRate * 2);
        }

        isShooting = false;
    }

    public void summonEnemy()
    {
        //Whatever enemy is set as the game object will be summoned
        Instantiate(enemyMinion, summonPos.position, transform.rotation);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
