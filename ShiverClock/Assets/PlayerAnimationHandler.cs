using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour {

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer sr;
    InputHandler ih;
    //bool throwingBall = false;
    //bool hurt = false;
	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        ih = GetComponent<InputHandler>();
    }
	
    public void setThrowingBall()
    {
        animator.SetTrigger("throwingBall");
    }

    public void setHurt()
    {
        animator.SetTrigger("isHurt");
    }

    // Update is called once per frame
    void Update () {
        animator.SetFloat("velocity", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isGrounded", ih.getGrounded());
        if (rb.velocity.x < 0.01)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
	}
}
