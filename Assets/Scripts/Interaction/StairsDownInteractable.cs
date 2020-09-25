using UnityEngine;

public class StairsDownInteractable : Interactable
{
    public IntEvent onStairsInteract;

    public override void OnInteract()
    {
        onStairsInteract.Raise(0);
    }
}

