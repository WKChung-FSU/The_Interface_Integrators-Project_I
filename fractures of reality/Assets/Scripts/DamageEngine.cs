using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEngine : MonoBehaviour
{
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
                healthCore.HP = TempHealth;
                if (TempHealth <= 0)
                {
                    TempHealth = 0;
                    // if player do player things if not do enemy things.
                    if (targetPlayer != null)
                    {
                        gameManager.instance.youLose();
                    }
                    else
                    {
                        Destroy(healthCore.gameObject);
                    }
                    if (healthCore.IsMandatory)
                        gameManager.instance.updateGameGoal(-1);
                }
                // does the specific damage effect
                //Debug.Log("Damage Engine: "+ DamageAmount);
                dmg.damageEffect(damageDealt, attackType);
            }
        }
    }

    int ElementTypeMultiplier(ElementType enemyType, int damageAmount, ElementType attackType)
    {
        float floatDamage = damageAmount;
        float fireMult = 1, lightningMult = 1, iceMult=1,earthMult=1,windMult=0,waterMult = 1;

        switch (enemyType)
        {
            // change numbers below to change the multipliers for specific elements
            case ElementType.Normal:
                break;
// edit these for global multipliers for attacks
            case ElementType.fire:
                
                fireMult = 0f;
                lightningMult = 0.5f;
                iceMult = 1.5f;
                windMult = -0.5f;
                waterMult = 2f;
                break;
            case ElementType.Lightning:
                lightningMult = 0f;
                earthMult = 2f;
                windMult = -0.5f;
                waterMult = 2f;

                break;
            case ElementType.Ice:
               
                fireMult = 2f;
                iceMult = 0f;
                earthMult = 1f;
                waterMult = -1f;

                break;

            case ElementType.Earth:
                fireMult = 0f;
                lightningMult = 0f;
                iceMult = 1.5f;

                break;
            case ElementType.Wind:
                // wind is immortal
                break;
            case ElementType.Water:
                lightningMult = 3f;
                iceMult = 2f;
                waterMult = 0f;
                break;
        }
        // no need to touch below
        switch (attackType)
        {
            case ElementType.Normal:
                break;
            case ElementType.fire:
                damageAmount = (int)(floatDamage * fireMult);
                break;
            case ElementType.Lightning:
                damageAmount = (int)(floatDamage * lightningMult);
                break;
            case ElementType.Ice:
                damageAmount = (int)(floatDamage * iceMult);
                break;
            case ElementType.Earth:
                damageAmount = (int)(floatDamage * earthMult);
                break;
            case ElementType.Wind:
                damageAmount = (int)(floatDamage * windMult);
                break;
            case ElementType.Water:
                damageAmount = (int)(floatDamage * waterMult);
                break;
        }
        return damageAmount;
    }
}
