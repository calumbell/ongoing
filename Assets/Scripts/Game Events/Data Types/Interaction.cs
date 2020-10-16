using UnityEngine;

[System.Serializable]
public struct Interaction
{
    public InteractionType type;
    public GameObject interactor;
    public GameObject target;

    public Interaction(InteractionType _type, GameObject _interactor, GameObject _target = null)
    {
        type = _type;
        interactor = _interactor;
        target = _target;
    }
}