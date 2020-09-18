using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float smoothing;

    public void OnPlayerMove(Vector3 targetPosition)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
    }

}
