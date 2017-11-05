using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pulseScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Color newColor = GetComponent<Text>().color;
        newColor.a = (Mathf.Sin(Time.time*2.0f)+1.0f)*0.33f +0.33f;
        GetComponent<Text>().color = newColor;
    }
}
