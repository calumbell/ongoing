using UnityEngine;

public interface IPhysics
{
    void Push(Vector2 _force, float _time);
    void Move(Vector3 _input);
    void Stop();
    void Teleport(Vector3 _input);
}
