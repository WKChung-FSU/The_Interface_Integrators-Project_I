using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICoreBeholder : MonoBehaviour
{
    AICore aiCore;

    // Start is called before the first frame update
    void Start()
    {
        aiCore = GetComponent<AICore>();

    }

    // Update is called once per frame
    void Update()
    {
        aiCore.SmoothAnimations();

        //if the player is in range and you can see the player
        if (aiCore.playerInRange && aiCore.CanSeePlayer() == true)
        {
            aiCore.isRoaming = false;
            Debug.Log("I can see you");
            //Your new destination is the players location
            aiCore.agent.SetDestination(gameManager.instance.player.transform.position);
            aiCore.agent.stoppingDistance = aiCore.stoppingDistanceOriginal;

            aiCore.FacePlayer();

            //if you are at the player
            if (aiCore.agent.remainingDistance <= aiCore.stoppingDistanceOriginal)
            {
                //if you are currently not attacking 
                if (aiCore.isAttacking == false)
                {
                    //do a melee attack
                    StartCoroutine(aiCore.shoot());
                }
            }
        }
        //if you cannot see the player and they're not in range
        else
        {
            //if you are at your destination and you can roam and you are not already
            if (aiCore.canRoam == true && aiCore.agent.remainingDistance < 0.05f && aiCore.isRoaming == false)
            {
                aiCore.roamTimer = Random.Range(aiCore.roamTimer - 5, aiCore.roamTimer + 5);
                StartCoroutine(aiCore.Roam());
                //aiCore.agent.stoppingDistance = aiCore.stoppingDistanceOriginal;
            }
        }
    }
}
