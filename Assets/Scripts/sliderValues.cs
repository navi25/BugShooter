using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class sliderValues : MonoBehaviour {

	public Text angleText, speedText;

	void Start(){
		angleText.text ="Angle: 30°";
		speedText.text ="Speed: 15 m/s";
	}

	public void updateAngle(float value)
	{
		angleText.text = "Angle: " + value.ToString () + "°";
	}
	
	public void updateSpeed(float value)
	{
		speedText.text = "Speed: " + value.ToString () + " m/s";
	}

}
