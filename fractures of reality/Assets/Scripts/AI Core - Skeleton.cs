using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AICoreSkeleton : MonoBehaviour
{
    AICore aiCore;

    [SerializeField] GameObject weapon;
    [SerializeField] bool canCastSpell;
    [SerializeField] int spellCastTimer;
    bool castTimerResetting;

    // Start is called before the first frame update
    void Start()
    {
        aiCore = GetComponent<AICore>();
        weapon.GetComponent<CapsuleCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        aiCore.SmoothAnimations();

        //if the player is in range and you can see the player
        if (aiCore.playerInRange && aiCore.CanSeePlayer() == true)
        {
            aiCore.isRoaming = false;
            //Debug.Log("I can see you");
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
                    StartCoroutine(SwingSword());
                }
            }
            //if you're not at the player, you can cast spells and your cast is not on cooldown,
            else if (canCastSpell == true && castTimerResetting == false)
            {
                StartCoroutine(aiCore.shoot());
                StartCoroutine(CastRecharge());
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
            else if (aiCore.canRoam == true && aiCore.isRoaming == false)
            {
                StartCoroutine(aiCore.Roam());
            }
        }
    }

    IEnumerator SwingSword()
    {
        aiCore.isAttacking = true;
        weapon.GetComponent<CapsuleCollider>().enabled = true;
        aiCore.animator.SetTrigger("MeleeAttack");
        StartCoroutine(DisableMelee());
        yield return new WaitForSeconds(aiCore.attackRate);


    }

    IEnumerator DisableMelee()
    {
        yield return new WaitForSeconds(aiCore.attackRate);
        weapon.GetComponent<CapsuleCollider>().enabled = false;
        aiCore.isAttacking = false;
    }

    IEnumerator CastRecharge()
    {
        castTimerResetting = true;
        yield return new WaitForSeconds(spellCastTimer);
        Debug.Log(this.name + " Mana refreshed!");
        castTimerResetting = false;
    }

    public void EnableMeleeHitbox()
    {
        weapon.GetComponent<CapsuleCollider>().enabled = true;
        aiCore.isAttacking = true;
    }

    public void DisableMeleeHitbox()
    {
        weapon.GetComponent<CapsuleCollider>().enabled = false;
        aiCore.isAttacking = false;
    }
}
