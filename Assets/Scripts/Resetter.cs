using UnityEngine;
using System.Collections;

public class Resetter : MonoBehaviour {

	public Rigidbody2D projectile ;
	public float resetSpeed = 0.0025f ;

	private float resetSpeedSqr ;
	private SpringJoint2D spring;


	void Start(){

		resetSpeedSqr = resetSpeed * resetSpeed ;
		spring = projectile.GetComponent<SpringJoint2D> ();
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.R)) {
			Reset();
		}
		// in order to not reload the game when we are dragging the catapult. 
		if (spring == null && projectile.velocity.sqrMagnitude < resetSpeedSqr) {
			Reset(); 
		}

	}

	void OnTriggerExit2D (Collider2D other){
	
			if (other.attachedRigidbody == projectile) {
				
				Reset ();
			}
	}

	void Reset(){

		Application.LoadLevel (Application.loadedLevel);
	}
}
