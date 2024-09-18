using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class AttackCore : MonoBehaviour
{
    [Header("-----MainAttributes-----")]
    [SerializeField] DamageEngine.ElementType attackElement;
    [SerializeField] DamageEngine.movementType movementType;
    [Range(-20, 100)][SerializeField] int damageAmount;
    [Range(0, 100)][SerializeField] int CastFractureDamage;
    [Header("if 0 then it will not deSpawn")]
    [Range(0, 30)][SerializeField] float RemoveTime = 0;
    [SerializeField] GameObject destroyParticle;
    [Header("-----Spell Attributes-----")]
    [Range(1, 30)][SerializeField] int speed;
    [SerializeField] Rigidbody rb;

    [Header("-----Aoe Components-----")]
    [SerializeField] bool IsInfectious = false;
    [SerializeField] GameObject AoeObject = null;
    [SerializeField] int colliderRadius;
    [SerializeField] LayerMask TargetMask1;
    [SerializeField] LayerMask TargetMask2;
    [Header("element that you need to spread infectious Aoe,")]
    [Header("if Normal it will always spread")]
    [SerializeField] DamageEngine.ElementType RequiredElement;
    [Header("HitScan info(Currently player only)")]
    [Range(1, 100)][SerializeField] int SpellRange;
    [SerializeField] LayerMask ignoreMask;
    [Header("-----Environmental Attributes-----")]
    [Header("will not attack if 0 / is also used for infection")]
    [Range(0, 1)][SerializeField] float AttackSpeed;
    [Header("-----Spell tracking-----")]
    [Range(0, 1)][SerializeField] float TrackingStrength;
    DestructibleHealthCore SpawningEntity;
    int FinalTargetMask;
    public List<Collider> targets = new List<Collider>();
    bool Attacking;
    bool IsTracking;

    // Start is called before the first frame update
    void Start()
    {
        switch (movementType)
        {
            case DamageEngine.movementType.Spell:
                case DamageEngine.movementType.AoeInitialization:
                    case DamageEngine.movementType.teleportation:
                gameManager.instance.playAudio(DamageEngine.instance.GetSpellSound(attackElement, false), DamageEngine.instance.GetSpellVolume(false));
                rb.velocity = rb.transform.forward * speed;
                if (RemoveTime != 0)
                {
                Destroy(this.GetComponent<TrailRenderer>(), RemoveTime);
                Destroy(gameObject, RemoveTime);
                }
                break;

            case DamageEngine.movementType.Spell_HitScan:
                StartCoroutine(LightningSpell());

                break;

            case DamageEngine.movementType.Environmental:
                if (RemoveTime != 0)
                {
                    Destroy(gameObject, RemoveTime);
                }
                break;

                default:
                Debug.Log("How? movement type error...");
                break;
        }

        if (IsInfectious)
            DamageEngine.instance.UpdateAOEs(1);

        FinalTargetMask = TargetMask1 | TargetMask2;
    }
    private void OnDestroy()
    {
        if (movementType == DamageEngine.movementType.teleportation)
        {
            gameManager.instance.playerScript.TeleportTo(transform.position+(Vector3.up*2));
        }


        if (IsInfectious)
                DamageEngine.instance.UpdateAOEs(-1);
        }


    private void Update()
    {

        switch (movementType)
        {
            default:


                break;

            case DamageEngine.movementType.Spell:
                if(!IsTracking)
                    StartCoroutine(SpellTracking());
                break;
            case DamageEngine.movementType.Environmental:
                if (!Attacking && AttackSpeed != 0)
                {
                    StartCoroutine(environmentalAttack());
                }
                break;
            case DamageEngine.movementType.teleportation:
                if (this.gameObject != gameManager.instance.playerWeapon.GetCurrentWeapon(true))
                {
                    Debug.Log(this.gameObject);
                    //Destroy(this.gameObject);
                }
                break;
        }

        if (IsInfectious)
        {
            Collider[] others = Physics.OverlapSphere(transform.position, colliderRadius, FinalTargetMask);

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

        if (other.isTrigger || other == gameManager.instance.player.GetComponent<CapsuleCollider>())
        {
            return;
        }
        if (movementType == DamageEngine.movementType.teleportation)
        {
            Destroy(gameObject);
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

        if (movementType == DamageEngine.movementType.AoeInitialization)
        {
            if (AoeObject != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (transform.up * -1), out hit))
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
            gameManager.instance.playAudio(DamageEngine.instance.GetSpellSound(attackElement, true), DamageEngine.instance.GetSpellVolume(true));
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

    public float SpellTrackingStrength
    {
        get { return TrackingStrength; }
        set { TrackingStrength = value; }
    }


    private void OnTriggerExit(Collider other)
    {
        if (movementType == DamageEngine.movementType.Environmental)
        {
            targets.Remove(other);
        }

        return;
    }

   IEnumerator SpellTracking()
    {
        IsTracking=true;
        Quaternion currentRotation = rb.rotation;
        
        Quaternion PlayerAngle = Quaternion.LookRotation(gameManager.instance.player.transform.position - gameObject.transform.position);
        rb.rotation= Quaternion.Lerp(currentRotation, PlayerAngle,TrackingStrength);
        rb.velocity = rb.transform.forward * speed;
        yield return new WaitForSeconds(0.1f);
        IsTracking=false;
    }
    IEnumerator environmentalAttack()
    {
        Attacking = true;
        if (IsInfectious)
            spreadInfection();

        for (int Target = 0; Target < targets.Count; Target++)
        {
            if (targets[Target] != null)
                DamageEngine.instance.CalculateDamage(targets[Target], damageAmount, attackElement, targets);
        }
        yield return new WaitForSeconds(AttackSpeed);
        Attacking = false;
    }
    void spreadInfection()
    {

        foreach (var target in targets)
        {
            if (target != null)
            {
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
    public int GetFractureDamage()
    {
        return CastFractureDamage;
    }
    //currently player only

    IEnumerator LightningSpell()
    {
        LineRenderer lightningVisual = gameManager.instance.playerWeapon.GetLineRenderer();
        //Visual of lightning being cast 
        lightningVisual.useWorldSpace = true;
        lightningVisual.SetPosition(0, transform.position);
        gameManager.instance.playAudio(DamageEngine.instance.GetSpellSound(DamageEngine.ElementType.Lightning, false), DamageEngine.instance.GetSpellVolume(false));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Camera.main.transform.forward, out hit, SpellRange, ~ignoreMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                DamageEngine.instance.CalculateDamage(hit.collider, 1, DamageEngine.ElementType.Lightning);
                Instantiate(DamageEngine.instance.lightningAOE, hit.transform.position, hit.transform.rotation);
            }
            lightningVisual.SetPosition(1, hit.point);
            lightningVisual.enabled = true;
            yield return new WaitForSeconds(0.1f);
            lightningVisual.enabled = false;

            Destroy(gameObject);
        }
      
    }
   
}
