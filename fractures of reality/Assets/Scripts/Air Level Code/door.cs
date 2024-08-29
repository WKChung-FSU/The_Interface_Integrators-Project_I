using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    public float interactDist;
    public GameObject intText;
    public string openAnimName, closeAnimName;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, interactDist))
        {
            if(hit.collider.gameObject.tag ==  "door")
            {
                GameObject doorParent = hit.collider.gameObject.transform.root.gameObject;
                Animator doorAnim = doorParent.GetComponent<Animator>();
                intText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(openAnimName))
                    {
                        doorAnim.ResetTrigger("Open");
                        doorAnim.SetTrigger("Close");
                    }

                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(openAnimName))
                    {
                        doorAnim.ResetTrigger("Close");
                        doorAnim.SetTrigger("Open");
                    }
                }
            }

            else
            {
                intText.SetActive(false);
            }
        }
    }
}
