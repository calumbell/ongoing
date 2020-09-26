using UnityEngine;

public class Interactable : MonoBehaviour
{
    public StringEvent onInteractTriggerEnter;
    public StringEvent onInteractTriggerExit;

    public BoolValue interactionAvailable;

    public string interactionType;

    /* Refactored elsewhere, but maybe useful in the future!
        
    private void OnTriggerEnter2D(Collider2D other)
    {

    }

    private void OnTriggerExit2D(Collider2D other)
    {

    }
    */

    public virtual void OnInteract(GameObject interactor)
    {
        return;
    }
}
