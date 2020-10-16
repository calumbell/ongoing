using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviour
{
    [Header("Script References")]
    public Rigidbody2D rb;

    [Header("Events")]
    public PushDataEvent onPlayerStagger;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Entity")
        {
            Vector2 difference = gameObject.transform.position - other.transform.position;
            difference = difference.normalized * 10;

            PushData push = new PushData(difference, 0.3f);

            onPlayerStagger.Raise(push);

            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
