using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEngine : MonoBehaviour
{
    // will add more spell types if necessary
    public enum damageType { spellBasic, Environmental, }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb; 

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int RemoveTime;

    // Start is called before the first frame update
    void Start()
    {
        //logic for the spell bullet itself
        if (type == damageType.spellBasic)
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
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount,type);
        }
        Destroy(gameObject);
    }
}
