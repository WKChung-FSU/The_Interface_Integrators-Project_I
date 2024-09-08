using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class AttackCore : MonoBehaviour
{
    [Header("-----MainAttributes-----")]
    [SerializeField] DamageEngine.ElementType attackElement;
    [SerializeField] DamageEngine.movementType movementType;
    [Range(0, 10)][SerializeField] int damageAmount;
    [Header("if 0 then it will not deSpawn")]
    [Range(0, 30)][SerializeField] float RemoveTime=0;
    [SerializeField] GameObject destroyParticle;
    [Header("-----Spell Attributes-----")]
    [Range(1, 30)][SerializeField] int speed;
    [SerializeField] Rigidbody rb;

    [Header("-----Aoe Components-----")]
    [SerializeField] bool IsInfectious=false;
    [SerializeField] GameObject AoeObject = null;
    [SerializeField] int colliderRadius;
    [SerializeField] LayerMask TargetMask1;
    [SerializeField] LayerMask TargetMask2;
    [Header("element that you need to spread infectious Aoe,")]
    [Header("if Normal it will always spread")]
    [SerializeField] DamageEngine.ElementType RequiredElement;
    [Header("HitScan info(Currently player only)")]
    [Range(1, 100)][SerializeField] int SpellRange;

    [Header("-----Environmental Attributes-----")]
    [Header("will not attack if 0 / is also used for infection")]
    [Range(0, 1)][SerializeField] float AttackSpeed = 0.5f;


    int FinalTargetMask;
    public List<Collider> targets=new List<Collider>();
    bool Attacking;
    bool Searching;
    // Start is called before the first frame update
    void Start()
    {
        if (movementType == DamageEngine.movementType.Spell || movementType == DamageEngine.movementType.AoeInitialization)
        {
            gameManager.instance.playAudio(DamageEngine.instance.GetSpellSound(attackElement,false), DamageEngine.instance.GetSpellVolume(false));
            rb.velocity = transform.forward * speed;
            if (RemoveTime != 0)
            {
                Destroy(this.GetComponent<TrailRenderer>(), RemoveTime);
                Destroy(gameObject, RemoveTime);
            }
        }

        if(IsInfectious)
            DamageEngine.instance.UpdateAOEs(1);

        if (movementType == DamageEngine.movementType.Environmental)
        {
            if (RemoveTime != 0){ 
            Destroy(gameObject, RemoveTime);
             }
        }
        FinalTargetMask = TargetMask1 | TargetMask2;
    }
    private void OnDestroy()
    {
        if(IsInfectious)
        DamageEngine.instance.UpdateAOEs(-1);
    }


    private void Update()
    {
            if ((!Attacking && AttackSpeed != 0)&&movementType==DamageEngine.movementType.Environmental)
            {
                StartCoroutine(environmentalAttack());
            }

        if (IsInfectious)
        {
            Collider[] others=Physics.OverlapSphere(transform.position, colliderRadius,FinalTargetMask);

            foreach (Collider collider in others) 
            {
                DestructibleHealthCore otherHealth = collider.GetComponent<DestructibleHealthCore>();
                if (!targets.Contains(collider) && otherHealth != null)
                {
                 targets.Add(collider);
                }
            }
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
        
        if(movementType == DamageEngine.movementType.AoeInitialization)
        {
            if (AoeObject != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (transform.up*-1), out hit))
                {
                    Debug.DrawRay(transform.position, (transform.up * -1), Color.red);
                    #region Debug
                    //Debug.Log(hit.collider.name);
                    #endregion
                  
                    Instantiate(AoeObject, hit.point, hit.transform.rotation);
                }

            }
            else
            {
                Debug.Log("Error in Aoe Spell");
            }
        }

        if (movementType == DamageEngine.movementType.Spell || movementType == DamageEngine.movementType.AoeInitialization)
        {
            if (this.GetComponent<TrailRenderer>() != null)
            {
                this.GetComponent<TrailRenderer>().enabled = false;
            }
            gameManager.instance.playAudio(DamageEngine.instance.GetSpellSound(attackElement,true), DamageEngine.instance.GetSpellVolume(true));
            //impactSource.PlayOneShot(DamageEngine.instance.GetSpellSound(attackElement, true), DamageEngine.instance.GetSpellVolume(true));
            Instantiate(destroyParticle, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);

            //play particle - CM
        }
        else if (movementType == DamageEngine.movementType.Spell_HitScan)
        {
            gameManager.instance.playAudio(DamageEngine.instance.GetSpellSound(attackElement, true), DamageEngine.instance.GetSpellVolume(true));
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
        if (IsInfectious)
            spreadInfection();
        
        for (int Target = 0; Target < targets.Count; Target++)
        {
            if(targets[Target]!=null)
                DamageEngine.instance.CalculateDamage(targets[Target], damageAmount, attackElement,targets);
        }
        yield return new WaitForSeconds(AttackSpeed);
        Attacking = false;
    }
    void spreadInfection()
    {

        foreach (var target in targets)
        {
            if (target != null) { 
            if (DamageEngine.instance.CurrentInfectedAOE < DamageEngine.instance.MaxInfectedAOE)
            {
                DestructibleHealthCore targetHealth = target.GetComponent<DestructibleHealthCore>();
                if ((targetHealth.statusDictionary[attackElement] == false && AoeObject != null))
                {
                    if (targetHealth.statusDictionary[RequiredElement] || RequiredElement == DamageEngine.ElementType.Normal)
                        Instantiate(AoeObject, target.transform.position, target.transform.rotation);
                }

            }
           }
        }
    }
    public DamageEngine.ElementType ElementType
    {
        get
        {
            return attackElement;
        }
    }
    //currently player only
   //public RaycastHit CastHitScanAttack(LayerMask ignoreMask)
   // {

   //     RaycastHit hit;
   //     if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, SpellRange, ~ignoreMask))
   //     {
   //         //IDamage damage = hit.collider.GetComponent<IDamage>();
   //         //if (damage != null)
   //         //{
   //         //}
   //             DamageEngine.instance.CalculateDamage(hit.collider, damageAmount, DamageEngine.ElementType.Lightning); 
   //     }
   //    return hit;
   // }
}
