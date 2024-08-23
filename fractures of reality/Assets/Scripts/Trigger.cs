using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigEnter;
    [SerializeField] UnityEvent onTrigExit;

    private void OnTriggerEnter(Collider other)
    {
        onTrigEnter.Invoke();
        gameManager.instance.youLose();

    }

    private void OnTriggerExit(Collider other)
    {
        onTrigExit.Invoke();
    }
}
