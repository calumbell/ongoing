using UnityEngine;

public class ContextClue : MonoBehaviour
{
    [Header("Script References")]
    public SpriteRenderer thoughtBubble;

    [Header("Thought Bubble Sprites")]
    public Sprite interactBubble;
    public Sprite stairsUpBubble;
    public Sprite stairsDownBubble;

    [Header("State Variables")]
    public BoolValue highlightEnabled;


    public void OnInteractTriggerEnter(string type)
    {
        if (!highlightEnabled.value || type == "clear")
        {
            thoughtBubble.enabled = false;
            return;
        }

 
        thoughtBubble.enabled = true;


        if (type == "interact")
        {
            thoughtBubble.sprite = interactBubble;
        }

        else if (type == "stairs up")
        {
            thoughtBubble.sprite = stairsUpBubble;
        }

        else if (type == "stairs down")
        {
            thoughtBubble.sprite = stairsDownBubble;
        }


        if (type == "clear")
        {
            thoughtBubble.enabled = false;
        }

        else
        {
            thoughtBubble.enabled = true;
        }
    }

    public void OnInteractTriggerExit(string type)
    {
        thoughtBubble.enabled = false;
    }
}
