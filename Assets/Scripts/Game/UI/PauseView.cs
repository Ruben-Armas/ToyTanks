using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    public MenuManager menuManager;
    public MenuView ingameView;

    public GameObject firstSelectedButton;

    //public InputActionAsset uiInputAsset;
    //private TankControls tankControls => GameInputManager.tankControls;

    //SUSCRIPCIÓN al EVENTO
    void OnEnable()
    {
        InGameView.onSelectButton += OnSelectButton;
    }
    //DESUSCRIPCIÓN al EVENTO
    void OnDisable()
    {
        InGameView.onSelectButton -= OnSelectButton;
    }
    //DELEGADOS
    private void OnSelectButton()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        //GameInputManager.ToggleActionMap(tankControls.Player);

        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (var playerInput in playerInputs)
        {
            playerInput.SwitchCurrentActionMap("Player");
        }

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
