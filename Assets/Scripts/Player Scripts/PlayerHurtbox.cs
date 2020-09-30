using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    public PlayerControl playerControl;
    public Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Entity")
        {
            playerControl.Stagger(0.05f);
            rb.velocity = Vector2.zero;
            Vector2 difference = gameObject.transform.position - other.transform.position;
            difference = difference.normalized * 10;
            rb.AddForce(difference, ForceMode2D.Impulse);

            // stop the other object from being knocked back via physics engine
            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
