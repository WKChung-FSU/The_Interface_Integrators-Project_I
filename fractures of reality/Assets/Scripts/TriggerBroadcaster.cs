using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBroadcaster : MonoBehaviour
{
    [SerializeField] GameObject listener;
    [SerializeField] bool setListenerActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            listener.SetActive(setListenerActive);

            Destroy(this.gameObject);
        }
    }
}
