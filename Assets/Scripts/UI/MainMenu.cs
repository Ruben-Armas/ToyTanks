using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Veil.instance.LoadScene(sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
