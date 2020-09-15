using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    [SerializeField] private IntEvent onStairsInteract;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("StairDown"))
        {
            onStairsInteract.Raise(0);
        }

        else if (other.CompareTag("StairUp"))
        {
            onStairsInteract.Raise(1);
        }

    }
}
