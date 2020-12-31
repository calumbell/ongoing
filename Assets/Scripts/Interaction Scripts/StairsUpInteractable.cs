using UnityEngine;

public class StairsUpInteractable : Interactable
{
    public IntEvent onStairsInteract;

    public override void OnInteract(GameObject interactor)
    {
        onStairsInteract.Raise(1);
    }
}

