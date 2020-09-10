using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    [SerializeField] private VoidEvent onStairsInteract;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("StairDown"))
        {
            onStairsInteract.Raise();
        }
    }
}
