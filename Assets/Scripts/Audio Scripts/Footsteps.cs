using System.Collections;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    private AudioManager audioManager;
    private bool footstepPlayingFlag;
    public float footstepInterval;
    public float footstepDelay;

    public AudioType[] footsteps;

    private int lastFootstepIndex;
    private int nextFootstepIndex;

    void Start()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        lastFootstepIndex = 0;
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
        nextFootstepIndex = Random.Range(0, footsteps.Length);

        if (nextFootstepIndex == lastFootstepIndex)
        {
            nextFootstepIndex = (nextFootstepIndex + 1) % (footsteps.Length - 1);
        }

        audioManager.PlayAudio(footsteps[nextFootstepIndex], false, footstepDelay);

        lastFootstepIndex = nextFootstepIndex;

        yield return new WaitForSeconds(interval);
        footstepPlayingFlag = false;
    }
}
