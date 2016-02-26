using UnityEngine;
using System.Collections;

public class ProjectileThrow : MonoBehaviour
{
	public Transform Target; // transform for our target
	public float firingAngle = 30.0f; // the angle at which the projectile will be thrown
	public float gravity = 9.8f; // the gravity value
	
	public Transform asteroidProjectile;     // transform for the object to be thrown 
	private Transform myTransform; // temporary varible transform for the object that throws asteroidProjectile
	//private int angle, code_angle;
	//private Vector3 target_angle;

	// Awake() is used in unity to initialize any variables or game state before the game starts.
	void Awake()
	{
		myTransform = transform;      
	}

	//Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	void Start()
	{   /*
			Coroutine : A class in UnityEngine/Inherits from: YieldInstruction
			MonoBehaviour.StartCoroutine returns a Coroutine. 
			A coroutine is a function that can suspend its execution (yield) until the given given YieldInstruction 
			finishes.

		*/       
		// angle = ;
		//code_angle =  30;
		StartCoroutine(SimulateProjectile()); // parameter as a method which will be defined in IEnumerator
	}
	
	
	IEnumerator SimulateProjectile()
	//void SimulateProjectile()
	{
		// Short delay added before Projectile is thrown
		yield return new WaitForSeconds(1.5f);
		
		// Move projectile to the position of throwing object + add some offset if needed.
		asteroidProjectile.position = myTransform.position + new Vector3(0, 0.0f, 0);
		
		// Calculate distance to target
		float target_Distance = Vector3.Distance(asteroidProjectile.position, Target.position);
		
		// Calculate the velocity needed to throw the object to the target at specified angle.
		float asteroidProjectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
		
		// Extract the X  Y componenent of the velocity
		float Vx = Mathf.Sqrt(asteroidProjectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(asteroidProjectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
		
		// Calculate flight time.
		float flightDuration = target_Distance / Vx;
		
		// Rotate projectile to face the target.
		asteroidProjectile.rotation = Quaternion.LookRotation(Target.position - asteroidProjectile.position);
		myTransform.rotation = Quaternion.LookRotation(Target.position - asteroidProjectile.position);
		// Rotate();



		float elapse_time = 0;
		
		while (elapse_time < flightDuration)
		{
			asteroidProjectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
			
			elapse_time += Time.deltaTime;
			
			yield return null;
		}
	}

	void Rotate () {
		//Quaternion theRotation = transform.localRotation;
		//theRotation.z *= 270;
		//transform.localRotation = theRotation;

		Vector3 target_dir = Target.position - asteroidProjectile.position;
		Vector3 forward = transform.forward;
		float target_angle = Vector3.Angle(target_dir, forward);
		transform.Rotate (Vector3.forward * 30);
		asteroidProjectile.Rotate(Vector3.forward* -60);
	}
}
