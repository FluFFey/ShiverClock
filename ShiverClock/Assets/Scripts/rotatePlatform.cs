using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatePlatform : MonoBehaviour {
    public enum RotationType
    {
        NO_ROT,
        FULL_ROT,
        HALF_ROT
    }
    public float rotationTime;
    public RotationType rotationType;
    private float sinVal = 0.0f;
    private Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float newAngle = transform.rotation.eulerAngles.z;

        switch (rotationType)
        {
            case RotationType.FULL_ROT:
                if (!Mathf.Approximately(rotationTime, 0))
                {
                    newAngle += (Time.deltaTime * MyGameManager.instance.timeScale * 360.0f) / (rotationTime);
                }
                break;
            case RotationType.HALF_ROT:
                if (!Mathf.Approximately( rotationTime,0))
                {
                    sinVal += (Time.deltaTime * MyGameManager.instance.timeScale) / (rotationTime * 9);
                    newAngle = Mathf.Sin(sinVal * Mathf.Rad2Deg) * 90;
                }
                break;
            default:
                print("no rotType. default to FULL_ROT");

                newAngle += rotationTime * Time.deltaTime;
                break;
        }
        rb.MoveRotation(newAngle);
	}
}
