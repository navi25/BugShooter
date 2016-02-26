using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonShowHide : MonoBehaviour {

	public bool startShowGroup;
	public GameObject[] showHideGroup;

	private bool show;

	void Start () {
		// This automatically registers the event click on the button component
		GetComponent<Button>().onClick.AddListener(() => { Click(); });
		show = startShowGroup;
		ShowHideUpdate();
	}

	public void Click()
	{
		show = !show;
		ShowHideUpdate();
	}

	private void ShowHideUpdate()
	{
		foreach (GameObject go in showHideGroup)
		{
			go.SetActive(show);
		}
	}
}
