using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextClue : MonoBehaviour
{
    public SpriteRenderer thoughtBubble;

    public void OnInteractTriggerEnter(string type)
    {
        thoughtBubble.enabled = true;
    }

    public void OnInteractTriggerExit(string type)
    {
        thoughtBubble.enabled = false;
    }
}
