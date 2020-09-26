using UnityEngine;

public class PotInteractable : Interactable
{
    public override void OnInteract(GameObject interactor)
    {
        interactor.GetComponent<PlayerControl>().PickUp(gameObject);
    }
}

