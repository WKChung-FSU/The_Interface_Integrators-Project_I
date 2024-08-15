using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEngine : MonoBehaviour
{
    [SerializeField] DamageEngine.damageType damageType;
    [SerializeField] DamageEngine.movementType movementType;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int RemoveTime;

    // Start is called before the first frame update
    void Start()
    {
        if (movementType == DamageEngine.movementType.Spell)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, RemoveTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        DamageEngine.instance.CalculateDamage(other, damageAmount, damageType);


        if (movementType == DamageEngine.movementType.Spell)
        {
            Destroy(gameObject);
        }
    }
}
