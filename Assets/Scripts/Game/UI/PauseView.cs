using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    public MenuManager menuManager;
    public MenuView ingameView;

    public GameObject firstSelectedButton;
    private ChangeFirstSelectedButton changeFirstSelectedButton;

    //public InputActionAsset uiInputAsset;
    //private TankControls tankControls => GameInputManager.tankControls;

    /*private void Awake()
    {
        GameInputManager.ToggleActionMap(tankControls.UI);
    }*/

    private void Start()
    {
        // Buscar el primer botón dentro de MenuHolder -> ButtonsHolder
        GameObject buttonsHolder = transform.Find("MenuHolder/ButtonsHolder").gameObject;
        if (buttonsHolder != null)
        {
            Button firstButton = buttonsHolder.GetComponentInChildren<Button>();
            if (firstButton != null)
            {
                firstSelectedButton = firstButton.gameObject;
            }
        }

        //changeFirstSelectedButton = new ChangeFirstSelectedButton();
        //changeFirstSelectedButton.SetFirstSelectedButton("MenuHolder/ButtonsHolder");
    }
    private void OnEnable()
    {
        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        //changeFirstSelectedButton = new ChangeFirstSelectedButton();
        //changeFirstSelectedButton.SetFirstSelectedButton("MenuHolder/ButtonsHolder");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        //GameInputManager.ToggleActionMap(tankControls.Player);
        menuManager.OpenView(ingameView);
    }

    public void ExitGame()
    {
        Time.timeScale = 1; //Reinicio del time
        //GameInputManager.ToggleActionMap(tankControls.UI);
        /*Veil veil = FindObjectOfType<Veil>();
        if (veil != null)
        {
            veil.LoadScene("UIMainMenu");
        }*/
        //Lo de arriba ya no se hace porque ya lo comprueba el instance en Veil
        Veil.instance.LoadScene("UIMainMenu");
    }
    /*
    void OnEnable()
    {
        Time.timeScale = 0;
        InputSystem.DisableAllEnabledActions();
        InputSystem.EnableDevice(uiInputAsset);
    }
    void OnDisable()
    {
        Time.timeScale = 1;
        InputSystem.DisableDevice(uiInputAsset);
        InputSystem.EnableAllEnabledActions();
    }*/
}
