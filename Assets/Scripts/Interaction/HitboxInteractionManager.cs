using UnityEngine;
using System.Collections.Generic;

public class HitboxInteractionManager : MonoBehaviour
{

    private List<Interactable> targetsInRange;
    private Interactable currentTarget;

    // Events
    public StringEvent updateContextClueBubble;

    private void OnEnable()
    {
        targetsInRange = new List<Interactable>();
    }

    private void OnDisable()
    {
        if (currentTarget != null)
        {
            currentTarget.DisableHighlight();
            currentTarget = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D targetCollider)
    {
        Interactable target = targetCollider.gameObject.GetComponent<Interactable>();

        // Make sure target is interactable and isn't already in list
        if (target != null
            && !targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);

            // Set target as currentTarget if we don't already have one
            if (currentTarget == null)
            {
                currentTarget = target;
                target.EnableHighlight();
                updateContextClueBubble.Raise(currentTarget.interactionType);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D targetCollider)
    {
        Interactable target = targetCollider.gameObject.GetComponent<Interactable>();


        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
        }

        if (currentTarget == target)
        {
            if (targetsInRange.Count == 0)
            {
                target.DisableHighlight();
                currentTarget = null;
            }

            else
            {
                currentTarget = targetsInRange[0];
                currentTarget.EnableHighlight();
            }

            UpdateContextClue();
        }
    }

    public void OnInteractCycle()
    {
        // if more than 1 target is in range, make the next target the current 
        if (targetsInRange.Count > 1)
        {
            int i = targetsInRange.IndexOf(currentTarget);

            // wrap around end of list
            i = i + 1 == targetsInRange.Count ? 0 : i + 1;
            currentTarget.DisableHighlight();
            currentTarget = targetsInRange[i];
            currentTarget.EnableHighlight();
            UpdateContextClue();
        }
    }

    public void OnInteractTriggered()
    {
        if (currentTarget != null)
        {
            currentTarget.DisableHighlight();
            currentTarget.OnInteract(gameObject.transform.parent.gameObject);
        }
    }

    public void UpdateContextClue()
    {
        if (currentTarget != null)
        {
            updateContextClueBubble.Raise(currentTarget.interactionType);
        }

        else
        {
            updateContextClueBubble.Raise("clear");
        }
    }

    public void OnTeleport(Vector3 position)
    {
        targetsInRange = new List<Interactable>();
        currentTarget = null;
        UpdateContextClue();
    }
}
