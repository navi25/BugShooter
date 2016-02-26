using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class cannonScript : MonoBehaviour {
	
	public int angle;
	private int code_angle;
	public GUIText angleText;
	// Use this for initialization
	void Start () {
		code_angle = angle - 30;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.A))
			RotateLeft();
	}
	
	void RotateLeft () {
		//Quaternion theRotation = transform.localRotation;
		//theRotation.z *= 270;
		//transform.localRotation = theRotation;
		transform.Rotate (Vector3.up * code_angle);
	}
}
