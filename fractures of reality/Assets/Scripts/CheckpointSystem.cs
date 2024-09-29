using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSystem : MonoBehaviour
{
    enum WaypointType { Checkpoint,Teleporter,WinPoint,ScenePoint}
    [Header("----- Main Attributes -----")]
    [SerializeField] List<GameObject> WaypointParticles = new List<GameObject>();
    [SerializeField] WaypointType waypoint;
    [SerializeField] bool IsElementPoint;
    [SerializeField] bool ClearKeys;
    [Header("----- type change Attributes -----")]
    [SerializeField] DamageEngine.ElementType NewElement;
    [SerializeField] int TypeChangeAnimationDelay;
    [Header("----- Teleportation Attributes -----")]
    [SerializeField] GameObject TLGameObject;
    [SerializeField] bool TransportPosAsNewStart;
    [SerializeField] Vector3 TransportPosition;
    [Header("----- Scene Attributes -----")]
    [SerializeField] int sceneIndex;
    void Start()
    {
        if (TLGameObject != null)
        {
            TransportPosition = TLGameObject.transform.position;
        }
        ///COCONUT.JPG IS REAL!!!!!!!!!!!!
       if(waypoint != WaypointType.WinPoint) 
        ToggleParticles(false);

    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if (other.CompareTag("Player")  && (((waypoint==WaypointType.Checkpoint && gameManager.instance.Checkpoint() != this) || waypoint != WaypointType.Checkpoint)))
        {
            if (ClearKeys) { 
            gameManager.instance.PlayerController.PlayerKeys.Clear();
            }
            switch (waypoint)
            {
                case WaypointType.Checkpoint:
                    if (gameManager.instance.Checkpoint() != null)
                    {
                        gameManager.instance.Checkpoint().ToggleParticles(false);
                    }
                    gameManager.instance.Checkpoint(this);
                    gameManager.instance.StartPosition(transform.position);
                    ToggleParticles(true);
                    break;
                case WaypointType.Teleporter:

                    if (TransportPosAsNewStart)
                        gameManager.instance.StartPosition(TransportPosition);

                    gameManager.instance.playerScript.TeleportTo(TransportPosition);
                    break;
                case WaypointType.WinPoint:
                    gameManager.instance.youWin();
                    break;

                case WaypointType.ScenePoint:
                    SceneManager.LoadScene(sceneIndex);
                        break;
                default:
                    break;
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
