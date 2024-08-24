using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AlwaysFaceCamera : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] bool targetPlayerBody;
    [SerializeField] bool invert;

    private void Start()
    {
        if (targetPlayerBody == false)
            target = GameObject.FindWithTag("MainCamera");
        else if (target == null)
            target = GameObject.FindWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
        if (invert)
        {
            transform.LookAt(target.transform.position, target.transform.rotation * Vector3.up);
        }
        else
        {
            transform.LookAt(transform.position + target.transform.rotation * Vector3.forward, target.transform.rotation * Vector3.up);
        }
    }
}
