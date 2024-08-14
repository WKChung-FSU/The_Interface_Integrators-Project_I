using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEngine : MonoBehaviour
{
    public static DamageEngine instance;
    // will add more spell types if necessary
    public enum EnemyType { Normal, fire }
    public enum movementType { Spell, Environmental }
    public enum damageType { spellBasic, Lightning, fire }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void CalculateDamage(Collider OtherCollider, int DamageAmount, damageType dType)
    {
        if (OtherCollider != null)
        {
            int TempHealth;
            IDamage dmg = OtherCollider.GetComponent<IDamage>();

            if (dmg != null)
            {
                playerControler targetPlayer = OtherCollider.GetComponent<playerControler>();
                EnemyAI TargetEnemyAI = OtherCollider.GetComponent<EnemyAI>();
                dmg.takeDamage(DamageAmount, dType);
                if (targetPlayer != null)
                {
                    TempHealth = targetPlayer.PlayerHP;
                    TempHealth -= DamageAmount;
                    if (TempHealth < 0)
                    {
                        TempHealth = 0;
                    }
                    targetPlayer.PlayerHP = TempHealth;
                }
                else if (TargetEnemyAI != null)
                {
                    TempHealth = TargetEnemyAI.EnemyHP;

                    // add damage calc
                    TempHealth -= EnemyTypeMultiplier(TargetEnemyAI.EnemyType,DamageAmount,dType);
                    TargetEnemyAI.EnemyHP = TempHealth;

                    if (TempHealth <= 0)
                    {
                        Destroy(TargetEnemyAI.gameObject);
                        gameManager.instance.updateGameGoal(-1);
                    }
                }
            }
        }
    }

    int EnemyTypeMultiplier(EnemyType enemyType, int damageAmount, damageType type)
    {
        float floatDamage = damageAmount;
        float basicMult = 0, fireMult = 0, lightningMult = 0;
        switch (enemyType)
        {
            case EnemyType.Normal:
                //nothing
                basicMult = 1f;
                fireMult = 1f;
                lightningMult = 1f;
                break;

            case EnemyType.fire:
                basicMult = 1f;
                fireMult = 0.5f;
                lightningMult = 0.8f;
                break;
        }

        switch (type)
        {
            case damageType.spellBasic:
                damageAmount = (int)(floatDamage * basicMult);
                break;
            case damageType.Lightning:
                damageAmount= (int)(floatDamage * lightningMult);
                break;
            case damageType.fire:
                damageAmount =(int) (floatDamage * fireMult);
                break;
        }
        return damageAmount;
    }






}
