using UnityEngine;

public class PotInteractable : Interactable
{
    public InteractionEvent onInteractionEvent;

    public override void OnInteract(GameObject interactor)
    {
        Interaction interaction = new Interaction(InteractionType.PickUp, interactor, gameObject);
        onInteractionEvent.Raise(interaction);
    }
}

