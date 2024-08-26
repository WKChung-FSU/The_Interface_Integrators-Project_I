using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    DestructibleHealthCore health;
    public int healthBonus =25;

    private void Awake()
    {
        health = FindObjectOfType<DestructibleHealthCore>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<DestructibleHealthCore>())
        {
            if (other.GetComponent<DestructibleHealthCore>().HP < other.GetComponent<DestructibleHealthCore>().HPMax)
            {
                Destroy(gameObject);
                other.GetComponent<DestructibleHealthCore>().HP += healthBonus;
                if (other.GetComponent<DestructibleHealthCore>().HP > other.GetComponent<DestructibleHealthCore>().HPMax)
                {
                    other.GetComponent<DestructibleHealthCore>().HP = other.GetComponent<DestructibleHealthCore>().HPMax;
                }
            }
            
        }
        
        
    }
}
