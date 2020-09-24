using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitboxInteractionManager : MonoBehaviour
{

    private List<Interactable> targetsInRange;
    private Interactable currentTarget;

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
        }
    }

    public void OnInteractSwitch()
    {
        // if more than 1 target is in range, make the next target the current 
        if (targetsInRange.Count > 1)
        {
            int i = targetsInRange.IndexOf(currentTarget);
            currentTarget = targetsInRange[(i % targetsInRange.Count) - 1];
        }
    }

    public void OnInteractTriggered()
    {
        if (currentTarget != null)
        {
            currentTarget.OnInteract();
        }
    }

}
