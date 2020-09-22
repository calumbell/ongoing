using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    [SerializeField] private IntEvent onStairsInteract;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Generate a UI element with contextual interact options for object
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("Interaction Triggered");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove UI elements for this objects interactions
    }


    /*
     * Stairs interaction code, refactor this as part of the interaction
     * update!
     * 
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
    */
}
