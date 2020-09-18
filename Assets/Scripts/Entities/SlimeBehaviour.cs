using UnityEngine;

public class SlimeBehaviour : EntityBaseBehaviour
{
    private Transform target;
    public float chaseRadius;
    public float minDistance;
    private Rigidbody2D rb;

    public bool movementEnabled;

    void Awake()
    {
        currentState = EntityState.idle;
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        movementEnabled = true;
    }

    void FixedUpdate()
    {
        CheckDistance();
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
}
