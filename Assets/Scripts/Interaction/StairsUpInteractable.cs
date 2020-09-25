using UnityEngine;

public class StairsUpInteractable : Interactable
{
    public IntEvent onStairsInteract;

    public override void OnInteract()
    {
        onStairsInteract.Raise(1);
    }
}

