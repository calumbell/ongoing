using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio")]
    public AudioType[] footsteps;

    [Header("Events")]
    public AudioDataEvent audioEvent;

    private bool footstepPlayingFlag = false;
    private int lastFootstep = 0;

#region Event Handlers

    public void OnPlayerAttackEventRaised()
    {
        AudioData audio = new AudioData(AudioType.SFX_woof, AudioAction.START);
        audioEvent.Raise(audio);
    }

    public void OnPlayerMoveEventRaised(Vector3 _input)
    {
        if (footstepPlayingFlag == false)
            StartCoroutine(FootstepCoroutine(0.4f));
    }

#endregion

    private IEnumerator FootstepCoroutine(float _interval)
    {
        footstepPlayingFlag = true;

        int nextFootstep = Random.Range(0, footsteps.Length);

        while (lastFootstep == nextFootstep) nextFootstep = Random.Range(0, footsteps.Length);

        AudioData audio = new AudioData(footsteps[nextFootstep], AudioAction.START);

        audioEvent.Raise(audio);
        yield return new WaitForSeconds(_interval);
        footstepPlayingFlag = false;
    }
}
