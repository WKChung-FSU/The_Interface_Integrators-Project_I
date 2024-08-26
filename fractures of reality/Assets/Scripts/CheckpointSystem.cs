using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Header("----- Main Attributes -----")]
    Color colorOriginal;
    [SerializeField] Renderer model;
    [SerializeField] List<GameObject> WaypointParticles = new List<GameObject>();
      [Header("----- Special Attributes -----")]
    [SerializeField] DamageEngine.ElementType NewElement;
    [SerializeField] bool IsCheckpoint;
    [SerializeField] bool IsElementPoint;
    [SerializeField] bool isTeleporter;
    [SerializeField] GameObject TLGameObject;
    [SerializeField] Vector3 TransportPosition;
    void Start()
    {
        if (TLGameObject != null)
        {
            TransportPosition = TLGameObject.transform.position;
        }
        ///COCONUT.JPG IS REAL!!!!!!!!!!!!
        ToggleParticles(false);

        if (IsCheckpoint) 
        isTeleporter=false;

        colorOriginal = model.material.color;
    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if ((other.CompareTag("Player") && gameManager.instance.StartPosition() != this.transform.position)&&IsCheckpoint||isTeleporter)
        {
            if (isTeleporter)
            {
                gameManager.instance.StartPosition(TransportPosition);
                // sent player to start position
            }
            else
            {
                gameManager.instance.StartPosition(transform.position);
            }
            ToggleParticles(true);
            // disable previous waypoint particle
        }
        DestructibleHealthCore otherHealth = other.GetComponent<DestructibleHealthCore>();
        if (IsElementPoint && otherHealth != null)
        {
            otherHealth.ElementType= NewElement;
            ToggleParticles(true);
        }

    }
    void ToggleParticles(bool Particle)
    {
       foreach (GameObject Object in WaypointParticles)
        {
            Object.SetActive(Particle);
        }
    }
}
