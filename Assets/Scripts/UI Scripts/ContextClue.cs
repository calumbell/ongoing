using UnityEngine;

public class ContextClue : MonoBehaviour
{
    public SpriteRenderer thoughtBubble;

    public Sprite interactBubble;
    public Sprite stairsUpBubble;
    public Sprite stairsDownBubble;

    public void OnInteractTriggerEnter(string type)
    {
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
