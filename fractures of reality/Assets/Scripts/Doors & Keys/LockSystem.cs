using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LockSystem : MonoBehaviour
{
    [Header("----- Main Attributes -----")]
    [SerializeField] KeySystem keySystem;
    [Header("----- KEY Attributes -----")]
    [SerializeField] bool IsKey;
    [Header("----- Lock Attributes -----")]
    [SerializeField] Collider boxCollider;
    [SerializeField] MeshRenderer meshRenderer;
    [Range(1, 10)][SerializeField] int OpenTime;
    bool Open;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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

        playerControler player = other.GetComponent<playerControler>();
        if (player != null)
        {
            if ((keySystem != null && IsKey))
            {
                if (!player.PlayerKeys.Contains(keySystem))
                    player.PlayerKeys.Add(keySystem);
                Destroy(this.gameObject);
            }
            else if (!IsKey&& KeysSearch(player))
            {
                meshRenderer.enabled = false;
                boxCollider.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger||IsKey)
        {
            return;
        }
        playerControler player = other.GetComponent<playerControler>();
        if (KeysSearch(player))
        {
            {
                StartCoroutine(DoorTimer());
            }
        }
    }

    bool KeysSearch(playerControler player)
    {
        bool keyFound=false;

        if (keySystem != null && player != null) {
            foreach (var key in player.PlayerKeys)
            {
                if (key.PairedDoor == keySystem.PairedDoor)
                {
                    keyFound = true;
                    break;
                }
            }
        }
        return keyFound;
    }

    IEnumerator DoorTimer()
    {
        yield return new WaitForSeconds(OpenTime);
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }
}
