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
        if (crystalManifest.DestroyList.Contains(elementType))
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (healthCore.HP==0 && !crystalManifest.DestroyList.Contains(elementType))
        crystalManifest.SetDestroyedCrystalOfType(elementType);
    }
}
