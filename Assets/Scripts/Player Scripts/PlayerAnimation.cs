using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

#region Member Fields

    [Header("Script References")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerInput input;
    [SerializeField] public Animator animator;

#endregion

#region Public Methods

    public void UpdateMovementAnimation(Vector3 _input)
    {
        if (_input != Vector3.zero)
        {
            animator.SetFloat("moveX", _input.x);
            animator.SetFloat("moveY", _input.y);
            animator.SetBool("moving", true);
        }

        else
        {
            animator.SetBool("moving", false);
        }
    }

#endregion


#region Public Methods (Event Handlers)

    public void OnPlayerAttackEvent()
    {
        StartCoroutine(AttackCoroutine(0.3f));
    }

    public void OnPlayerStaggerEvent(PushData _push)
    {
        StartCoroutine(StaggerCoroutine(_push.time));
    }

#endregion

#region Private Methods (Coroutines)

    private IEnumerator AttackCoroutine(float _time)
    {
        animator.SetBool("attacking", true);
        yield return new WaitForSeconds(_time);
        animator.SetBool("attacking", false);
    }

    private IEnumerator StaggerCoroutine(float _time)
    {
        animator.SetBool("hurt", true);
        yield return new WaitForSeconds(_time);
        animator.SetBool("hurt", false);
    }

#endregion

}
