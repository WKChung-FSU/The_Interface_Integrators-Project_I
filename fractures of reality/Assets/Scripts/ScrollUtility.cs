using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUtility : MonoBehaviour
{
    [SerializeField] float scrollSpeedX,scrollSpeedY;
    [SerializeField] Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX=Time.time*scrollSpeedX;
        float offsetY=Time.time*scrollSpeedY;

        rend.material.mainTextureOffset = new Vector2(offsetX,offsetY);
    }
}
