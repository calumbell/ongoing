using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    private AudioManager audioManager;
    private bool footstepPlayingFlag;
    public float footstepInterval;
    public float footstepDelay;

    public AudioType[] footsteps;

    private int nextFootstepIndex;

    void Start()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        nextFootstepIndex = 0;
    }



    public void OnPlayerMove(Vector3 position)
    {
        if (footstepPlayingFlag == false)
        {
            footstepPlayingFlag = true;
            StartCoroutine(FootstepCoroutine(footstepInterval));
        }
    }

    private IEnumerator FootstepCoroutine(float interval)
    {
        audioManager.PlayAudio(footsteps[nextFootstepIndex++], false, footstepDelay);
        nextFootstepIndex = nextFootstepIndex >= footsteps.Length ? 0 : nextFootstepIndex;
        yield return new WaitForSeconds(interval);
        footstepPlayingFlag = false;
    }
}
