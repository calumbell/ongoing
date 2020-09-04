using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float speed;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector3 change;
    private GameObject dungeonObj;
    private Dungeon dungeon;
    private GameObject mainCamara;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // get references to dungeon
        dungeonObj = GameObject.FindGameObjectWithTag("Dungeon");
        dungeon = dungeonObj.GetComponentInChildren<DungeonGenerator>().dungeon;
        Room startRoom = dungeon.getStartRoom();
        Teleport(4 * (startRoom.getX() + (startRoom.getWidth() / 2)) + 1,
            4 * (startRoom.getY() + (startRoom.getHeight() / 2)) + 1);

        // find main camera and set it to follow the player
        mainCamara = GameObject.FindGameObjectWithTag("MainCamera");
        CameraMovement camMoveScript = mainCamara.GetComponent<CameraMovement>();
        camMoveScript.setTarget(transform);
    }

    // Update is called once per frame
    void Update() {
        change = Vector3.zero;
        change.x = Input.GetAxis("Horizontal");
        change.y = Input.GetAxis("Vertical");

        UpdateAnimationAndMove();
    }

    void UpdateAnimationAndMove() {
        if (change != Vector3.zero) {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }

        else
            animator.SetBool("moving", false);
    }

    public void MoveCharacter() {
        rb.MovePosition(
            transform.position + change.normalized * speed * Time.deltaTime);
    }

    public void Teleport(float x, float y) {
        rb.position = new Vector2(x, y);
    }
}
