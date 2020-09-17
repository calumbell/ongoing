using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityState
{
    idle,
    walk,
    attack,
    stagger
}

public class EntityBaseClass : MonoBehaviour
{
    public EntityState currentState;


    public string entityName;
    public float moveSpeed;


    public void ChangeState(EntityState state)
    {
        if (state != currentState)
            currentState = state;
    }
}
