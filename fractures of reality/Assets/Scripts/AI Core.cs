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
    [SerializeField] Vector3 floatingAroundPlayerDistance = Vector3.zero;
    public bool isAttacking;
    public float stoppingDistanceOriginal;
    [SerializeField]LayerMask enemyLayerMask;


    [Header(" --- Attacks --- ")]
    //[SerializeField] public bool isMelee;
    [SerializeField] public List<GameObject> spellsList = new List<GameObject>();
    [SerializeField] public Transform shootPos;
    [SerializeField] public float attackRate;
    [SerializeField] bool SpellTracking;
    [SerializeField] float TrackingStrength;

    [Header(" --- Roaming --- ")]
    [SerializeField] public bool canRoam;
    public bool isRoaming;
    [SerializeField] public int roamDistance = 15;
    [Range(6,25)][SerializeField] public int roamTimer = 15;

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
        //Debug.Log("CastRandomSpell");
        animator.SetTrigger("CastRandomSpell");
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    public void CastRandomSpell()
    {
        AttackCore _currentSpell;
        //get random spell from list
        int thisSpell = Random.Range(0, spellsList.Count);
        //cast the random spell
        _currentSpell= Instantiate(spellsList[thisSpell], shootPos.position, shootPos.rotation).GetComponent<AttackCore>();
        if(SpellTracking) 
           _currentSpell.SpellTrackingStrength = TrackingStrength;
    }

    public bool CanSeePlayer()
    {
        //know where the player is at all times
        playerDirection = gameManager.instance.player.transform.position - headPos.position;
        Debug.DrawRay(headPos.position, playerDirection, Color.red);
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        RaycastHit hit;
        //if there is nothing in the way
        if (Physics.Raycast(headPos.position, playerDirection, out hit, Mathf.Infinity,enemyLayerMask))
        {
            //if the player is within your sightlines
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                return true;
            }
        }
        //enemy can't see us, 
        return false;
    }

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
        Vector3 playerDirY = playerDirection;
        playerDirY.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerDirY);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * facePlayerSpeed);
      
    }

    public void SmoothAnimations()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agentSpeed, Time.deltaTime * animationSpeedTransition));
    }

    /// <summary>
    /// Returns a Vector3 that is somewhere random inbetween the original stopping distance*2 and *3
    /// </summary>
    /// <returns></returns>
    public Vector3 FloatAroundBounds()
    {
        floatingAroundPlayerDistance += new Vector3(
            (stoppingDistanceOriginal + (Random.Range(stoppingDistanceOriginal, stoppingDistanceOriginal * 2))),
            0,
            (stoppingDistanceOriginal + (Random.Range(stoppingDistanceOriginal, stoppingDistanceOriginal * 2)))
            );
        return floatingAroundPlayerDistance;
    }

    public void SetDestinationToPlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        agent.stoppingDistance = stoppingDistanceOriginal;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
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
            agent.stoppingDistance = stoppingDistanceOriginal;
        }

    }
}
