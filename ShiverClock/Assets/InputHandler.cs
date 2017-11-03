using System.Collections;
using UnityEngine;

enum HERO_STATE
{
    IDLE,
    MOVING,
    DASHING,
    NO_OF_STATES
}

enum GHOST_STATE
{
    HUMAN,
    GHOST
}

enum MOVE_DIRECTION
{
    UP = 1,
    DOWN = 2,
    LEFT = 4,
    RIGHT = 8,
    UPLEFT = UP + LEFT,
    UPRIGHT = UP + RIGHT,
    DOWNLEFT = DOWN + LEFT,
    DOWNRIGHT = DOWN + RIGHT
}

struct Inputs
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool space;
}

public class InputHandler : MonoBehaviour
{

    public int moveSpeed = 1;
    private void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        float speed = 2.0f;
        Vector2 newVelocity = new Vector3(0, 0);
        //newVelocity.y = Input.GetAxis("Horizontal");
        newVelocity.x = Input.GetAxis("Horizontal");
        newVelocity = newVelocity.normalized * speed;
        gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
    }
}
