using System.Collections;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public enum PlayerID
    {
        PlayerOne,
        PlayerTwo
    }
    public PlayerID playerID = 0;
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
        string horizontalInput = "";
        switch (playerID)
        {
            case PlayerID.PlayerOne:
                horizontalInput = "HorizontalOne";
                break;
            case PlayerID.PlayerTwo:
                horizontalInput = "HorizontalTwo";
                break;
            default:
                print("Error. wrong PlayerID. defaulting to player1");
                horizontalInput = "HorizontalOne";
                break;
        }
        Vector2 newVelocity = new Vector3(0, 0);
        //newVelocity.y = Input.GetAxis("Horizontal");
        newVelocity.x = Input.GetAxis(horizontalInput);
        newVelocity = newVelocity.normalized * speed;
        gameObject.GetComponent<Rigidbody2D>().AddForce(newVelocity*10);
        //gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
    }
}
