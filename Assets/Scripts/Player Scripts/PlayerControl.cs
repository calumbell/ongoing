using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    idle,
    walk,
    interact,
    stagger,
    carrying
}

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public PlayerState currentState;

    public BoolValue interacting;

    public BoolValue inputEnabled;

    public Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private Vector3 change;

    private GameObject dungeonObj;
    private Dungeon dungeon;

    public IntValue playerHealth;

    private GameObject carriedObject;


    // Events

    public IntEvent onPlayerHealthChange;
    public VoidEvent onPlayerAttackTriggered;
    public VoidEvent onPlayerInteractTriggered;
    public VoidEvent onPlayerInteractCycle;
    public Vector3Event onPlayerMoveEvent;
    public Vector3Event onPlayerTeleport;

    void Awake()
    {
        Application.targetFrameRate = 60;

        currentState = PlayerState.walk;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // get references to dungeon
        dungeonObj = GameObject.FindGameObjectWithTag("DungeonManager");

        if (dungeonObj != null)
        {
            dungeon = dungeonObj.GetComponentInChildren<DungeonManager>().dungeon;
            Room startRoom = dungeon.getStartRoom();
            Teleport(4 * (startRoom.getX() + (startRoom.getWidth() / 2)) + 1,
                4 * (startRoom.getY() + (startRoom.getHeight() / 2)) + 1);
        }

        playerHealth.value = 3;
    }

    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        // Handle player attacks
        if (Input.GetButtonDown("interact") && inputEnabled.value &&
            (currentState == PlayerState.idle || currentState == PlayerState.walk))
        {
            onPlayerAttackTriggered.Raise();
            StartCoroutine(InteractCo());
        }

        else if (Input.GetButtonDown("interact") && inputEnabled.value &&
            currentState == PlayerState.carrying)
        {
            Throw();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            onPlayerInteractCycle.Raise();
        }

        // Handle player interactions
        if (Input.GetKeyDown(KeyCode.E)
        && currentState != PlayerState.interact
        && currentState != PlayerState.stagger
        && inputEnabled.value)
        {
            onPlayerInteractTriggered.Raise();
        }

        // Handle player movement
        else if ((currentState == PlayerState.walk || currentState == PlayerState.idle || currentState == PlayerState.carrying)
            && inputEnabled.value)
            UpdateAnimationAndMove();

        // Update camera movement if player is being moved by something else
        else if (currentState == PlayerState.stagger)
            onPlayerMoveEvent.Raise(new Vector3(transform.position.x, transform.position.y, -10));

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
        { 
            animator.SetBool("moving", false);
        }
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
        playerHealth.value = playerHealth.value - 1;

        if (playerHealth.value == 0)
        {
            SceneManager.LoadScene("End");
        }

        StartCoroutine(StaggerCoroutine(time));
    }

    private IEnumerator StaggerCoroutine(float time)
    {
        currentState = PlayerState.stagger;
        animator.SetBool("hurt", true);
        onPlayerHealthChange.Raise(playerHealth.value);
        yield return new WaitForSeconds(time);
        rb.velocity = Vector2.zero;
        animator.SetBool("hurt", false);
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

    public void PickUp(GameObject obj)
    {
        if (currentState == PlayerState.carrying)
        {
            return;
        }

        obj.transform.parent = gameObject.transform;
        carriedObject = obj;
        obj.transform.position = new Vector3(
            gameObject.transform.position.x, gameObject.transform.position.y + 0.75f, obj.transform.position.z);
        currentState = PlayerState.carrying;

        // turn off context clues
        gameObject.transform.Find("ContextClue").gameObject.SetActive(false);

    }

    public void Teleport(float x, float y)
    {
        rb.position = new Vector3(x, y, 0);
        onPlayerTeleport.Raise(new Vector3(x, y, -10));
    }

    public void Throw()
    {
        carriedObject.transform.parent = null;
        currentState = PlayerState.idle;

        gameObject.transform.Find("ContextClue").gameObject.SetActive(true);

    }
}
