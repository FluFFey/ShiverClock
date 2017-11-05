using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRechargeScript : MonoBehaviour {
    public int rechargeAmount;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newRot = transform.rotation.eulerAngles;
        newRot.z += (Mathf.Sin(Time.deltaTime * MyGameManager.instance.timeScale))*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(newRot);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player")
        {
            collision.GetComponent<InputHandler>().modifyEnergy(rechargeAmount);
            Destroy(gameObject);
        }
        
    }
}
