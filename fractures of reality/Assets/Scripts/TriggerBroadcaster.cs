using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBroadcaster : MonoBehaviour
{
    [SerializeField] GameObject listener;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            listener.SetActive(true);

            Destroy(this.gameObject);
        }
    }
}
