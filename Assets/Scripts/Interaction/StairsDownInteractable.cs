using UnityEngine;

public class StairsDownInteractable : Interactable
{
    public IntEvent onStairsInteract;

    public override void OnInteract(GameObject interactor)
    {
        onStairsInteract.Raise(0);
    }
}

