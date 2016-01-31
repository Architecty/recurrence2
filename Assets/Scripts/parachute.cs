using UnityEngine;
using System.Collections;

public class parachute : MonoBehaviour {
	public float forwardspeed =  3F;
	public float downspeed = 3F;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Super basic for now
		transform.position += ((transform.forward * forwardspeed) + (transform.up * -downspeed)) * Time.deltaTime;
	}
}
