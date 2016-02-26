using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UI_behaviour_Script : MonoBehaviour {

	public Text velocityX, velocityY;

	private Vector2 instantVel;

	// Use this for initialization
	void Start () {

		instantVel = GetComponent<Rigidbody2D>().velocity;
		updateValues ();
	
	}
	
	// Update is called once per frame
	void Update () {
		updateValues ();
	}

	void updateValues()
	{
		velocityX.text = "Vx: " + instantVel.x.ToString ();
		velocityY.text = "Vy: " + instantVel.y.ToString (); 
	}


}
