using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageEngine : MonoBehaviour
{
    public enum DamageEffect { Normal, critical, nullified, Weak, Heal }

    public static DamageEngine instance;
    // will add more spell types if necessary
    public enum ElementType { Normal, fire, Lightning, Ice, Earth, Wind, Water }
    public enum movementType { Spell, Environmental, melee, Spell_HitScan, AoeSpell }
    // Start is called before the first frame update
    [System.Serializable]
     struct DamageMultipliers
    {
        [SerializeField] public DamageEffect normalMult;
        [SerializeField] public DamageEffect fireMult;
        [SerializeField] public DamageEffect lightningMult;
        [SerializeField] public DamageEffect iceMult;
        [SerializeField] public DamageEffect earthMult;
        [SerializeField] public DamageEffect windMult;
        [SerializeField] public DamageEffect waterMult;
    }

    [Header("---- Global Effect Multipliers ----")]
    [SerializeField] float critMult = 2f;
    [SerializeField] float nullMult = 0f;
    [SerializeField] float weakMult = 0.5f;
    [SerializeField] float healMult = -1f;

    [Header("---- Damage Multipliers ----")]
    [SerializeField] DamageMultipliers normalMultiplier;
    [SerializeField] DamageMultipliers FireMultiplier, LightningMultiplier, IceMultiplier, EarthMultiplier, WindMultiplier, WaterMultiplier;


    [Header("AOE Effects Zones")]
    [SerializeField] public GameObject lightningAOE;

    [Header("---- Spell sounds ----")]
    
    [SerializeField] AudioClip[] castSounds;
    [Range(0, 1)][SerializeField] float castSoundsVol = 0.5f;
    [SerializeField] AudioClip[] impactSounds;
    [Range(0, 1)][SerializeField] float impactSoundsVol = 0.5f;

    [Header("---- Player Sounds ----")]
    [SerializeField] AudioClip[] AudioHurt;
    [Range(0, 1)][SerializeField] float AudioHurtVol = 0.5f;

    void Start()
    {
        instance = this;
    }
    // remember that this is a instance
    public void CalculateDamage(Collider OtherCollider, int DamageAmount, ElementType attackType, List<Collider>AttackList=null)
    {
        if (OtherCollider != null)
        {
            int TempHealth;
            IDamage dmg = OtherCollider.GetComponent<IDamage>();

            if (dmg != null)
            {
                playerController targetPlayer = OtherCollider.GetComponent<playerController>();
                DestructibleHealthCore healthCore = OtherCollider.GetComponent<DestructibleHealthCore>();
                TempHealth = healthCore.HP;

                if (DamageAmount == 0)
                {
                    dmg.damageEffect(DamageAmount, attackType);
                }
                else
                {
                    // add damage calculation
                    int damageDealt = ElementTypeMultiplier(healthCore.ElementType, DamageAmount, attackType);
                    TempHealth -= damageDealt;

                    if(targetPlayer != null)
                    {
                        gameManager.instance.playAudio(AudioHurt[Random.Range(0, AudioHurt.Length)], AudioHurtVol);
                    }


                    if (TempHealth <= 0)
                    {
                        TempHealth = 0;
                        // if player do player things if not do enemy things.
                        if (healthCore.IsMandatory)
                            gameManager.instance.updateGameGoal(-1);
                        if (targetPlayer != null)
                        {
                            if (gameManager.instance.PlayerDead == false)
                                gameManager.instance.youLose();
                        }
                        else
                        {
                            Vector3 spawnLocation=new Vector3();
                            Quaternion spawnRotation= new Quaternion();
                            GameObject SpawnObject=null;
                            bool isSpawning = false;
                            if (AttackList != null)
                            {
                                AttackList.Remove(OtherCollider);
                            }
                            if (healthCore.GETSpawnsItemOnDeath())
                            {
                                spawnLocation = healthCore.transform.position;
                                spawnRotation=healthCore.transform.rotation;
                                 SpawnObject = healthCore.GETDeathSpawnItems();
                                isSpawning = true;
                            }
                            Destroy(healthCore.gameObject);

                            if (isSpawning)
                                Instantiate(SpawnObject, spawnLocation + Vector3.up, spawnRotation);
                        }
                        
                    }
                    healthCore.HP = TempHealth;
                    // does the specific damage effect
                    //Debug.Log("Damage Engine: "+ DamageAmount);
                    dmg.damageEffect(damageDealt, attackType);
                }
            }
        }
    }

     int ElementTypeMultiplier(ElementType enemyType, int damageAmount, ElementType attackType)
    {
        float floatDamage = damageAmount;
        DamageMultipliers CurrentMult = normalMultiplier;
        DamageEffect currentEffect;
        switch (enemyType)
        {
            // Set enemies Multiplier
            case ElementType.fire:
                CurrentMult = FireMultiplier;
                break;

            case ElementType.Lightning:
                CurrentMult = LightningMultiplier;
                break;

            case ElementType.Ice:
                CurrentMult = IceMultiplier;
                break;

            case ElementType.Earth:
                CurrentMult = EarthMultiplier;
                break;

            case ElementType.Wind:
                CurrentMult = WindMultiplier;
                break;

            case ElementType.Water:
                CurrentMult = WaterMultiplier;
                break;
        }
        // no need to touch below
        switch (attackType)
        {
            case ElementType.Normal:
                currentEffect = CurrentMult.normalMult;
                damageAmount = DamageMult(currentEffect, floatDamage);
                break;
            case ElementType.fire:
                currentEffect = CurrentMult.fireMult;
                damageAmount = DamageMult(currentEffect,floatDamage);
                break;
            case ElementType.Lightning:
                currentEffect = CurrentMult.lightningMult;
                damageAmount = DamageMult(currentEffect, floatDamage);
                break;
            case ElementType.Ice:
                currentEffect = CurrentMult.iceMult;
                damageAmount = DamageMult(currentEffect, floatDamage);
                break;
            case ElementType.Earth:
                currentEffect = CurrentMult.earthMult;
                damageAmount = DamageMult(currentEffect, floatDamage);
                break;
            case ElementType.Wind:
                currentEffect = CurrentMult.windMult;
                damageAmount = DamageMult(currentEffect, floatDamage);
                break;
            case ElementType.Water:
                currentEffect = CurrentMult.waterMult;
                damageAmount = DamageMult(currentEffect, floatDamage);
                break;
        }
        return damageAmount;
    }

    int DamageMult(DamageEffect AttackElement, float damageAmount)
    {
        float Multiplier=1f;
        switch (AttackElement)
        {
            case DamageEffect.critical:
                Multiplier = critMult;
                break;
            case DamageEffect.nullified:
                Multiplier = nullMult;
                break;

            case DamageEffect.Weak:
                Multiplier = weakMult;
                break;

            case DamageEffect.Heal:
                Multiplier = healMult;
                break;
        }
        return (int)(damageAmount * Multiplier);
    }

    public AudioClip GetSpellSound(ElementType spellElement, bool IsImpact)
    {
        AudioClip[] Output;
        AudioClip outClip;
        if (IsImpact)
            Output = impactSounds;
        else
            Output = castSounds;
        switch (spellElement)
        {
            default:
                outClip = Output[0];
                break;

            case ElementType.fire:
                outClip = Output[1];
                break;

            case ElementType.Lightning:
                outClip = Output[2];
                break;

            case ElementType.Ice:
                outClip = Output[3];
                break;

            case ElementType.Earth:
                outClip = Output[4];
                break;

            case ElementType.Wind:
                outClip = Output[5];
                break;

            case ElementType.Water:
                outClip = Output[6];
                break;
        }
        return outClip;
    }
    public float GetSpellVolume(bool IsImpact)
    {
        float volume=0.5f;
        if (IsImpact)
            volume = impactSoundsVol;
        else
            volume = castSoundsVol;

        return volume;
    }
}
