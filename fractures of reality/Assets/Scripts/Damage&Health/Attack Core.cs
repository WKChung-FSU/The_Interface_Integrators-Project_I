using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackCore : MonoBehaviour
{
    [Header("-----MainAttributes-----")]
    [SerializeField] DamageEngine.ElementType attackElement;
    [SerializeField] DamageEngine.movementType movementType;
    [SerializeField] Rigidbody rb;
    [Range(0, 10)][SerializeField] int damageAmount;
    [Header("if 0 then it will not deSpawn")]
    [Range(0, 30)][SerializeField] int RemoveTime=0;

    [Header("-----Spell Attributes-----")]
    [Range(1, 30)][SerializeField] int speed;
   

    [Header("HitScan info(Currently player only)")]
    [Range(1, 100)][SerializeField] int SpellRange;

    [Header("-----Environmental Attributes-----")]
    [Header("will not attack if 0")]
    [Range(0, 1)][SerializeField] float AttackSpeed = 1;
 


    [Header("-----Aoe Components-----")]
    [SerializeField] GameObject AoeObject;
    
    public List<Collider> targets=new List<Collider>();
    bool Attacking;
    // Start is called before the first frame update
    void Start()
    {
        if (movementType == DamageEngine.movementType.Spell || movementType == DamageEngine.movementType.AoeSpell)
        {
            rb.velocity = transform.forward * speed;
            if (RemoveTime != 0)
            {
                Destroy(this.GetComponent<TrailRenderer>(), RemoveTime);
                Destroy(gameObject, RemoveTime);
            }
        }

        if (movementType == DamageEngine.movementType.Environmental)
        {
            if (RemoveTime != 0){ 
            Destroy(gameObject, RemoveTime);
             }
        }
    }

    private void Update()
    {
        if (!Attacking&&AttackSpeed!=0)
        {
            StartCoroutine(environmentalAttack());
        }
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger ||other == gameManager.instance.player.GetComponent<CapsuleCollider>())
        {
            return;
        }
        //do damage
        if (movementType == DamageEngine.movementType.Environmental)
        {
            IDamage dmg = other.GetComponent<IDamage>();
            if (dmg != null)
            {
                targets.Add(other);
            }
        }
        else
        {
            DamageEngine.instance.CalculateDamage(other, damageAmount, attackElement);
        }
        
        if(movementType == DamageEngine.movementType.AoeSpell)
        {
            if (AoeObject != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (transform.up*-1), out hit))
                {
                    Debug.DrawRay(transform.position, (transform.up * -1), Color.red);
                    #region Debug
                    Debug.Log(hit.collider.name);
                    #endregion
                  
                    Instantiate(AoeObject, hit.point, hit.transform.rotation);
                }

            }
            else
            {
                Debug.Log("Error in Aoe Spell");
            }

        }

        if (movementType == DamageEngine.movementType.Spell|| movementType==DamageEngine.movementType.AoeSpell)
        {
            if (this.GetComponent<TrailRenderer>()!=null) {
                this.GetComponent<TrailRenderer>().enabled = false;
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (movementType == DamageEngine.movementType.Environmental)
        {
            targets.Remove(other);
        }

            return;
    }
    IEnumerator environmentalAttack()
    {
        Attacking = true;
        for (int Target = 0; Target < targets.Count; Target++)
        {
            DamageEngine.instance.CalculateDamage(targets[Target], damageAmount, attackElement,targets);
        }
        yield return new WaitForSeconds(AttackSpeed);
        Attacking = false;
    }

    //currently player only
   public RaycastHit CastHitScanAttack(LayerMask ignoreMask)
    {

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, SpellRange, ~ignoreMask))
        {
            #region Debug
            //Debug.Log(hit.collider.name);
            #endregion    
            IDamage damage = hit.collider.GetComponent<IDamage>();
            if (damage != null)
            {
                DamageEngine.instance.CalculateDamage(hit.collider, damageAmount, DamageEngine.ElementType.Lightning); 
            }
        }
       return hit;
    }
}
