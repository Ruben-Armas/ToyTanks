using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreen : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        //Actualiza el estado visual del toggle
        toggle.isOn = Screen.fullScreen;
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = toggle.isOn;
    }
    public void ActivateFullScreen()
    {
        Screen.fullScreen = true;
    }
    public void DeactivateFullScreen()
    {
        Screen.fullScreen = false;
    }
}
