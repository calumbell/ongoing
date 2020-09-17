using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float force;
    public float time;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Entity"))
        {
            Rigidbody2D target = other.GetComponent<Rigidbody2D>();
            if (target == null) return;

            target.GetComponent<EntityBaseClass>().currentState = EntityState.stagger;

            target.velocity = Vector2.zero;
            Vector2 difference = target.transform.position - transform.position;
            difference = difference.normalized * force;
            target.AddForce(difference, ForceMode2D.Impulse);
            StartCoroutine(KnockbackCoroutine(target));
        }
    }

    private IEnumerator KnockbackCoroutine(Rigidbody2D rb)
    {
        if (rb != null)
        {
            yield return new WaitForSeconds(time);
            rb.velocity = Vector2.zero;
            rb.GetComponent<EntityBaseClass>().currentState = EntityState.idle;
        }
    }
}
