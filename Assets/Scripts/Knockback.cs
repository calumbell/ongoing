using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public FloatValue force;
    public FloatValue time;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only entities and players can be knocked back
        if (!(other.gameObject.CompareTag("Entity") || other.gameObject.CompareTag("Player")))
            return;
        
        Rigidbody2D target = other.GetComponent<Rigidbody2D>();

        // make sure that the target has a RigidBody
        if (target == null) return;

        target.GetComponent<EntityBaseClass>().currentState = EntityState.stagger;

        target.velocity = Vector2.zero;
        Vector2 difference = target.transform.position - transform.position;
        difference = difference.normalized * force.value;
        target.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCoroutine(target));
        
    }

    private IEnumerator KnockbackCoroutine(Rigidbody2D rb)
    {
        if (rb != null)
        {
            yield return new WaitForSeconds(time.value);
            rb.velocity = Vector2.zero;
            rb.GetComponent<EntityBaseClass>().currentState = EntityState.idle;
        }
    }
}
