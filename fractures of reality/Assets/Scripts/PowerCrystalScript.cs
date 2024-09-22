using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerCrystalScript : MonoBehaviour
{
    [SerializeField] PowerCrystalManifest crystalManifest;
    [SerializeField] float displayTime = 5f;
    DestructibleHealthCore healthCore;
    DamageEngine.ElementType elementType;
    [SerializeField] GameObject textObject;
    TMP_Text textBox;

    // Start is called before the first frame update
    void Start()
    {
        textBox = textObject.GetComponent<TMP_Text>();
        healthCore = GetComponent<DestructibleHealthCore>();
        elementType = healthCore.ElementType;
        if (crystalManifest.DestroyList.Contains(elementType))
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (healthCore.HP == 0 && !crystalManifest.DestroyList.Contains(elementType))
        {
            gameManager.instance.ToolTip(textBox, displayTime);
            crystalManifest.SetDestroyedCrystalOfType(elementType);
        }
    }
}
