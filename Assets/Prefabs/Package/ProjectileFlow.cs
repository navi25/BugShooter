using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProjectileFlow : MonoBehaviour {

	public Rigidbody2D asteroidProjectile1, asteroidProjectile2, asteroidProjectile3, asteroidProjectile4, asteroidProjectile5 ;
	public float speed = 15;
	public float fireAngle, count,ansC, gravity;
	public Text rangeText, heightText, velocityX, velocityY;
	public Slider angleSlider, speedSlider;
	public Text angleText, speedText;

	public Transform bird;
	private string targetHit;
	public Text row1, row2, row3, row4, row5;
	private Vector3 instantVel, gravityVector; // instantenous velocity
	private bool hit = false;
	private float vY, vX, Range, Height;

	private bool slowMotion = false;

	void Start()
	{
		transform.Rotate (new Vector3 (0,0 , fireAngle -30));
		vY = speed * Mathf.Sin (fireAngle * Mathf.Deg2Rad);
		vX = speed * Mathf.Cos (fireAngle * Mathf.Deg2Rad);
		count = 0;
		gravityVector = Physics2D.gravity; 
		gravity = 9.8f;
		targetHit = "NO";

		//Rigidbody2D asteroidClone = (Rigidbody2D) Instantiate(asteroidProjectile, transform.position, transform.rotation);
		//instantVel = Vector3.zero;
	}

	void Update(){
		/*if ((Input.GetKeyDown(KeyCode.S))) {
			if (Time.timeScale == 1.0F)
				Time.timeScale = 0.2F;
			else
				Time.timeScale = 1.0F;
			Time.fixedDeltaTime = 0.02F * Time.timeScale;

		}
		*/
	}

	void FixedUpdate () {


		vY = speed * Mathf.Sin (fireAngle * Mathf.Deg2Rad);
		vX = speed * Mathf.Cos (fireAngle * Mathf.Deg2Rad);

		updateValues ();

		if (Input.GetKeyDown(KeyCode.A)) {
			ThrowProjectile();

		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}

	}

	public void ThrowProjectile()
	{
		count += 1; 
		//Rigidbody2D asteroidClone = (Rigidbody2D) Instantiate(asteroidProjectile, transform.position, transform.rotation);
		if (count == 1) {
			asteroidProjectile1.isKinematic = false;
			asteroidProjectile1.velocity = new Vector3 (vX, vY, 0);
			swipePreviousValues();

		}
		else if (count == 2) {
			asteroidProjectile2.isKinematic = false ;
			asteroidProjectile2.velocity = new Vector3 (vX, vY, 0);
			swipePreviousValues();


		}
		else if (count == 3) {
			asteroidProjectile3.isKinematic = false ;
			asteroidProjectile3.velocity = new Vector3 (vX, vY, 0);
			swipePreviousValues();


		}
		else if (count == 4) {
			asteroidProjectile4.isKinematic = false ;
			asteroidProjectile4.velocity = new Vector3 (vX, vY, 0);
			swipePreviousValues();


		}
		else if (count == 5) {
			asteroidProjectile5.isKinematic = false ;
			asteroidProjectile5.velocity = new Vector3 (vX, vY, 0);
			swipePreviousValues();

		
		}

		//updateObservationTable ();

		}

	public void adjustSpeed(float newSpeed)
	{
		speed = newSpeed ;
	}

	public void adjustAngle(float newAngle)
	{
		fireAngle = newAngle;
		transform.Rotate (new Vector3 (0,0 , fireAngle -30));
		transform.eulerAngles = new Vector3(0, 0, fireAngle -30);
	}


	void updateValues()
	{
		if (count == 1) {
			velocityX.text = "Vx \n " + asteroidProjectile1.velocity.x.ToString ("F2") + "m/s";
			velocityY.text = "Vy \n" + asteroidProjectile1.velocity.y.ToString ("F2") + "m/s";

			
		}
		else if (count == 2) {
			velocityX.text = "Vx\n " + asteroidProjectile2.velocity.x.ToString ("F2") + "m/s";
			velocityY.text = "Vy\n " + asteroidProjectile2.velocity.y.ToString ("F2") + "m/s";
		}

		else if (count == 3) {
			velocityX.text = "Vx\n " + asteroidProjectile3.velocity.x.ToString ("F2") + "m/s";
			velocityY.text = "Vy\n" + asteroidProjectile3.velocity.y.ToString ("F2") + "m/s";
		}

		else if (count == 4) {
			velocityX.text = "Vx\n" + asteroidProjectile4.velocity.x.ToString ("F2") + "m/s";
			velocityY.text = "Vy\n" + asteroidProjectile4.velocity.y.ToString ("F2") + "m/s";
		}

		else if (count == 5) {
			velocityX.text = "Vx\n " + asteroidProjectile5.velocity.x.ToString ("F2") + "m/s";
			velocityY.text = "Vy\n " + asteroidProjectile5.velocity.y.ToString ("F2") + "m/s";
		}

	}

	void updateRange()
	{
		Range = (speed * speed * Mathf.Sin (2 * fireAngle * Mathf.Deg2Rad)) / gravity ; 
		rangeText.text = "Range\n " + Range.ToString ("F2") + "m";
	}

	void updateHeight()
	{
		Height = (speed * Mathf.Sin (fireAngle * Mathf.Deg2Rad) * speed * Mathf.Sin (fireAngle * Mathf.Deg2Rad)) / gravity;
		heightText.text = "Max Height\n " + Height.ToString ("F2")+ "m";
	}

	void swipePreviousValues(){

		rangeText.text = "Range\n " ;
		heightText.text = "Max Height\n ";
	
	}

	public void Reset(){

		Application.LoadLevel(Application.loadedLevel);
		Time.timeScale = 1.0F;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;

	
	}

	void 	updateObservationTable (){


		if (count == 1) {

			updateRange ();
			updateHeight ();
			if(bird.position != new Vector3(7.5f, -2.2f, 0) && hit == false)
			{
				targetHit = "YES";
				hit = true;
			}

			row1.text = "1.            " + speed.ToString() + "m/s            " + fireAngle.ToString() + "°            " + Range.ToString("F2")  + "m             "+ Height.ToString("F2")  + "m             " + targetHit; 
			targetHit = "NO";
		}
		else if (count == 2) {

			updateRange ();
			updateHeight ();
			if(bird.position != new Vector3(7.5f, -2.2f, 0) && hit == false)
			{
				targetHit = "YES";
				hit = true;
			}
			row2.text = "2.            " + speed.ToString() + "m/s            " + fireAngle.ToString() + "°            " + Range.ToString("F2")  + "m             "+ Height.ToString("F2")  + "m             " + targetHit; 
			targetHit = "NO";
		}
		else if (count == 3) {

			updateRange ();
			updateHeight ();
			if(bird.position != new Vector3(7.5f, -2.2f, 0) && hit == false)
			{
				targetHit = "YES";
				hit = true;
			}
			row3.text = "3.            " + speed.ToString() + "m/s            " + fireAngle.ToString() + "°            " + Range.ToString("F2")  + "m             "+ Height.ToString("F2")  + "m             " + targetHit; 
			targetHit = "NO";
		}
		else if (count == 4) {

			updateRange ();
			updateHeight ();
			if(bird.position != new Vector3(7.5f, -2.2f, 0) && hit == false)
			{
				targetHit = "YES";
				hit = true;
			}
			row4.text = "4.            " + speed.ToString() + "m/s            " + fireAngle.ToString() + "°            " + Range.ToString("F2")  + "m             "+ Height.ToString("F2")  + "m             " + targetHit; 
			targetHit = "NO";
		}
		else if (count == 5) {

			updateRange ();
			updateHeight ();
			if(bird.position != new Vector3(7.5f, -2.2f, 0) && hit == false)
			{
				targetHit = "YES";
				hit = true;
			}
			row5.text = "5.            " + speed.ToString() + "m/s            " + fireAngle.ToString() + "°            " + Range.ToString("F2")  + "m             "+ Height.ToString("F2")  + "m             " + targetHit; 
			targetHit = "NO";
		}



	}

	void OnEnable(){

		collisionScript.happened += updateObservationTable;
	}

	void OnDisable(){
		
		collisionScript.happened -= updateObservationTable;
	}

	public void beginSlowMotion(){

		slowMotion = true;
	}

	public void stopSlowMotion(){
		
		slowMotion = false;
	}

	public void slowMotionFunc(){
		if (Time.timeScale == 1.0F)
			Time.timeScale = 0.2F;
		else
			Time.timeScale = 1.0F;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
	}


	
}
