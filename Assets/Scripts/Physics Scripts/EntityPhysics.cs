using System.Collections;
using UnityEngine;

public class EntityPhysics : MonoBehaviour, IPhysics
{

#region Member Fields

    [Header("Script References")]
    [SerializeField] Rigidbody2D rb;
    // TODO [SerializedField] EntityStateMachine stateMachine
    // TODO input

    [Header("State Variables")]
    public FloatValue movementSpeed;

#endregion

#region Public Methods (IPhysics)

    public void Push(Vector2 _force, float _time)
    {
        StartCoroutine(PushCoroutine(_force, _time));
    }

    public void Move(Vector3 _input)
    {
        rb.MovePosition(
            transform.position + _input.normalized * movementSpeed.value * Time.deltaTime);
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    public void Teleport(Vector3 _input)
    {
        _input.z = transform.position.z;
        rb.position = _input;
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
