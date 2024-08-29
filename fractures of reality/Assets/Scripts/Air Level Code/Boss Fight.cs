using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossFight : MonoBehaviour
{
    [SerializeField] UnityEvent wallpass;

    bool walltrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if ((!walltrigger))
        {
            wallpass.Invoke();
        } 
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
