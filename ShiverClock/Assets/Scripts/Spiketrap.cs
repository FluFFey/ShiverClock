using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiketrap : MonoBehaviour
{

    private Rigidbody2D rb;
    private bool playerAbove = false;
    private float spikeTime = 0.5f;
    private float spikeHeight;
    private bool spikeHurts;
    private bool shootInProgress;
    // Use this for initialization
    void Start()
    {
        spikeHurts = false;
        shootInProgress = false;
        spikeHeight = transform.lossyScale.y;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!shootInProgress)
        {
            checkAboveCollision();
        }
        
    }
    private void checkAboveCollision()
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        bc.enabled = false; //hacky but works 4 gamejam
        Vector2 origin = (Vector2)transform.position + Vector2.up * transform.lossyScale.y * 0.52f;
        Vector2 size = transform.lossyScale;
        size.y *= 0.02f;
        size.x *= 0.98f;
        RaycastHit2D boxHit = Physics2D.BoxCast(origin, size, 0, Vector2.up, 0.1f);
        if (boxHit)
        {
            StartCoroutine(shootSpike(Vector2.up));
        }
        bc.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag =="Player")
        {
            StartCoroutine(collider.GetComponent<InputHandler>().killPlayer());
        }
    }

    public IEnumerator shootSpike(Vector3 direction) //direction normalized
    {
        shootInProgress = true; //cba with state machine at this stage
        yield return new WaitForSeconds(0.2f);

        
        Vector3 startPos = transform.localPosition;
        //float delta = 0.0f;
        //float oldPos = 0.0f;
        float newPos = 0.0f;

        for (float i = 0; i < spikeTime; i += Time.deltaTime)
        {
            float pd = i / spikeTime; //TODO: ???
            if (pd > 0.3f)
            {
                spikeHurts = true;
            }
            newPos = smoothlerp(pd);
            transform.localPosition = startPos + (direction * newPos * spikeHeight);
            yield return null;
        }
        transform.localPosition = startPos + (direction * spikeHeight);
        Vector3 endpos = transform.localPosition;

        yield return new WaitForSeconds(0.3f);
        for (float i = 0; i < spikeTime; i += Time.deltaTime)
        {
            float pd = i / spikeTime; //TODO: ???
            newPos = smoothlerp(pd);
            if (pd > 0.7f)
            {
                spikeHurts = false;
            }
            transform.localPosition = endpos + (-direction * newPos * spikeHeight);
            yield return null;
        }
        transform.localPosition = endpos+ (-direction * spikeHeight);
        shootInProgress = false;
    }

    private float smoothlerp(float pd)
    {
        return pd * pd * (3 - 2 * pd);
    }
}
