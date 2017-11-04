using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour {

    public float defaultRespawnTime;
    private float modifiedRespawnTime;
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
        respawnTimer = new Timer(defaultRespawnTime,false);
        modifiedRespawnTime = defaultRespawnTime;
	}

    private void FixedUpdate()
    {
        if (falling)
        {
            rb.velocity += Vector2.down * Time.fixedDeltaTime * MyGameManager.instance.timeScale* MyGameManager.instance.defaultGravity;
        }
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (respawnTimer.hasEnded())
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            rb.freezeRotation = true;
            falling = false;
            respawnTimer.restart();
            respawnTimer.stop();
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            transform.position = spawnPos;
        }
        if (falling)
        {
            //print(modifiedRespawnTime);
            //modifiedRespawnTime += modifiedRespawnTime * (1.0f / MyGameManager.instance.timeScale) * Time.deltaTime - modifiedRespawnTime*Time.deltaTime; //should work I guess?? note: does not work. don't know 
            //print(modifiedRespawnTime);
            respawnTimer.setDuration(modifiedRespawnTime);
        }
        if (!falling)
        {
            foreach (GameObject player in MyGameManager.getPlayers())
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
        rb.velocity = Vector2.zero;
        gameObject.GetComponent<MeshRenderer>().enabled = false; //hacky solution for gamejam
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        //...damage player...
    }

    //void isHit fall();

    void fall()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.freezeRotation = true;
        falling = true;
    }
}
