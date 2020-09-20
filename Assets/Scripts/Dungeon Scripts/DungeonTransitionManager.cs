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
            Destroy(panal, fadeTime);
        }
    }

    public void OnStairsInteractEventReceived(int input)
    {
        StartCoroutine(fadeCoroutine());
    }

    public IEnumerator fadeCoroutine()
    {
        GameObject fadeIn = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
        yield return new WaitForSeconds(fadeTime);
        GameObject fadeOut = Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity) as GameObject;
        Destroy(fadeIn);
        yield return new WaitForSeconds(fadeTime);
        Destroy(fadeOut);
    }
}
