using UnityEngine;

public class StairsDownInteractable : Interactable
{
    public IntEvent onStairsInteract;

    public override void OnInteract()
    {
        Debug.Log("Looks like a stairs down to me, boss");
        onStairsInteract.Raise(0);
    }
}

