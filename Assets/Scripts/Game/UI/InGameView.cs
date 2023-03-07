using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
public class InGameView : MonoBehaviour
{
    //EVENTO (DELEGADO)   --> Para cambiar el Foco al botón ResumeButton
    public delegate void SelectButton();
    public static event SelectButton onSelectButton;    //(EVENTO)

    public MenuManager menuManager;
    public MenuView pauseView;

    public GameManager gameManager;
    public TextMeshProUGUI lives;
    public TextMeshProUGUI level;
    public TextMeshProUGUI record;  //Para guardar el record


    //private TankControls tankControls;
    private TankControls tankControls => GameInputManager.tankControls;
    //public PlayerInput playerInput;
    //private PlayerInput _playerInput;


    private void Awake()
    {
        //PlayerPrefs.DeleteAll();  //Borra todo

        /*//Si nunca se ha guardado una puntuación - que aparezca otro texto
        if (PlayerPrefs.GetInt("Record", -1) == -1)
            record.text = "Keep Playing!!";
        else*/

        //Si no tiene puntos se queda a 0
        record.text = $"Record: <color=red><b>{PlayerPrefs.GetInt("Record", 0)}</b>";   //- Forma 1
    }

    void Update()
    {
        lives.text = $"Lives: <color=#00e600><b>{gameManager.currentLives}</b>";
        level.text = $"Level: <color=#e60000><b>{gameManager.currentLevel}</b>";
        record.text = $"Record: <color=#e60000><b>{PlayerPrefs.GetInt("Record", 0)}</b>";
    }

    public void PauseGame()
    {
        //tankControls.Player.Disable();
        //GameInputManager.ToggleActionMap(tankControls.UI);

        //tankControls.Player.Disable();
        //tankControls.UI.Enable();

        //InputSystem.DisableAllEnabledActions();
        //playerInput.SwitchCurrentActionMap("UI");
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (var playerInput in playerInputs)
        {
            playerInput.SwitchCurrentActionMap("UI");
        }

        //Parar el tiempo   también para los menus que se deberían generar(no los genera)
        Time.timeScale = 0; //Todo lo que depende de la física se para
        //Time.timeScale = 0.5f;  //Cámara lenta
        //Time.timeScale = 10;  //Cámara rápida

        
        //Evento --> Cambiar Foco al botón ResumeButton
        if (onSelectButton != null)
            onSelectButton();

        menuManager.OpenView(pauseView);
    }
}