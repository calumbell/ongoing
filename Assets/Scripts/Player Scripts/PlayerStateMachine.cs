using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateMachine : MonoBehaviour
{

    public enum PlayerState
    {
        idle,
        walking,
        interacting,
        attacking,
        carrying,
        stagger
    }

    public enum PlayerAction
    {
        move,
        attack,
        interact,
        interactCycle
    }

    [Header("State Variables")]
    public PlayerState currentState;

    // Perhaps these belong somewhere else?
    [Header("State Dependant Scriptable Objects")]
    public BoolValue objectHighlightEnabled;


    /* indexing into this dict with the current state will return the PlayerActions
     * that are available in that state. Maybe this belongs somewhere else?
     */

    private Dictionary<PlayerState, PlayerAction[]> actionsPerState;

#region Unity Functions

    private void Awake()
    {
        // THIS BELONGS IN A CONFIG!!
        Application.targetFrameRate = 60;
        // make a config sometime why don't you?


        currentState = PlayerState.idle;
        actionsPerState = GenerateActionsPerStateDict();
    }

#endregion


#region Public Functions

    public void ChangeState(PlayerState _state)
    {
        currentState = _state;

        /* Updating state dependant scriptable objects belongs somewhere else.
         * This will look pretty silly if it gets any bigger. Perhaps its own method?
         */

        if (currentState == PlayerState.carrying) objectHighlightEnabled.value = false;
        else objectHighlightEnabled.value = true;
    }


    public bool IsActionAvailable(PlayerAction _action)
    {
        foreach (PlayerAction action in actionsPerState[currentState])
        {
            if (action == _action) return true;
        }

        return false;
    }

#endregion

#region Public Methods (Event Handlers)

    public void OnPlayerStagger(PushData _push)
    {
        StartCoroutine(ChangeStateCoroutine(PlayerState.stagger, _push.time));
    }

    public void OnPlayerAttack()
    {
        StartCoroutine(ChangeStateCoroutine(PlayerState.attacking, 0.3f));
    }

    public void OnPlayerPickup()
    {
        ChangeState(PlayerState.carrying);
    }

#endregion

#region Private Methods (Coroutines)

    private IEnumerator ChangeStateCoroutine(PlayerState _state, float _time)
    {
        ChangeState(_state);
        yield return new WaitForSeconds(_time);
        ChangeState(PlayerState.idle);
    }

#endregion

#region Private Methods

    private Dictionary<PlayerState, PlayerAction[]> GenerateActionsPerStateDict()
    {
        Dictionary<PlayerState, PlayerAction[]> dict = new Dictionary<PlayerState, PlayerAction[]>()
        {
            {
                PlayerState.idle, new PlayerAction[]
                { PlayerAction.move, PlayerAction.attack, PlayerAction.interact, PlayerAction.interactCycle }
            },

            {
                PlayerState.walking, new PlayerAction[]
                { PlayerAction.move, PlayerAction.attack, PlayerAction.interact, PlayerAction.interactCycle }
            },

            {
                PlayerState.interacting, new PlayerAction[]
                { }
            },

            {
                PlayerState.attacking, new PlayerAction[]
                { }
            },

            {
                PlayerState.carrying, new PlayerAction[]
                { PlayerAction.move, PlayerAction.interact }
            },

            {
                PlayerState.stagger, new PlayerAction[]
                { }
            }

        };

        return dict;
    }

#endregion

}
