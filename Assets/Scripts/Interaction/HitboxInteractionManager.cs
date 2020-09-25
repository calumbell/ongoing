using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HitboxInteractionManager : MonoBehaviour
{

    public List<Interactable> targetsInRange;
    public Interactable currentTarget;

    public StringEvent updateContextClueBubble;

    void Start()
    {
        targetsInRange = new List<Interactable>();
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
                currentTarget = null;
            }

            else
            {
                currentTarget = targetsInRange[0];
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
            currentTarget = targetsInRange[i];
            UpdateContextClue();
        }
    }

    public void OnInteractTriggered()
    {
        if (currentTarget != null)
        {
            currentTarget.OnInteract();
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
}
