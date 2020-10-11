using UnityEngine;

public class Interactable : MonoBehaviour
{
    public StringEvent onInteractTriggerEnter;
    public StringEvent onInteractTriggerExit;

    public BoolValue interactionAvailable;

    public string interactionType;

    Material material;

    private void Awake()
    {
        material = gameObject.GetComponent<SpriteRenderer>().material;

    }

    public virtual void OnInteract(GameObject interactor)
    {
        return;
    }

    public void EnableHighlight()
    {
        if (material != null)
        {
            material.SetFloat("_Intensity", 1.2f);
        }
    }

    public void DisableHighlight()
    {
        if (material != null)
        {
            material.SetInt("_Intensity", 0);
        }
    }
}
