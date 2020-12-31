using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float smoothing;
    public float cameraZ;

    public void OnPlayerMove(Vector3 target)
    {
        target.z = cameraZ;
        transform.position = target;
    }

    public void OnPlayerTeleport(Vector3 target)
    {
        target.z = cameraZ;
        transform.position = target;
    }

    public void OnPlayerStagger(PushData _pushData)
    {
        Transform _target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(FollowCoroutine(_target, _pushData.time));
    }

    private IEnumerator FollowCoroutine(Transform _target, float _duration)
    {
        float _time = 0.0f;
        while (_time < _duration)
        {
            _time += Time.deltaTime;
            transform.position = new Vector3(
                _target.transform.position.x, _target.transform.position.y, transform.position.z);
        
            yield return null;
        }
    }
}
