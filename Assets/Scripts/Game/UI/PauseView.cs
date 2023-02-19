using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseView : MonoBehaviour
{
    public MenuManager menuManager;
    public MenuView ingameView;

    public void ResumeGame()
    {
        Time.timeScale = 1;
        menuManager.OpenView(ingameView);
    }

    public void ExitGame()
    {
        Time.timeScale = 1; //Reinicio del time
        /*Veil veil = FindObjectOfType<Veil>();
        if (veil != null)
        {
            veil.LoadScene("UIMainMenu");
        }*/
        //Lo de arriba ya no se hace porque ya lo comprueba el instance en Veil
        Veil.instance.LoadScene("UIMainMenu");
    }
}
