using UnityEngine;

public class PotInteractable : Interactable
{
    public override void OnInteract()
    {
        Debug.Log("Looks like a pot to me, boss");
    }
}

