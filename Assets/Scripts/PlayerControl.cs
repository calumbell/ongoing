using System.Collections;
using UnityEngine;

public enum PlayerState
{
    idle,
    walk,
    interact,
    stagger
}

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public PlayerState currentState;

    public BoolValue inputEnabled;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private Vector3 change;

    private GameObject dungeonObj;
    private Dungeon dungeon;

    public Vector3Event onPlayerMoveEvent;

    void Awake()
    {
        currentState = PlayerState.walk;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // get references to dungeon
        dungeonObj = GameObject.FindGameObjectWithTag("DungeonManager");
        dungeon = dungeonObj.GetComponentInChildren<DungeonManager>().dungeon;
        Room startRoom = dungeon.getStartRoom();
        Teleport(4 * (startRoom.getX() + (startRoom.getWidth() / 2)) + 1,
            4 * (startRoom.getY() + (startRoom.getHeight() / 2)) + 1);
    }

    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("interact") && currentState != PlayerState.interact
            && currentState != PlayerState.stagger && inputEnabled.value)
        {
            StartCoroutine(InteractCo());
        }

        else if ((currentState == PlayerState.walk || currentState == PlayerState.idle)
            && inputEnabled.value)
            UpdateAnimationAndMove();

    }

    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }

        else
            animator.SetBool("moving", false);
    }

    public void MoveCharacter()
    {
        rb.MovePosition(
            transform.position + change.normalized * speed * Time.deltaTime);

        onPlayerMoveEvent.Raise(new Vector3(
            transform.position.x, transform.position.y, -10));
    }

    public void Stagger(float time)
    {
        StartCoroutine(StaggerCoroutine(time));
    }

    private IEnumerator StaggerCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector2.zero;
        currentState = PlayerState.idle;
    }

    private IEnumerator InteractCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.interact;
        audioSource.Play();
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(0.3f);
        currentState = PlayerState.walk;
    }

    public void Teleport(float x, float y)
    {
        rb.position = new Vector3(x, y, 0);
        onPlayerMoveEvent.Raise(new Vector3(x, y, -10));
    }
}
