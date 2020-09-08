using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("StairDown")) {
            Debug.Log("These stairs are going down!");
        }
    }
}
