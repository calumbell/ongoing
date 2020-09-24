using UnityEngine;

public class StairsUpInteractable : Interactable
{
    public IntEvent onStairsInteract;

    public override void OnInteract()
    {
        Debug.Log("Looks like a stairs up to me, boss");
        onStairsInteract.Raise(1);
    }
}

