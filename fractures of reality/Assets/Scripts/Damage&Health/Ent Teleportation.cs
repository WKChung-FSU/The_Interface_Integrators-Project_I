using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntTeleportation : MonoBehaviour
{
    [SerializeField] DestructibleHealthCore TransportObject;
    [SerializeField] float TeleportRange;
    [SerializeField] LayerMask environment;
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject CurrentReticle;
    Renderer ReticleRenderer;
    bool Holding;
    bool CanTeleport;
    // Start is called before the first frame update
    private void Start()
    {
        if(CurrentReticle == null)
        CurrentReticle = Instantiate(reticle, transform.position, transform.rotation);
        
        if (ReticleRenderer == null)
        {
            ReticleRenderer=CurrentReticle.GetComponent<Renderer>();
        }
        CanTeleport = true;
        ReticleRenderer.enabled = false;
    }
    private void OnDestroy()
    {
        gameManager.instance.playerWeapon.ClearTP();
        Destroy(CurrentReticle);
        CanTeleport=false;
    }

    public void Teleport(bool Holding)
    {


        RaycastHit reticlePos;
        if (Physics.Raycast(transform.position, Camera.main.transform.forward, out reticlePos, TeleportRange, environment))
        {
            if (Holding)
            {
                ReticleRenderer.enabled = true;
                reticle.transform.position = reticlePos.point;
            }
            else
            {
                ReticleRenderer.enabled = false;
                if (CanTeleport)
                    TransportObject.TeleportTo(reticlePos.transform.position);
            }
        }
    }
    public void SetHealthCore(DestructibleHealthCore core)
    {
        TransportObject=core;
    }
}
