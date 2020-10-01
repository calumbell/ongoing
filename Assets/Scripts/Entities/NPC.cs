using System.Collections;
using UnityEngine;

public enum State
{
    idle,
    walk,
    attack,
    stagger
}

public class NPC : Entity
{
    public State currentState;

    public float moveSpeed;


    public void SetId(int n) { id = n; }
    public int GetId() { return id; }
    public Transform GetTransform() { return transform; }

    public NPC(GameObject prefabInput, Vector3 locationInput, int n) : base(prefabInput, locationInput, n)
    { 
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }

    public void ChangeState(State state)
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
        if (rb == null || currentState == State.stagger) return;

        ChangeState(State.stagger);
        StartCoroutine(StaggerCoroutine(rb, time));
    }

    private IEnumerator StaggerCoroutine(Rigidbody2D rb, float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector2.zero;
        ChangeState(State.idle);
    }
}
