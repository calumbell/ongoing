using UnityEngine;
using System.Collections;

public class Slime : NPC
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



    public Slime(GameObject prefabInput, Vector3 locationInput, int n) : base(prefabInput, locationInput, n)
    {
        prefab = prefabInput;
        id = n;
        location = locationInput;
    }


    void Awake()
    {
        currentState = State.idle;
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        movementEnabled = true;
    }

    void Update()
    {
        if (currentState != State.attack && currentState != State.stagger)
        { 
            CheckDistance();
            CheckForAttack();
        }
    }

    void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) < chaseRadius
            && Vector3.Distance(target.position, transform.position) > minDistance
            && (currentState == State.idle || currentState == State.walk))
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target.position,
                moveSpeed * Time.deltaTime);

            rb.MovePosition(newPos);
            ChangeState(State.walk);
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

        ChangeState(State.attack);

        yield return new WaitForSeconds(attackTime);

        foreach (HitboxAttackManager hitbox in hitboxes)
        {
            hitbox.OnAttackTriggerReceived();
        }

        ChangeState(State.stagger);
        yield return new WaitForSeconds(attackCooldown);
        ChangeState(State.idle);
    }
}
