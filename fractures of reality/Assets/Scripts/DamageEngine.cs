using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DamageEngine : MonoBehaviour
{
    public enum DamageEffect { Normal, critical, nullified, Weak, Heal }

    [System.Serializable]
    public struct DamageMultipliers
    {
        [SerializeField] public DamageEffect normalMult;
        [SerializeField] public DamageEffect fireMult;
        [SerializeField] public DamageEffect lightningMult;
        [SerializeField] public DamageEffect iceMult;
        [SerializeField] public DamageEffect earthMult;
        [SerializeField] public DamageEffect windMult;
        [SerializeField] public DamageEffect waterMult;
    }

    [SerializeField] DamageMultipliers normalMultiplier, FireMultiplier, LightningMultiplier;
    [SerializeField] DamageMultipliers IceMultiplier, EarthMultiplier, WindMultiplier, WaterMultiplier;

    public static DamageEngine instance;
    // will add more spell types if necessary
    public enum ElementType { Normal, fire, Lightning, Ice, Earth, Wind, Water }
    public enum movementType { Spell, Environmental }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    // remember that this is a instance
    public void CalculateDamage(Collider OtherCollider, int DamageAmount, ElementType attackType)
    {
        if (OtherCollider != null)
        {
            int TempHealth;
            IDamage dmg = OtherCollider.GetComponent<IDamage>();

            if (dmg != null)
            {
                playerControler targetPlayer = OtherCollider.GetComponent<playerControler>();
                DestructibleHealthCore healthCore = OtherCollider.GetComponent<DestructibleHealthCore>();
                TempHealth = healthCore.HP;

                // add damage calculation
                int damageDealt = ElementTypeMultiplier(healthCore.ElementType, DamageAmount, attackType);
                TempHealth -= damageDealt;
                if (TempHealth <= 0)
                {
                    TempHealth = 0;
                    // if player do player things if not do enemy things.
                    if (targetPlayer != null)
                    {
                        if (gameManager.instance.playerDead == false)
                            gameManager.instance.youLose();
                    }
                    else
                    {
                        Destroy(healthCore.gameObject);
                    }
                    if (healthCore.IsMandatory)
                        gameManager.instance.updateGameGoal(-1);
                }
                healthCore.HP = TempHealth;
                // does the specific damage effect
                //Debug.Log("Damage Engine: "+ DamageAmount);
                dmg.damageEffect(damageDealt, attackType);
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
                Multiplier = 2f;
                break;
            case DamageEffect.nullified:
                Multiplier = 0f;
                break;

            case DamageEffect.Weak:
                Multiplier = 0.5f;
                break;

            case DamageEffect.Heal:
                Multiplier = -1f;
                break;
        }
        return (int)(damageAmount * Multiplier);
    }

}
