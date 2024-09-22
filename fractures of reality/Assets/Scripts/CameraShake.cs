using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] float intensity = 0.2f;

    private Vector3 originalPos;
    private float timeAtCurrentFrame;
    private float timeAtLastFrame;
    private float timeDelta;

    static bool isShaking = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

void Update()
    {
        // Calculate a fake delta time, so we can Shake while game is paused.
        timeAtCurrentFrame = Time.realtimeSinceStartup;
        timeDelta = timeAtCurrentFrame - timeAtLastFrame;
        timeAtLastFrame = timeAtCurrentFrame;
    }

    public static void Shake(float duration)
    {
        if(isShaking == true)
        {
            return;
        }
        else 
        {
            isShaking = true;
            instance.originalPos = instance.gameObject.transform.localPosition;
            //instance.StopAllCoroutines();
            instance.StartCoroutine(instance.cShake(duration));
            isShaking = false;
        }
       
    }

    public IEnumerator cShake(float duration)
    {
        float endTime = Time.time + duration;

        while (duration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * intensity;

            duration -= timeDelta;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

}
