using UnityEngine;

[System.Serializable] public struct PushData
{
    public float time;
    public Vector2 force;

    public PushData(Vector2 _force, float _time)
    {
        time = _time;
        force = _force;
    }
}
