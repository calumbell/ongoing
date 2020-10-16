using UnityEngine;

[System.Serializable]
public struct AudioData
{
    public AudioType type;
    public AudioAction action;
    public bool fade;
    public float delay;

    public AudioData(AudioType _type, AudioAction _action, bool _fade = false, float _delay = 0.0f)
    {
        type = _type;
        action = _action;
        fade = _fade;
        delay = _delay;
    }
}
