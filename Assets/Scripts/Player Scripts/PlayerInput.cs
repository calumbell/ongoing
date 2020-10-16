using UnityEngine;

public class PlayerInput : MonoBehaviour
{

#region Class Fields

    [Header("Script Connections")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerPhysics physics;
    [SerializeField] PlayerAnimation animation;

    [Header("State Variables")]
    public bool inputEnabled;

    [Header("Events")]
    public VoidEvent onPlayerAttackEvent;
    public VoidEvent onPlayerInteractTriggerEvent;
    public VoidEvent onPlayerInteractCycleEvent;
    public InteractionEvent onPlayerInteractEvent;

#endregion

#region Unity Methods

    private void Awake()
    {
        if (stateMachine == null) gameObject.GetComponent<PlayerStateMachine>();
        if (physics == null) gameObject.GetComponent<PlayerPhysics>();
    }

    private void Update()
    {
        if (!inputEnabled)
            return;

        if (stateMachine.IsActionAvailable(PlayerStateMachine.PlayerAction.attack))
            HandleAttackInput();

        if (stateMachine.IsActionAvailable(PlayerStateMachine.PlayerAction.interact))
            HandleInteractInput();

        if (stateMachine.IsActionAvailable(PlayerStateMachine.PlayerAction.interactCycle))
            HandleInteractCycleInput();

        if (stateMachine.IsActionAvailable(PlayerStateMachine.PlayerAction.move))
            HandleMovementInput();

    }

#endregion

#region Private Methods

    private void HandleAttackInput()
    {
        if(Input.GetButtonDown("Attack"))
        {
            onPlayerAttackEvent.Raise();
        }
    }

    private void HandleInteractInput()
    {
        if(Input.GetButtonDown("Interact"))
        {
            // if carrying,the only interaction possible is to put down what we are carrying
            if (stateMachine.currentState == PlayerStateMachine.PlayerState.carrying)
            {
                Interaction interaction = new Interaction(InteractionType.PutDown, gameObject);
                onPlayerInteractEvent.Raise(interaction); // send to int. mngr, not Entity scrpt
            }

            else
            {
                onPlayerInteractTriggerEvent.Raise();
            }
        }
    }

    private void HandleInteractCycleInput()
    {
        if(Input.GetButtonDown("Interact Cycle"))
        {
            onPlayerInteractCycleEvent.Raise();
        }
    }

    private void HandleMovementInput()
    {
        Vector3 change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        animation.UpdateMovementAnimation(change);

        if (change != Vector3.zero)
        {
            physics.Move(change);
        }
    }

#endregion

}
