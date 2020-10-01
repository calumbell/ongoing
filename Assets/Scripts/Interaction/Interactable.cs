using UnityEngine;

public class Interactable : MonoBehaviour
{
    public StringEvent onInteractTriggerEnter;
    public StringEvent onInteractTriggerExit;

    public BoolValue interactionAvailable;

    public string interactionType;

    public virtual void OnInteract(GameObject interactor)
    {
        return;
    }
}
