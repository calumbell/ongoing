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
        if (!target.gameObject.CompareTag("EntityHurtBox") && !target.gameObject.CompareTag("PlayerHurtBox"))
            return;

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
            Rigidbody2D target = targetCollider.GetComponentInParent<Rigidbody2D>();

            // make sure that the target has a RigidBody
            if (target == null) continue;


            // Stagger entity, and begin coroutine to end stagger
            if (target.gameObject.CompareTag("Entity"))
            {
                target.GetComponent<EntityBaseBehaviour>().Stagger(target, time.value);
            }

            else if (target.gameObject.CompareTag("Player"))
            {
                target.GetComponent<PlayerControl>().TakeDamage();
                target.GetComponent<PlayerControl>().Stagger(time.value);
            }

            target.velocity = Vector2.zero;
            Vector2 difference = target.transform.position - transform.position;
            difference = difference.normalized * force.value;
            target.AddForce(difference, ForceMode2D.Impulse);

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
