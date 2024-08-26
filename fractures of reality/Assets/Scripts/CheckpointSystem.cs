using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Header("----- Main Attributes -----")]
    Color colorOriginal;
    [SerializeField] Renderer model;
    [SerializeField] DamageEngine.ElementType NewElement;
    [Header("----- Special Attributes -----")]

    [SerializeField] bool IsCheckpoint;
    [SerializeField] bool IsElementPoint;

    void Start()
    {
        colorOriginal = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if ((other.CompareTag("Player") && gameManager.instance.StartPosition() != this.transform.position)&&IsCheckpoint)
        {
            gameManager.instance.StartPosition(transform.position);
            StartCoroutine(FlashModel());
        }
        DestructibleHealthCore otherHealth = other.GetComponent<DestructibleHealthCore>();
        if (IsElementPoint && otherHealth != null)
        {
            otherHealth.ElementType= NewElement;
        }

    }
    IEnumerator FlashModel()
    {
       
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        model.material.color = colorOriginal;
       
    }
}
