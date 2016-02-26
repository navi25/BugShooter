using UnityEngine;
using System.Collections;

public class activate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setActiveLearn()
	{
		gameObject.SetActive(true);
	}

	public void deActiveLearn()
	{
		gameObject.SetActive(false);
	}

	public void setActiveExplanation(){
		gameObject.SetActive(true);
	}

	public void deActiveExplanation(){
		gameObject.SetActive(false);
	}

	public void setActiveObservation(){
		gameObject.SetActive(true);
	}
	
	public void deActiveObservation(){
		gameObject.SetActive(false);
	}


	public void increaseSizeObservation(){
		gameObject.transform.localScale = new Vector3(1.8f , 1.3f , 0);
	}

	public void decreaseSizeObservation(){
		gameObject.transform.localScale = new Vector3(1.0f , 1.0f , 0);
	}

	public void hideIncreaser(){
		gameObject.SetActive(false);
	}

	public void hideDecreaser(){
		gameObject.SetActive(false);
	}

	public void showIncreaser(){
		gameObject.SetActive(true);
	}

	public void showDecreaser(){
		gameObject.SetActive(true);
	}

	public void increaseSizeExplanation(){
		gameObject.transform.localScale = new Vector3(1.8f , 1.3f , 0);
	}
	
	public void decreaseSizeExplanation(){
		gameObject.transform.localScale = new Vector3(1.0f , 1.0f , 0);
	}
	
	public void hideIncreaserExplanation(){
		gameObject.SetActive(false);
	}
	
	public void hideDecreaserExplanation(){
		gameObject.SetActive(false);
	}
	
	public void showIncreaserExplanation(){
		gameObject.SetActive(true);
	}
	
	public void showDecreaserExplanation(){
		gameObject.SetActive(true);
	}
}
