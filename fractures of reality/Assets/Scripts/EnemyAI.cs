using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour,IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;

    [SerializeField] int Hp;
    [SerializeField] DamageEngine.EnemyType enemyType;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool isShooting;
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
             if (!isShooting) {
                 StartCoroutine(shoot());
             }
        }
    }

    public void takeDamage(int amount, DamageEngine.damageType type)
    {
        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet,shootPos.position,transform.rotation);
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

