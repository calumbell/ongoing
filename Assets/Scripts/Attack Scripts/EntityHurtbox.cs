using UnityEngine;

public class EntityHurtbox : MonoBehaviour, IAttackable
{

#region Member Fields

    [Header("Script References")]
    public EntityPhysics physics;

#endregion

#region Public Methods (implementing IAttackable)

    public void OnAttack(GameObject _attacker, int _damage)
    {
        Vector2 difference = gameObject.transform.parent.position - _attacker.transform.position;
        difference = difference.normalized * 15;
        physics.Push(difference, 0.5f);
    }

#endregion
}
