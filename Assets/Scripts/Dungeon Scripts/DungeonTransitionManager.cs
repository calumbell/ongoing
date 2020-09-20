using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTransitionManager : MonoBehaviour
{
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float fadeTime;

    private void Awake()
    {
        if (fadeInPanel != null)
        {
            GameObject panal = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
            Destroy(panal, 1);
        }
    }
}
