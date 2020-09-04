using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float speed;

    private float verticalInput;
    private float horizontalInput;
    private Rigidbody2D rb;
    private Vector3 change;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        change = Vector3.zero;
        change.x = Input.GetAxis("Horizontal");
        change.y = Input.GetAxis("Vertical");

        if (change != Vector3.zero)
            MoveCharacter(change);

    }

    void MoveCharacter(Vector3 change) {
        rb.MovePosition(
            transform.position + change.normalized * speed * Time.deltaTime);
    }
}
