using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI1 : MonoBehaviour,IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [SerializeField] int Hp;
    [SerializeField] DamageEngine.EnemyType enemyType;
    [SerializeField] GameObject meleeAttack;
    [SerializeField] float attackRate;

    bool isAttacking;
    bool playerInRange;
    Color colorOriginal;

    public int EnemyHP
    {
        get
        {
            return Hp;
        }

        set
        {
            Hp = value;
        }
    }

    public DamageEngine.EnemyType EnemyType
    {
        get
        {
            return enemyType;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        colorOriginal = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
             if (!isAttacking) {
                 StartCoroutine(MeleeAttack());
             }

            if (Hp <= Hp / 2)
            {
                attackRate = attackRate * 1.5f;
            }
        }
    }

    public void takeDamage(int amount, DamageEngine.damageType type)
    {
        Hp -= amount;

        if (Hp < 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        Instantiate(meleeAttack);
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
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

