using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Toggle))]
public class ToggleButtonIcon : MonoBehaviour {

    public Sprite onIcon;
    public Sprite offIcon;
    private Toggle toggleButton;

    void Start()
    {
        // This automatically registers the event click on the button component
        toggleButton = GetComponent<Toggle>();
        toggleButton.onValueChanged.AddListener(Click);
        SetIcon();
    }

    public void Click(bool newValue)
    {
        SetIcon();
    }

    private void SetIcon()
    {
        if (toggleButton.isOn)
        {
            GetComponent<Image>().sprite = onIcon;
        }
        else
        {
            GetComponent<Image>().sprite = offIcon;
        }
    }
}
