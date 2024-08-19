using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEngine : MonoBehaviour
{
    public static DamageEngine instance;
    // will add more spell types if necessary
    public enum ElementType { Normal, fire, Lightning }
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
                DestructibleHealthCore TargetEnemyAI = OtherCollider.GetComponent<DestructibleHealthCore>();
                if (targetPlayer != null)
                {
                    TempHealth = targetPlayer.PlayerHP;
                    TempHealth -= DamageAmount;
                    if (TempHealth <= 0)
                    {
                        TempHealth = 0;
                        gameManager.instance.youLose();
                    }
                    targetPlayer.PlayerHP = TempHealth;
                }
                else if (TargetEnemyAI != null)
                {
                    TempHealth = TargetEnemyAI.EnemyHP;

                    // add damage calc
                    TempHealth -= EnemyTypeMultiplier(TargetEnemyAI.EnemyType,DamageAmount,attackType);
                    TargetEnemyAI.EnemyHP = TempHealth;

                    if (TempHealth <= 0)
                    {
                        Destroy(TargetEnemyAI.gameObject);
                        gameManager.instance.updateGameGoal(-1);
                    }
                }
                dmg.takeDamage(DamageAmount, attackType);
            }
        }
    }

    int EnemyTypeMultiplier(ElementType enemyType, int damageAmount, ElementType attackType)
    {
        float floatDamage = damageAmount;
        float basicMult = 0, fireMult = 0, lightningMult = 0;
        switch (enemyType)
        {
            case ElementType.Normal:
                //nothing
                basicMult = 1f;
                fireMult = 1f;
                lightningMult = 1f;
                break;

            case ElementType.fire:
                basicMult = 1f;
                fireMult = 0.5f;
                lightningMult = 0.8f;
                break;
        }

        switch (attackType)
        {
            case ElementType.Normal:
                damageAmount = (int)(floatDamage * basicMult);
                break;
            case ElementType.fire:
                damageAmount = (int)(floatDamage * fireMult);
                break;
            case ElementType.Lightning:
                damageAmount= (int)(floatDamage * lightningMult);
                break;
            
        }
        return damageAmount;
    }






}
