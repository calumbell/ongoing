using UnityEngine;
using System.Collections.Generic;

public class HitboxInteractionManager : MonoBehaviour
{
    private List<Interactable> targetsInRange;
    private Interactable currentTarget;

    // Events
    public StringEvent updateContextClueBubble;


#region Unity Functions

    private void OnEnable()
    {
        targetsInRange = new List<Interactable>();
    }

    private void OnDisable()
    {
        foreach(Interactable target in targetsInRange)
        {
            target.Highlight(false);
        }

        currentTarget = null;
    }


    private void OnTriggerEnter2D(Collider2D targetCollider)
    {
        Interactable target = targetCollider.gameObject.GetComponent<Interactable>();

        // Make sure target is interactable and isn't already in list
        if (target != null && !targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);

            // Set target as currentTarget if we don't already have one
            if (currentTarget == null)
            {
                SetTarget(target);
                updateContextClueBubble.Raise(currentTarget.interactionType);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D targetCollider)
    {
        Interactable target = targetCollider.gameObject.GetComponent<Interactable>();

        if (target == null) return;
        target.Highlight(false);

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
                SetTarget(targetsInRange[0]);
            }

            UpdateContextClue();
        }
    }

#endregion


#region Public Methods

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

#endregion

#region Public Methods (Event Handlers)

    public void OnInteractCycle()
    {
        // if more than 1 target is in range, make the next target the current 
        if (targetsInRange.Count > 1)
        {
            int i = targetsInRange.IndexOf(currentTarget);

            // wrap around end of list
            i = i + 1 == targetsInRange.Count ? 0 : i + 1;
            currentTarget.Highlight(false);
            currentTarget = targetsInRange[i];
            currentTarget.Highlight(true);
            UpdateContextClue();
        }
    }

    public void OnInteractTriggered()
    {
        if (currentTarget != null)
        {
            currentTarget.Highlight(false);
            currentTarget.OnInteract(gameObject.transform.parent.gameObject);
        }
    }

    public void OnTeleport(Vector3 position)
    {
        targetsInRange = new List<Interactable>();
        SetTarget(null);
        UpdateContextClue();
    }

#endregion

#region Private Methods

    private void SetTarget(Interactable target)
    {
        if (currentTarget != null) currentTarget.Highlight(false);
        currentTarget = target;
        if (currentTarget != null) currentTarget.Highlight(true);
    }

#endregion
}
