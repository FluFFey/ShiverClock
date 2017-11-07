using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveLength;
    public float moveTime;
    public enum StartMoveDirection
    {
        START_LEFT,
        START_RIGHT
    }
    public Vector3 moveDirection;
    private float sinVal = 0.0f;
    private Vector3 startPos;
    private Rigidbody2D rb;
    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        //GetComponent<Rigidbody2D>().velocity = moveDirection
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!Mathf.Approximately(moveTime, 0))
        {
            //sinVal += Time.fixedDeltaTime / (moveTime * 9); //not really sure about this
            //transform.position = startPos +moveDirection * Mathf.Sin(sinVal * Mathf.Rad2Deg) * moveLength;
        }
    }

    private void FixedUpdate()
    {
        if (!Mathf.Approximately(moveTime, 0))
        {
            sinVal += Time.fixedDeltaTime*MyGameManager.instance.timeScale / (moveTime * 9); //not really sure about this
            rb.MovePosition(startPos+ moveDirection * Mathf.Sin(sinVal * Mathf.Rad2Deg) * moveLength);
            rb.AddForce(Vector2.zero);
        }

    }
}