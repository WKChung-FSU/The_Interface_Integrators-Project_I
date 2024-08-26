using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Header("----- Main Attributes -----")]
    Color colorOriginal;
    [SerializeField] List<GameObject> WaypointParticles = new List<GameObject>();
      [Header("----- Special Attributes -----")]
    [SerializeField] DamageEngine.ElementType NewElement;
    [SerializeField] bool IsCheckpoint;
    [SerializeField] bool IsElementPoint;
    [SerializeField] bool isTeleporter;
    [SerializeField] GameObject TLGameObject;
    [SerializeField] Vector3 TransportPosition;
    [SerializeField] int TypeChangeAnimationDelay;
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

    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if (other.CompareTag("Player")  && ((IsCheckpoint && gameManager.instance.Checkpoint() != this) || isTeleporter))
        {
            if (isTeleporter)
            {
                gameManager.instance.StartPosition(TransportPosition);
                gameManager.instance.Respawn();
            }
            else
            {
                if (gameManager.instance.Checkpoint() != null)
                {
                    gameManager.instance.Checkpoint().ToggleParticles(false);
                }
                gameManager.instance.Checkpoint(this);
                gameManager.instance.StartPosition(transform.position);
                ToggleParticles(true);
            }
           
            // disable previous waypoint particle
        }
        DestructibleHealthCore otherHealth = other.GetComponent<DestructibleHealthCore>();
        if (IsElementPoint && otherHealth != null)
        {
            otherHealth.SetNewElementType(NewElement);
            StartCoroutine(typeChangeTimer());
        }

    }
    public void ToggleParticles(bool Particle)
    {
       foreach (GameObject Object in WaypointParticles)
        {
            Object.SetActive(Particle);
        }
    }
    IEnumerator typeChangeTimer()
    {
        ToggleParticles(true);
        yield return new WaitForSeconds(TypeChangeAnimationDelay);
        ToggleParticles(false);
    }
}
