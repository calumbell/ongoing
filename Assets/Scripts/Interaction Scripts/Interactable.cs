using UnityEngine;

public class Interactable : MonoBehaviour
{

    [Header("State Variables")]
    public BoolValue highlightsEnabled;
    public BoolValue interactionAvailable;
    public string interactionType;

    [Header("Events")]
    public StringEvent onInteractTriggerEnter;
    public StringEvent onInteractTriggerExit;

    [Header("Script References")]
    Material material;

    private void Awake()
    {
        material = gameObject.GetComponent<SpriteRenderer>().material;
    }

    public virtual void OnInteract(GameObject interactor)
    {
        return;
    }

    public void Highlight(bool state)
    {
        if (state == false || highlightsEnabled.value == false)
            material.SetInt("_Intensity", 0);
        else
            material.SetFloat("_Intensity", 1.2f);
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
