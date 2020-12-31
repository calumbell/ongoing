using System.Collections;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour, IPhysics
{

#region Member Fields
    [Header("Script References")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerInput input;
    [SerializeField] Rigidbody2D rb;


    [Header("State Variables")]
    public FloatValue movementSpeed;

    [Header("Events")]
    public Vector3Event onPlayerMoveEvent;
    public Vector3Event onPlayerTeleportEvent;

#endregion

#region Unity Methods

    // Raise a tp event in Start to center camera AFTER it has been instantiated
    private void Start()
    {
        onPlayerTeleportEvent.Raise(rb.position);
    }

#endregion

#region Public Methods (implementing IPhysics)

    public void Push(Vector2 _force, float _time)
    {
        Stop();
        StartCoroutine(PushCoroutine(_force, _time));
    }

    public void Move(Vector3 _input)
    {
        rb.MovePosition(
            transform.position + _input.normalized * movementSpeed.value * Time.deltaTime);
        onPlayerMoveEvent.Raise(rb.position);
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    public void Teleport(Vector3 _input)
    {
        _input.z = transform.position.z;
        rb.position = _input;
        onPlayerTeleportEvent.Raise(rb.position);
    }

#endregion

#region Public Methods (Event Handlers)

    public void OnPlayerStaggerEvent(PushData _push)
    {
        StartCoroutine(PushCoroutine(_push.force, _push.time));
    }

    #endregion

#region Private Methods (Coroutines)

    private IEnumerator PushCoroutine(Vector2 _force, float _time)
    {
        rb.AddForce(_force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_time);
        rb.velocity = Vector2.zero;
    }

#endregion

}
