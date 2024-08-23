using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCore : MonoBehaviour
{
    [Header("-----MainAttributes-----")]
    [SerializeField] DamageEngine.ElementType attackElement;
    [SerializeField] DamageEngine.movementType movementType;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;

    [Header("-----Spell Attributes-----")]
    [SerializeField] int speed;
    [SerializeField] int RemoveTime;

    [Header("-----Environmental Attributes-----")]
    [SerializeField] int AttackSpeed = 1;

    Collider target;
    bool EntityInRange;
    bool Attacking;
    // Start is called before the first frame update
    void Start()
    {
        if (movementType == DamageEngine.movementType.Spell)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, RemoveTime);
        }

    }

    private void Update()
    {
        if (EntityInRange&&!Attacking)
        {
            StartCoroutine(environmentalAttack());
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (movementType == DamageEngine.movementType.Environmental)
        {
            EntityInRange = true;
            target = other;
        }
        else
        {
            DamageEngine.instance.CalculateDamage(other, damageAmount, attackElement);
        }
        //do damage

        if (movementType == DamageEngine.movementType.Spell)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (movementType == DamageEngine.movementType.Environmental)
        {
            EntityInRange = false;
            target=null;

        }
    }

    IEnumerator environmentalAttack()
    {
        Attacking = true;
        DamageEngine.instance.CalculateDamage(target, damageAmount, attackElement);
        yield return new WaitForSeconds(AttackSpeed);
        Attacking = false;
    }
}
