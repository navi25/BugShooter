using UnityEngine;
using System.Collections;

public class ProjectileFollow : MonoBehaviour {

	public Transform projectile; // transform of object that needs to be followed
	public Transform farLeft;    // transform for left boundary marker
	public Transform farRight;   // transform for right boundary marker

	/*  Algorithm for cameraFollow:
		1. Objective:- The position of camera should follow the position of attached gameObject.
		2. In every 1 second, we will store position of camera in with a temporary variable in update function.
		3. Then we will store the position of object to be followed in the camera's position.
		4. The camera's position will be clamped between farleft and farRight position.
		5. In every frame, the position of camera will change as per temprory newPosition variable.
	 */
	void Update () {

		Vector3 newPosition = transform.position;
		newPosition.x = projectile.position.x;
		newPosition.x = Mathf.Clamp (newPosition.x, farLeft.position.x, farRight.position.x);
		transform.position = newPosition; 
	
	}
}
