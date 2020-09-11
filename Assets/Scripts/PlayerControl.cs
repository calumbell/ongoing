using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{

    walk,
    interact
}

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public PlayerState currentState;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private Vector3 change;

    private GameObject dungeonObj;
    private Dungeon dungeon;
    private GameObject mainCamara;

    void Awake()
    {
        currentState = PlayerState.walk;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // get references to dungeon
        dungeonObj = GameObject.FindGameObjectWithTag("DungeonManager");
        dungeon = dungeonObj.GetComponentInChildren<DungeonGenerator>().dungeon;
        Room startRoom = dungeon.getStartRoom();
        Teleport(4 * (startRoom.getX() + (startRoom.getWidth() / 2)) + 1,
            4 * (startRoom.getY() + (startRoom.getHeight() / 2)) + 1);

        // find main camera and set it to follow the player
        mainCamara = GameObject.FindGameObjectWithTag("MainCamera");
        CameraMovement camMoveScript = mainCamara.GetComponent<CameraMovement>();
        camMoveScript.setTarget(transform);
    }

    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("interact") && currentState != PlayerState.interact)
        {
            StartCoroutine(InteractCo());
        }

        else if (currentState == PlayerState.walk)
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
        rb.position = new Vector2(x, y);
    }
}
