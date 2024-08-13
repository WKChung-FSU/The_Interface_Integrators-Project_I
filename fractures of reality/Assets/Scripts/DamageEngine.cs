using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEngine : MonoBehaviour
{
    public static DamageEngine instance;
    // will add more spell types if necessary
    public enum damageType { spellBasic, Environmental, }

    // Start is called before the first frame update
    void Start()
    {
    instance = this;
    }

   public void CalculateDamage(Collider OtherCollider,int DamageAmount,damageType Type){
       
        int TempHealth;
        IDamage dmg = OtherCollider.GetComponent<IDamage>();
        playerControler targetPlayer = OtherCollider.GetComponent<playerControler>();
        EnemyAI TargetEnemyAI = OtherCollider.GetComponent<EnemyAI>();
        dmg.takeDamage(DamageAmount, Type);
        if (targetPlayer != null)
        {

        }
        else if (TargetEnemyAI != null)
        {
            TempHealth = TargetEnemyAI.EnemyHP;

            // add damage calc
            TempHealth -= DamageAmount;
            TargetEnemyAI.EnemyHP = TempHealth;

            if (TempHealth <= 0)
                Destroy(TargetEnemyAI.gameObject);
            gameManager.instance.updateGameGoal(-1);
        }

    }
}
