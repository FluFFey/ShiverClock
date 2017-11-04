using System.Collections;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private float jumpForce = 10.0f;
    private float maxHorVelocity = 5.0f;
    public enum PlayerID
    {
        PlayerOne,
        PlayerTwo,
        PlayerThree,
        PlayerFour
    }
    public PlayerID playerID = 0;
    public int moveSpeed = 1;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        string horizontalInput = "";
        string jumpInput = "";
        switch (playerID)
        {
            case PlayerID.PlayerOne:
                horizontalInput = "HorizontalOne";
                jumpInput = "JumpOne";
                break;
            case PlayerID.PlayerTwo:
                horizontalInput = "HorizontalTwo";
                jumpInput = "JumpTwo";
                break;
            case PlayerID.PlayerThree:
            case PlayerID.PlayerFour:
            default:
                //print("Error. wrong PlayerID. defaulting to player1");
                horizontalInput = "HorizontalOne";
                jumpInput = "JumpTwo";
                break;
        }
        Vector2 newVelocity = new Vector3(0, 0);
        newVelocity.x = Input.GetAxis(horizontalInput);
        newVelocity = newVelocity.normalized * speed;
        rb.AddForce(newVelocity*10);
        if (rb.velocity.x > maxHorVelocity)
        {
            rb.velocity = new Vector2(maxHorVelocity, rb.velocity.y);
        }
        if (Mathf.Approximately(Input.GetAxis(jumpInput),1))
        {
            Vector2 origin = transform.position;// + Vector2.down * transform.lossyScale.y * 0.5f;
            Vector2 size = transform.lossyScale;
            BoxCollider2D bc = GetComponent<BoxCollider2D>();
            bc.enabled = false; //hacky but works 4 gamejam
            RaycastHit2D boxHit = Physics2D.BoxCast(origin, size, 0, Vector2.down, 0.1f);
            if (boxHit && (rb.velocity.y < 0 || Mathf.Approximately(rb.velocity.y,0.0f))) //dunno if approx check is needed, but just to be safe for the jam
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            bc.enabled = true;
        }
    }
}
