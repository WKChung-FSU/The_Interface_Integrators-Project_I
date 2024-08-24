using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICoreWizard : MonoBehaviour
{
    // ====================================================== //
    //  This script is for everything the wizard CAN do,
    //  it governs how the wizard should behave
    //                                              -Chris
    // ====================================================== //

    AICore aiCore;
    [SerializeField] GameObject minion;
    [SerializeField] List<Transform> summonPositions = new List<Transform>();
    [Range(0, 3)][SerializeField] int summonCountAllowance = 3;
    [Range(5, 30)][SerializeField] int summonRefreshTimer = 20;
    int summonCount;
    bool refreshingSummonCount;



    // Start is called before the first frame update
    void Start()
    {
        aiCore = transform.GetComponent<AICore>();
    }

    // Update is called once per frame
    void Update()
    {
        aiCore.SmoothAnimations();
        //if the player is in range and you cannot see the player
        if (aiCore.playerInRange && !CanSeePlayer())
        {
            //if you can roam
            if (aiCore.canRoam && !aiCore.isRoaming && aiCore.agent.remainingDistance < 0.05f)
            {
                StartCoroutine(aiCore.Roam());
            }
            //if the player is not in range
        }
        else if (!aiCore.playerInRange)
        {
            aiCore.agent.SetDestination(aiCore.startingPosition);
        }
        if (summonCount == summonCountAllowance && refreshingSummonCount == false)
        {
            StartCoroutine(RefreshSummonCount());
        }
    }

    public bool CanSeePlayer()
    {
        aiCore.playerDirection = gameManager.instance.player.transform.position - aiCore.headPos.position;
        Debug.DrawRay(aiCore.headPos.position, aiCore.playerDirection, Color.red);
        aiCore.angleToPlayer = Vector3.Angle(aiCore.playerDirection, transform.forward);

        //Debug.Log(angleToPlayer);

        RaycastHit hit;
        //If you can see the player
        if (Physics.Raycast(aiCore.headPos.position, aiCore.playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && aiCore.angleToPlayer <= aiCore.viewAngle) //is this right?
            {
                //Do ya summons
                if (summonCount < summonCountAllowance - 1)
                {
                    aiCore.isAttacking = true;
                    StartCoroutine(SummonMinions());
                    aiCore.isAttacking = false;
                }
                //otherwise
                else
                {
                    //approach the player
                    aiCore.agent.SetDestination(gameManager.instance.player.transform.position);
                    if (aiCore.agent.remainingDistance <= aiCore.agent.stoppingDistance)
                    {
                        aiCore.FacePlayer();
                        if (!aiCore.isAttacking && aiCore.angleToPlayer <= aiCore.castAngle)
                        {
                            StartCoroutine(aiCore.shoot());
                        }
                    }

                    aiCore.agent.stoppingDistance = aiCore.stoppingDistanceOriginal;
                }
                return true;
            }
        }
        //enemy can't see us, 
        //aiCore.agent.stoppingDistance = 0;
        return false;
    }

    IEnumerator SummonMinions()
    {
        // aiCore.isAttacking = true;
        aiCore.animator.SetTrigger("CastSkele");   //Use the animator to call the summonEnemy code
        yield return new WaitForSeconds(aiCore.attackRate);
        //aiCore.isAttacking = false;
    }

    public void summonEnemy()
    {
        //Whatever enemy is set as the game object will be summoned
        Instantiate(minion, summonPositions[Random.Range(0, summonPositions.Count)].position, transform.rotation);
        summonCount++;
    }

    IEnumerator RefreshSummonCount()
    {
        refreshingSummonCount = true;
        yield return new WaitForSeconds(summonRefreshTimer);
        Debug.Log("Summon count refreshed!");
        summonCount = 0;
        refreshingSummonCount= false;
    }
}
