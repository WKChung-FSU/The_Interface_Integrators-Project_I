using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEngine : MonoBehaviour
{
    [SerializeField] DamageEngine.damageType damageType;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int RemoveTime;


    // Start is called before the first frame update
    void Start()
    {
        if (damageType == DamageEngine.damageType.spellBasic)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, RemoveTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
       
            DamageEngine.instance.CalculateDamage(other, damageAmount, damageType);
     

        TrailRenderer objectTrail = gameObject.GetComponent<TrailRenderer>();
        if (objectTrail != null)
            objectTrail.enabled = false;

        Destroy(gameObject);
    }
}
