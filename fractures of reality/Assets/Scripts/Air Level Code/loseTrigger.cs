using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class loseTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigDeathEnt;

    private void OnTriggerEnter(Collider other)
    {
            onTrigDeathEnt.Invoke();
            gameManager.instance.youLose();
    }
}
