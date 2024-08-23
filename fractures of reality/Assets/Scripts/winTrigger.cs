using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class winTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent onTrigWinEnt;

    private void OnTriggerEnter(Collider other)
    {
            onTrigWinEnt.Invoke();
            gameManager.instance.youWin();
    }
}
