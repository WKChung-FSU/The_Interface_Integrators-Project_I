using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuforWebGLScript : MonoBehaviour
{
    [SerializeField] GameObject ExitButton;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        ExitButton.SetActive(false);
#endif
    }
}
