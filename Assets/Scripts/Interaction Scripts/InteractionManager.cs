using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Events")]
    public VoidEvent onPlayerPickupEvent;

#region Public Methods (Event Handlers)

    public void OnInteractionEventRaised(Interaction _interaction)
    {
        InteractionType type = _interaction.type;

        switch(type)
        {
            case InteractionType.GoToNextFloor:
                NextFloor();
                break;

            case InteractionType.GoToPreviousFloor:
                PreviousFloor();
                break;

            case InteractionType.PickUp:
                PickUp(_interaction.interactor, _interaction.target);
                break;

            case InteractionType.PutDown:
                PutDown(_interaction.interactor);
                break;
        }
    }

#endregion

#region Private Methods

    private void NextFloor()
    {
        // TODO
    }

    private void PickUp(GameObject _subject, GameObject _object)
    {
        _object.GetComponent<Collider2D>().enabled = false;
        _object.transform.parent = _subject.transform;

        _object.transform.position = new Vector3(
            _subject.transform.position.x,
            _subject.transform.position.y + 0.75f,
            _subject.transform.position.z);

        if (_subject.CompareTag("Player"))
        {
            onPlayerPickupEvent.Raise();
        }
    }

    private void PutDown(GameObject _subject)
    {
        GameObject _carried = _subject.GetComponentInChildren<Object>().gameObject;

        Vector3 originalPosition = _carried.transform.position;

        _carried.transform.position = new Vector3(
                _carried.transform.position.x + _subject.GetComponent<Animator>().GetFloat("moveX"),
                _carried.transform.position.y + _subject.GetComponent<Animator>().GetFloat("moveY") - 0.75f,
                _carried.transform.position.z + 1);

        _carried.GetComponent<Collider2D>().enabled = true;

        // Perhaps an entities list can be my an EntityManager class? or a Scriptable Object?
        Entity[] entities = GameObject.FindObjectsOfType<Entity>();

        // if new position is invalid, return
        if (_carried.GetComponent<Entity>().IsTouchingAnotherEntity(entities) ||
            _carried.GetComponent<Entity>().IsTouchingWalls())
        {
            _carried.GetComponent<BoxCollider2D>().enabled = false;
            _carried.transform.position = originalPosition;
            return;
        }

        

        GameObject entityObjectInHierarchy = GameObject.FindGameObjectWithTag("EntitiesList");
        if (entityObjectInHierarchy != null)  _carried.transform.parent = entityObjectInHierarchy.transform;
        else _carried.transform.parent = null;

        if (_subject.CompareTag("Player"))
        {
            _subject.GetComponent<PlayerStateMachine>().ChangeState(
                PlayerStateMachine.PlayerState.idle);
        }

    }

    private void PreviousFloor()
    {
        // TODO
    }

#endregion
}
