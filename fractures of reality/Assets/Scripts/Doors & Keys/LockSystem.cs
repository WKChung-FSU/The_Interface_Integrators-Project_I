using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockSystem : MonoBehaviour
{
    [Header("----- Main Attributes -----")]
    [SerializeField] KeySystem keySystem;
    [Header("----- KEY Attributes -----")]
    [SerializeField] bool IsKey;
    [Header("Keys will have triggers, Glyphs will not")]
    [SerializeField] bool IsGlyph;
    [Header("----- Lock Attributes -----")]
    [SerializeField] Collider boxCollider;
    [SerializeField] MeshRenderer meshRenderer;
    [Range(1, 10)][SerializeField] int OpenTime;
    [SerializeField] AudioClip[] doorSounds;
    [Range(0, 1)][SerializeField] float doorSoundVol=0.5f;
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
        playerController player = other.GetComponent<playerController>();
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
                gameManager.instance.playAudio(doorSounds[Random.Range(0, doorSounds.Length)], doorSoundVol);
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
        playerController player = other.GetComponent<playerController>();
        if (KeysSearch(player))
        {
            {
                StartCoroutine(DoorTimer());
            }
        }
    }

    bool KeysSearch(playerController player)
    {
        bool keyFound=false;

        if (keySystem != null && player != null) {
            foreach (var key in player.PlayerKeys)
            {
                if (key == keySystem)
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
