using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public Light2D light;

    private float initialOuterRadius;
    private float previousOuterRadius;
    private float targetOuterRadius;

    private float initialInnerRadius;
    private float t;

    public float flickerAmount;
    public float flickerInterval;

    private void Awake()
    {
        if (light == null)
        {
            light = gameObject.GetComponent<Light2D>();
        }

        initialOuterRadius = light.pointLightOuterRadius;
        t = flickerInterval;
        targetOuterRadius = initialOuterRadius;
    }

    void Update()
    {
        if (t >= flickerInterval)
        {
            t = 0;
            previousOuterRadius = targetOuterRadius;
            targetOuterRadius = initialOuterRadius + Random.Range(-flickerAmount, flickerAmount);
        }

        light.pointLightOuterRadius =  Mathf.Lerp(previousOuterRadius, targetOuterRadius, t / flickerInterval);

        t += Time.deltaTime;
    }
}
