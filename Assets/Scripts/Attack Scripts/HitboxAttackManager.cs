using System.Collections.Generic;
using UnityEngine;

public class HitboxAttackManager : MonoBehaviour
{
    public FloatValue force;
    public FloatValue time;

    public List<Collider2D> targetsInRange;

    private void Awake()
    {
        targetsInRange = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.GetComponentInParent<IAttackable>() == null) return;

        if (!targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);
        }    
    }

    private void OnTriggerExit2D(Collider2D target)
    {
        if (targetsInRange.Contains(target))
        {
            targetsInRange.Remove(target);
        }
    }

    public void OnAttackTriggerReceived()
    {
        foreach (Collider2D targetCollider in targetsInRange)
        {
            targetCollider.GetComponentInParent<IAttackable>().OnAttack(transform.parent.gameObject, 1);
        }
    }


    public Collider2D GetPlayerCollider()
    {
        foreach (Collider2D target in targetsInRange)
        {
            if (target.gameObject.CompareTag("PlayerHurtBox"))
            {
                return target;
            }
        }

        return null;
    }

    public bool IsTargetInRange()
    {
        if (targetsInRange.Count > 0)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
