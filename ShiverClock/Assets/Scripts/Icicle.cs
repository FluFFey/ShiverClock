using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour {

    public float respawnTime;
    private Timer respawnTimer;
    private Vector3 spawnPos;
    private Rigidbody2D rb;
    private bool falling = false;
	// Use this for initialization
	void Start () {
        spawnPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        rb.freezeRotation = true;
        respawnTimer = new Timer(respawnTime,false);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (respawnTimer.hasEnded())
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            falling = false;
            respawnTimer.restart();
            respawnTimer.stop();
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            transform.position = spawnPos;
        }
        if (!falling)
        {
            foreach (GameObject player in GameController.getPlayers())
            {
                if (Mathf.Abs(player.transform.position.x - transform.position.x) < 1)
                {
                    fall();
                }
            }
        }
		
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        respawnTimer.start();
        gameObject.GetComponent<MeshRenderer>().enabled = false; //hacky solution for gamejam
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //...damage player...
    }

    //void isHit fall();

    void fall()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        falling = true;
    }
}
