using UnityEngine;
using System.Collections;

public class screenChange : MonoBehaviour {

	public void changeToLevel00(){

		Application.LoadLevel (0);
	}

	public void changeToLevel01(){
		
		Application.LoadLevel (1);
	}

	public void changeToLevel02(){
		
		Application.LoadLevel (2);
	}

}
