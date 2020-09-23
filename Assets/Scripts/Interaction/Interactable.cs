using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public StringEvent onInteractTriggerEnter;
    public StringEvent onInteractTriggerExit;

    public BoolValue interactionAvailable;

    public string interactionType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        interactionAvailable.value = true;
        onInteractTriggerEnter.Raise(interactionType);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        interactionAvailable.value = false;
        onInteractTriggerExit.Raise(interactionType);
    }

    public void OnInteract()
    {

    }
}
