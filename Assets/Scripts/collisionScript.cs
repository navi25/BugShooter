using UnityEngine;
using System.Collections;

public class collisionScript : MonoBehaviour {

	public delegate void colHappens();
	public static event colHappens happened;

	Rigidbody2D body;
	// Use this for initialization
	void Start () {
		body = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	void OnCollisionEnter2D(Collision2D col){
		
		if (col.gameObject.tag == "ground") {
			body.velocity = Vector3.zero; 
			body.isKinematic = true;

			if(happened != null){

				happened();
			}
		}

	}

}
