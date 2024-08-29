using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimescript : MonoBehaviour
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

        if (aiCore.playerInRange && aiCore.CanSeePlayer() == true)
        {
            aiCore.isRoaming = false;
            aiCore.agent.SetDestination(gameManager.instance.player.transform.position);
            aiCore.agent.stoppingDistance = aiCore.stoppingDistanceOriginal;

            aiCore.FacePlayer();

            if (aiCore.agent.remainingDistance <= aiCore.stoppingDistanceOriginal)
            {
                if (aiCore.isAttacking == false)
                {
                    StartCoroutine(aiCore.shoot());
                }
            }
        }
        else
        {
            if (aiCore.canRoam == true && aiCore.agent.remainingDistance < 0.05f && aiCore.isRoaming == false)
            {
                aiCore.roamTimer = Random.Range(aiCore.roamTimer - 5, aiCore.roamTimer + 5);
                StartCoroutine(aiCore.Roam());
            }
            else if (aiCore.canRoam == true && aiCore.isRoaming == false)
            {
                StartCoroutine(aiCore.Roam());
            }
        }
    }
}
