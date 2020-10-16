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
}
