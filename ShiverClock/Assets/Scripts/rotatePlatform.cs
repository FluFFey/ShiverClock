using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatePlatform : MonoBehaviour {
    public enum RotationType
    {
        FULL_ROT,
        HALF_ROT
    }
    public float rotationTime;
    public RotationType rotationType;
    private float sinVal = 0.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 newRot = transform.rotation.eulerAngles;

        switch (rotationType)
        {
            case RotationType.FULL_ROT:
                if (!Mathf.Approximately(rotationTime, 0))
                {
                    newRot.z += (Time.deltaTime * 360.0f) / (rotationTime);
                }
                break;
            case RotationType.HALF_ROT:
                if (!Mathf.Approximately( rotationTime,0))
                {
                    sinVal += Time.deltaTime/ (rotationTime * 9); //not really sure about this
                    newRot.z = Mathf.Sin(sinVal * Mathf.Rad2Deg) * 90;
                }
                break;
            default:
                print("no rotType. default to FULL_ROT");
                newRot.z += rotationTime * Time.deltaTime;
                break;
        }
        transform.rotation = Quaternion.Euler(newRot);
	}
}
