using UnityEngine;
using System.Collections;

public class SlimeBehaviour : EntityBaseBehaviour
{
    private Transform target;
    private Rigidbody2D rb;

    public float chaseRadius;
    public float minDistance;

    public float attackTime;
    public float attackCooldown;

    public bool movementEnabled;

    public HitboxAttackManager[] hitboxes;

    public ProgressBar progressBar;

    void Awake()
    {
        currentState = EntityState.idle;
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        movementEnabled = true;
    }

    void Update()
    {
        if (currentState != EntityState.attack && currentState != EntityState.stagger)
        { 
            CheckDistance();
            CheckForAttack();
        }
    }

    void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) < chaseRadius
            && Vector3.Distance(target.position, transform.position) > minDistance
            && (currentState == EntityState.idle || currentState == EntityState.walk))
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.position,
                moveSpeed * Time.deltaTime);

            rb.MovePosition(newPos);
            ChangeState(EntityState.walk);
        }
    }

    void CheckForAttack()
    {
        Collider2D target = null;

        foreach (HitboxAttackManager hitbox in hitboxes)
        {
            target = hitbox.GetPlayerCollider();
            if (target != null) break;
        }

        if (target == null) return;

        StartCoroutine(AttackCoroutine());
    }

    public IEnumerator AttackCoroutine()
    {
        progressBar.Begin(attackTime);

        ChangeState(EntityState.attack);

        yield return new WaitForSeconds(attackTime);

        foreach (HitboxAttackManager hitbox in hitboxes)
        {
            hitbox.OnAttackTriggerReceived();
        }

        ChangeState(EntityState.stagger);
        yield return new WaitForSeconds(attackCooldown);
        ChangeState(EntityState.idle);
    }
}
