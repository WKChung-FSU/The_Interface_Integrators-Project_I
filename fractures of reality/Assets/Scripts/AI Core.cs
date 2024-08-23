using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICore : MonoBehaviour
{
    [Header(" --- Essential --- ")]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] public int animationSpeedTransition = 5;
    public Vector3 startingPosition;
    public Vector3 playerDirection;
    public float angleToPlayer;
    [SerializeField] public Transform headPos;
    [SerializeField] public int viewAngle = 90;
    [SerializeField] public int castAngle = 45;
    [SerializeField] public int facePlayerSpeed = 10;
    public bool playerInRange;
    public bool isAttacking;
    public float stoppingDistanceOriginal;



    [Header(" --- Attacks --- ")]
    [SerializeField] public bool isMelee;
    [SerializeField] public List<GameObject> spellsList = new List<GameObject>();
    [SerializeField] public Transform shootPos;
    [SerializeField] public float attackRate;

    [Header(" --- Roaming --- ")]
    [SerializeField] public bool canRoam;
    public bool isRoaming;
    [SerializeField] public int roamDistance = 15;
    [SerializeField] public int roamTimer = 15;

    void Start()
    {
        agent = transform.GetComponent<NavMeshAgent>();
        animator = transform.GetComponent<Animator>();
        startingPosition = transform.position;
        agent.stoppingDistance = stoppingDistanceOriginal;
        startingPosition = transform.position;
    }
    public IEnumerator shoot()
    //this is the function that actually creates the spell
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackRate);
        animator.SetTrigger("CastRandomSpell");
        isAttacking = false;
    }

    public void CastRandomSpell()
    {
        //get random spell from list
        int thisSpell = Random.Range(0, spellsList.Count);
        //cast the random spell
        Instantiate(spellsList[thisSpell], shootPos.position, shootPos.rotation);
    }

    //public bool CanSeePlayer()
    //{
    //    playerDirection = gameManager.instance.player.transform.position - headPos.position;
    //    angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

    //    //Debug.Log(angleToPlayer);
    //    Debug.DrawRay(headPos.position, playerDirection, Color.red);

    //    RaycastHit hit;
    //    if (Physics.Raycast(headPos.position, playerDirection, out hit))
    //    {
    //        if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle) //is this right?
    //        {
    //            agent.SetDestination(gameManager.instance.player.transform.position);
    //            if (agent.remainingDistance <= agent.stoppingDistance)
    //            {
    //                FacePlayer();
    //            }
    //            if (!isAttacking && angleToPlayer <= castAngle && !isMelee)
    //            {
    //                StartCoroutine(shoot());
    //            }
    //            agent.stoppingDistance = stoppingDistanceOriginal;
    //            return true;
    //        }
    //    }
    //    //enemy can't see us, 
    //    agent.stoppingDistance = 0;
    //    return false;
    //}

    public IEnumerator Roam()
    {
        if (canRoam)
        {
            isRoaming = true;
            yield return new WaitForSeconds(roamTimer);

            agent.stoppingDistance = 0;
            //random roam distance
            Vector3 randomRoamingDistance = Random.insideUnitSphere * roamDistance;

            //tie it to starting position
            randomRoamingDistance += startingPosition;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomRoamingDistance, out hit, roamDistance, 1); //1 = default layer
            agent.SetDestination(hit.position);

            isRoaming = false;
        }
    }

    public void FacePlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * facePlayerSpeed);
    }

    public void SmoothAnimations()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animationSpeedTransition));
    }
    //void Update()
    //{
    //    float agentSpeed = agent.velocity.normalized.magnitude;
    //    animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animationSpeedTransition));

    //    if (playerInRange)
    //    {
    //        agent.SetDestination(gameManager.instance.player.transform.position);
    //        if (!isAttacking && playerInRange)
    //        {
    //            if (isMelee)
    //            {
    //                StartCoroutine(MeleeAttack());
    //            }
    //            else
    //            {
    //                StartCoroutine(shoot());
    //            }
    //        }

    //    }
    //}
    //IEnumerator MeleeAttack()
    //{
    //    isAttacking = true;
    //    DamageEngine.instance.CalculateDamage(attackTarget, meleeDamage, mDamage);
    //    yield return new WaitForSeconds(attackRate);
    //    isAttacking = false;
    //}


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            // attackTarget = other;
            playerInRange = true;
            agent.stoppingDistance = stoppingDistanceOriginal;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            //attackTarget = null;
            playerInRange = false;
        }

    }
}
