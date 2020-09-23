using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public FloatValue force;
    public FloatValue time;

    private List<Collider2D> targetsInRange;

    private void Awake()
    {
        targetsInRange = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        

        if (!(target.gameObject.CompareTag("Entity") || target.gameObject.CompareTag("Player")))
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
            Rigidbody2D target = targetCollider.GetComponent<Rigidbody2D>();

            // make sure that the target has a RigidBody
            if (target == null) continue;

            // Stagger entity, and begin coroutine to end stagger
            if (targetCollider.gameObject.CompareTag("Entity"))
                target.GetComponent<EntityBaseBehaviour>().Stagger(target, time.value);

            else if (targetCollider.gameObject.CompareTag("Player") && targetCollider.isTrigger == true)
            {
                target.GetComponent<PlayerControl>().Stagger(time.value);
            }

            target.velocity = Vector2.zero;
            Vector2 difference = target.transform.position - transform.position;
            difference = difference.normalized * force.value;
            target.AddForce(difference, ForceMode2D.Impulse);

        }
    }
}
