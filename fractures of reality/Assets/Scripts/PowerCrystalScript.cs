using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCrystalScript : MonoBehaviour
{
    [SerializeField] PowerCrystalManifest crystalManifest;
    DestructibleHealthCore healthCore;
    DamageEngine.ElementType elementType;

    // Start is called before the first frame update
    void Start()
    {
        healthCore = GetComponent<DestructibleHealthCore>();
        elementType = healthCore.ElementType;
    }

    // Update is called once per frame
   //void Update()
   //{
   //    
   //}
    private void OnDestroy()
    {
        if (healthCore.HP==0)
        crystalManifest.SetDestroyedCrystalOfType(elementType);
    }
}
