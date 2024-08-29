using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour
{

    [SerializeField] float scrollSpeedX;
    [SerializeField] float scrollSpeedY;
    [SerializeField] Renderer renderer;


    private void Start()
    {
        renderer = transform.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = scrollSpeedX * Time.time;
        float offsetY = scrollSpeedY * Time.time;

        renderer.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
