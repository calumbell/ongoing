using System.Collections;
using UnityEngine;

public enum EntityState
{
    idle,
    walk,
    attack,
    stagger
}

public class EntityBaseBehaviour : MonoBehaviour
{
    public EntityState currentState;

    public string entityName;
    public float moveSpeed;


    public void ChangeState(EntityState state)
    {
        if (state != currentState)
            currentState = state;
    }

    /*
     * Stagger & StaggerCoroutine
     * Staggers this entity for time seconds, then unstaggers them
     */

    public void Stagger(Rigidbody2D rb, float time)
    {
        if (rb == null || currentState == EntityState.stagger) return;

        ChangeState(EntityState.stagger);
        StartCoroutine(StaggerCoroutine(rb, time));
    }

    private IEnumerator StaggerCoroutine(Rigidbody2D rb, float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector2.zero;
        ChangeState(EntityState.idle);
    }
}
