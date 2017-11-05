using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballScript : MonoBehaviour {

    Rigidbody2D rb;
    public float lifetime;
    private float spawntime;
    public Sprite[] snowballVariations;
    GameObject throwerObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawntime = Time.time;

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
        if (Time.time - spawntime > 0.05f)
        {
            if (collision.tag == "Player" && (throwerObject != collision.gameObject))
            {
                print(collision.gameObject.name);
                collision.gameObject.GetComponent<InputHandler>().getKnockedBack(rb.velocity*0.8f);
            }
            Destroy(gameObject);
        }
        
    }
}