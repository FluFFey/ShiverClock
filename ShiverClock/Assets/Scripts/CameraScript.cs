using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Vector2 target; //target to follow
    public float zoom;
   // public Vector2 targetOffset;
    public Color fadeColor;
    private Texture2D fadeTexture;
    public bool startFaded;

    // Use this for initialization
    void Awake()
    {
        Color startColor = fadeColor;
        startColor.a = startFaded ? 1 : 0;
     //   targetOffset = new Vector3(0, 0, 0);
        fadeTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        fadeTexture.SetPixel(0, 0, startColor);
        fadeTexture.Apply();
        target = Vector2.zero;
        zoom = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target;
            float maxZoom = -12;
            float minZoom = -26;
            newPos.z = zoom;// transform.position.z +(zoom - transform.position.z);
            if (newPos.z > maxZoom)
            {
                newPos.z = maxZoom;
            }
            if (newPos.z < minZoom)
            {
                newPos.z = minZoom;
            }
            transform.position = newPos;
            Vector2 botLeftEdge;
            Vector2 topRightEdge;
            Vector2 screenSize;
            updateEdges(newPos, out botLeftEdge, out topRightEdge, out screenSize);
            
            //print(screenWidth);
            if (botLeftEdge.x < -30)
            {
                newPos.z += (-30 - botLeftEdge.x) * 0.2f;
                updateEdges(newPos, out botLeftEdge, out topRightEdge, out screenSize);
                newPos.x = -30 + screenSize.x / 2.0f;
            }
            if (topRightEdge.x > 30)
            {
                newPos.z += (topRightEdge.x - 30 ) * 0.2f;
                updateEdges(newPos, out botLeftEdge, out topRightEdge, out screenSize);
                newPos.x = 30 - screenSize.x / 2.0f;
            }

            if (botLeftEdge.y < 0)
            {
                newPos.y += (30 - botLeftEdge.y) * 0.2f;
                updateEdges(newPos, out botLeftEdge, out topRightEdge, out screenSize);
                newPos.y = 0 + screenSize.y / 2.0f;
            }
            if (topRightEdge.y > 29)
            {
                newPos.y += (topRightEdge.y - 30) * 0.2f;
                updateEdges(newPos, out botLeftEdge, out topRightEdge, out screenSize);
                newPos.y = 29 - screenSize.y / 2.0f;
            }


            if (newPos.z > maxZoom)
            {
                newPos.z = maxZoom;
            }
            if (newPos.z < minZoom)
            {
                newPos.z = minZoom;
            }
            transform.position = newPos;

            //float followSpeed = 1.0f;
            //Vector3 distanceVector = target - (Vector2)transform.position;
            //float distanceFromTarget = distanceVector.magnitude;
            //distanceVector.z = (zoom - transform.position.z)*0.5f;
            //float zoomSpeed = 2.0f;
            ////print(target - (Vector2)transform.position);
            ////print(target);
            ////print((Vector2)transform.position);
            ////print("");
            //float maxZoom = -10;
            //float minZoom = -25;
            //Vector3 newPos = transform.position;
            //newPos += new Vector3(distanceVector.x * distanceFromTarget, distanceVector.y * distanceFromTarget, distanceVector.z * zoomSpeed) * followSpeed * Time.fixedDeltaTime;
            //if (newPos.z > maxZoom)
            //{
            //    newPos.z = maxZoom;
            //}
            //if (newPos.z < minZoom)
            //{
            //    newPos.z = minZoom;
            //}
            //Vector3 oldPos = transform.position;

            //transform.position = newPos;
            //Vector3 viewPosTopLeft = Camera.main.WorldToViewportPoint(new Vector3(-30, 29, 0));
            //Vector3 viewPosBotRight = Camera.main.WorldToViewportPoint(new Vector3(30, 1, 0));
            ////Vector3 camCenterInWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, transform.position.z));
            //float screenWidthInWorldSpace = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, -transform.position.z)).x - Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, -transform.position.z)).x;
            //float xEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, -transform.position.z)).x + screenWidthInWorldSpace/2.0f;
            //if (viewPosTopLeft.x > 0.0f)// || viewPosBotRight.x < 1.0f)
            //{

            //    //print(viewPosTopLeft.x);
            //    newPos.x = Camera.main.WorldToViewportPoint(new Vector3(-30.0f+screenWidthInWorldSpace, transform.position.y, transform.position.z)).x;// Camera.main.WorldToViewportPoint(new Vector3(-30-30, transform.position.y, transform.position.z)).x;
            //}

            //if (viewPosTopLeft.y < 1.0f || viewPosBotRight.y > 0.0f)
            //{
            //    //newPos.y = transform.position.y;
            //}
            //transform.position = newPos;

            //print(target.x);
            //print(viewPosTopLeft.x + screenWidthInWorldSpace / 2.0f);
            //if (target.x < xEdge)
            //{
            //    target.x = xEdge;
            //}
            //transform.position = new Vector3(target.x, target.y, newPos.z);
        }
    }

    private void updateEdges(Vector3 newPos, out Vector2 botLeftEdge, out Vector2 topRightEdge, out Vector2 screenSize)
    {
        botLeftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, -newPos.z));
        topRightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, -newPos.z));
        screenSize = topRightEdge - botLeftEdge;
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.width), fadeTexture);
    }

    public IEnumerator fade(bool fadeIn, float fadeTime = 1.0f)
    {
        float multiplier = fadeIn ? -1.0f : 1.0f;
        float start = fadeIn ? 1.0f : 0.0f;
        float a = 0;
        for (float i = 0; i < fadeTime; i += Time.deltaTime)
        {
            a = (i / fadeTime) * multiplier;
            fadeTexture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, start + a));
            fadeTexture.Apply();
            yield return null;
        }
        fadeTexture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, start + multiplier));
        fadeTexture.Apply();
    }
}
