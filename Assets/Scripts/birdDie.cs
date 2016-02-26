using UnityEngine;
using System.Collections;

public class birdDie : MonoBehaviour {

	private Rigidbody2D body;
	public float ansCount = 0;
	// Use this for initialization
	void Start () {
		body = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col){
		
		if (col.gameObject.tag == "asteroid") {
			body.isKinematic = false;
			ansCount +=1;
		}

		if (col.gameObject.tag == "ground") {
			body.isKinematic = true;
		}
		
	}

	void updateAnsCount(){
		ansCount += 1;
	}
}
