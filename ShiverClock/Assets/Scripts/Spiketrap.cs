//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Spiketrap : MonoBehaviour {

//    Rigidbody2D rb;
//    bool playerAbove = false;
//    private float spikeTime = 0.5f;
//    float spikeHeight;
//	// Use this for initialization
//	void Start () {
//        spikeHeight = transform.lossyScale.y;
//		rb = GetComponent<Rigidbody2D>();
//    }
	
//	// Update is called once per frame
//	void Update () {
		
//	}
//    private void checkAboveCollision()
//    {
//        BoxCollider2D bc = GetComponent<BoxCollider2D>();
//        bc.enabled = false; //hacky but works 4 gamejam
//        Vector2 origin = (Vector2)transform.position + Vector2.up * transform.lossyScale.y * 0.5f;
//        Vector2 size = transform.lossyScale;
//        size.y *= 0.05f;
//        size.x *= 0.98f;
//        RaycastHit2D boxHit = Physics2D.BoxCast(origin, size, 0, Vector2.down, 0.1f);
//        if (boxHit && (rb.velocity.y < 0 || Mathf.Approximately(rb.velocity.y, 0.0f))) //dunno if approx check is needed, but just to be safe for the jam
//        {
//            if (boxHit.collider.tag == "Player")
//            {
//                StartCoroutine(shootSpike(Vector2.up));
//            }
            
//        }
//        bc.enabled = true;
//    }

//    public IEnumerator shootSpike(Vector3 direction) //direction normalized
//    {
//        Vector3 startPos = transform.localPosition;
//        //float delta = 0.0f;
//        //float oldPos = 0.0f;
//        float newPos = 0.0f;

//        for (float i = 0; i < spikeTime; i += Time.deltaTime)
//        {
//            float pd = i / spikeTime; //TODO: ???
//            newPos = smoothlerp(pd);
//            transform.localPosition = startPos + (direction * newPos* spikeHeight);
//            yield return null;
//        }
//        transform.localPosition = startPos + (direction * spikeHeight);
//        yield return new WaitForSeconds(0.5f);
//        for (float i = 0; i < spikeTime; i += Time.deltaTime)
//        {
//            float pd = i / spikeTime; //TODO: ???
//            newPos = smoothlerp(pd);
//            transform.localPosition = startPos + (direction * newPos * spikeHeight);
//            yield return null;
//        }
//    }

//    private float smoothlerp(float pd)
//    {
//        return pd * pd * (3 - 2 * pd);
//    }
//}
