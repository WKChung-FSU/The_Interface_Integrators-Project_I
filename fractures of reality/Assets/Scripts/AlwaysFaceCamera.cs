using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AlwaysFaceCamera : MonoBehaviour
{
    GameObject mainCamera;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
