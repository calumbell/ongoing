using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHurtbox : MonoBehaviour, IAttackable
{
#region Member Fields

    [Header("State Variables")]
    public IntValue playerHealth;

    [Header("Events")]
    public PushDataEvent onPlayerStagger;
    public VoidEvent onPlayerHealthChangeEvent;

#endregion

#region Public Methods (Interfaces)

    public void OnAttack(GameObject _attacker, int _damage)
    {
        Vector2 difference = gameObject.transform.parent.position - _attacker.transform.position;
        difference = difference.normalized * 10;
        PushData push = new PushData(difference, 0.5f);
        onPlayerStagger.Raise(push);
        if (_damage > 0) OnDamage(_damage);
        
    }

#endregion

    public void OnDamage(int _damage)
    {
        playerHealth.value -= _damage;
        if (playerHealth.value <= 0) SceneManager.LoadScene("End");
        onPlayerHealthChangeEvent.Raise();
    }

#region Unity Methods

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Entity")
        {
            Vector2 difference = gameObject.transform.position - other.transform.position;
            difference = difference.normalized * 10;

            PushData push = new PushData(difference, 0.3f);

            onPlayerStagger.Raise(push);

            other.gameObject.GetComponent<IPhysics>().Stop();
        }
    }

#endregion

}
