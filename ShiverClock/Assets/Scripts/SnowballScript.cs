using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballScript : MonoBehaviour {

    Rigidbody2D rb;
    public float lifetime;
    private float spawntime;
    public Sprite[] snowballVariations;
    GameObject throwerObject;
    private Vector3 startVel;
    private void Awake()
    {
        startVel = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
        spawntime = Time.time;

    }
    public void setStartVel(Vector3 newStartVel)
    {
        startVel = newStartVel;
    }

    // Use this for initialization
    void Start () {
        GetComponent<SpriteRenderer>().sprite = snowballVariations[Random.Range(0, snowballVariations.Length)];
	}

	public void setThrower(GameObject thrower)
    {
        throwerObject = thrower;
    }
	// Update is called once per frame
	void Update ()
    {
        rb.velocity = startVel * MyGameManager.instance.timeScale;
        if (Time.time - spawntime > lifetime)
        {
            Destroy(gameObject);
        }
        Vector3 newRot = transform.rotation.eulerAngles;
        newRot.z += 5 * Time.deltaTime * MyGameManager.instance.timeScale;
        transform.rotation = Quaternion.Euler(newRot);
        //Vector3 newVel = rb.velocity;
       // newVel.y -= 4 * Time.deltaTime * MyGameManager.instance.timeScale;
        //rb.velocity = newVel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (Time.time - spawntime > 0.01f)
       // {
            if (collision.tag == "Player" && (throwerObject != collision.gameObject))
            {
                collision.gameObject.GetComponent<InputHandler>().getKnockedBack(rb.velocity*1.65f+ Vector2.up*1.5f);
            }
            Destroy(gameObject);
        //}
        
    }
}