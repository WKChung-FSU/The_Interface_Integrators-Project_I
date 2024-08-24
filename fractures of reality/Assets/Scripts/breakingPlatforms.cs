using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class breakingPlatforms : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigEnt;
    [SerializeField] UnityEvent onTrigExi;

    private void OnTriggerEnter(Collider other)
    {
        onTrigEnt.Invoke();

    }

    private void OnTriggerExit(Collider other)
    {
        onTrigExi.Invoke();

    }
}
