using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageEngine : MonoBehaviour
{
    public enum DamageEffect { Normal, critical, nullified, Weak, Heal }

    public static DamageEngine instance;
    // will add more spell types if necessary
    public enum ElementType { Normal, fire, Lightning, Ice, Earth,Water, Wind_tempHeal  }
    public enum movementType { Spell, Environmental, melee, Spell_HitScan, AoeInitialization, teleportation }
    // struct framework for the damage multipliers, will eventually change it to a scriptable game object
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
    [Header("---Given Spells---")]
    [Header("if empty will not give a spell, should work with more than one")]
    [SerializeField] List<DamageEngine.ElementType> GivenSpells;
    [SerializeField] bool ClearLists;
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
    [SerializeField] int MaxInfectAoe;
    [Header("---- Spell sounds ----")]
    
    [SerializeField] AudioClip[] castSounds;
    [Range(0, 1)][SerializeField] float castSoundsVol = 0.5f;
    [SerializeField] AudioClip[] impactSounds;
    [Range(0, 1)][SerializeField] float impactSoundsVol = 0.5f;

    [Header("---- Player Sounds ----")]
    [SerializeField] AudioClip[] AudioHurt;
    [Range(0, 1)][SerializeField] float AudioHurtVol = 0.5f;
    int currInfectAoeAmount;

    // remember that this is a instance
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (ClearLists)
        {
         gameManager.instance.playerWeapon.CurrentList.ClearSpells();
        }

        if (GivenSpells.Count > 0)
        {
            GivenSpells.ForEach(gameManager.instance.playerWeapon.AddSpell);
        }
    }
    #region Getters/Setters
    public int MaxInfectedAOE
    {
        get 
        {
            return MaxInfectAoe;
        }
    }
    public int CurrentInfectedAOE
    {
        get { return currInfectAoeAmount; }
    }

    public void UpdateAOEs(int amount = 0)
    {
        currInfectAoeAmount += amount;

    }

    #endregion
    /// <summary>
    /// the Main Function of this class, to make something take damage all you need to add is the collider of what you hit
    /// </summary>
    /// <param name="OtherCollider">Collider of the thing that is taking damage</param>
    /// <param name="DamageAmount">the amount of damage that the entity takes</param>
    /// <param name="attackType">the element of the attack</param>
    /// <param name="AttackList">if you are attacking in a list(Aoe) you can send it in the function and it will be auto deleted if it dies</param>
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

                // skips damage calc if damage is zero
                if (DamageAmount == 0)
                {
                    dmg.damageEffect(DamageAmount, attackType);
                }
                else
                {
                    // add damage calculation
                    int damageDealt = ElementTypeMultiplier(healthCore.ElementType, DamageAmount, attackType);

                    if (damageDealt ==0&&targetPlayer)
                    {
                        // hardcoded for debug purposes
                        targetPlayer.UpdateFractureBar(DamageAmount);
                    }

                    TempHealth -= damageDealt;
                   
                    // if it is the player
                    if (targetPlayer != null)
                    {
                        gameManager.instance.playAudio(AudioHurt[Random.Range(0, AudioHurt.Length)], AudioHurtVol);
                    }

                    if (TempHealth <= 0)
                    {
                        TempHealth = 0;
                        if (AttackList != null)
                        {
                            AttackList.Remove(OtherCollider);
                        }
                        //  checks if the enemy is mandatory then you remove it form the game goal.
                        if (healthCore.IsMandatory)
                            gameManager.instance.updateGameGoal(-1);

                        // death code for the player
                        if (targetPlayer != null)
                        {
                            if (gameManager.instance.PlayerDead == false)
                                gameManager.instance.youLose();
                        }
                        // death code for the enemies
                        else
                        {
                            //if this thing with health was part of the crowd surrounding the player
                            if (gameManager.instance.CrowdListContains(healthCore.gameObject))
                            {
                                //remove it from the crowd list
                                gameManager.instance.RemoveFromPlayerMeleeRangeList(healthCore.gameObject);
                            }
                            gameManager.instance.scoreKeeper.AddTally(healthCore.thisEnemy);
                            Destroy(healthCore.gameObject);
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


    /// <summary>
    /// Finds the proper damage for the attack
    /// </summary>
    /// <param name="enemyType">the element type of the entity being attacked</param>
    /// <param name="damageAmount">the base amount of damage</param>
    /// <param name="attackType">the element of the attack</param>
    /// <returns></returns>

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

            case ElementType.Wind_tempHeal:
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
            case ElementType.Wind_tempHeal:
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

            case ElementType.Wind_tempHeal:
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


   void FractureEngine(ElementType CurrentSpellElement,ElementType SpellHitElement)
    {



    }
}
