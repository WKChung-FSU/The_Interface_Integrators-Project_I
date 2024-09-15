using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextObject : MonoBehaviour
{
    BoxCollider m_collider;
    TMP_Text m_text;
    void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        m_text = GetComponent<TMP_Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            gameManager.instance.ToolTip(m_text);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            gameManager.instance.ClearToolTip();
    }
}
