using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
public class InGameView : MonoBehaviour
{
    public MenuManager menuManager;
    public MenuView pauseView;

    public GameManager gameManager;
    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI highScoreLabel;  //Para guardar el record


    //private TankControls tankControls;
    //private TankControls tankControls => GameInputManager.tankControls;
    public PlayerInput playerInput;
    //private PlayerInput _playerInput;


    private void Awake()
    {
        //_playerInput = GetComponent<PlayerInput>();
        //GameInputManager.ToggleActionMap(tankControls.Player);
        //tankControls = new TankControls();

        //PlayerPrefs.DeleteAll();  //Borra todo
        //Si nunca se ha guardado una puntuaci�n - que aparezca otro texto
        if (PlayerPrefs.GetInt("HighScore", -1) == -1)
            highScoreLabel.text = "Keep Playing!!";
        else
            //Si no tiene puntos se queda a 0   (IMPORTANTE ver que tiene el mismo nombre que en el Manager)
            highScoreLabel.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";   //- Forma 1
    }

    void Update()
    {
        //scoreLabel.text = $"Score: <color=red><b>{gameManager.currentScore}</b>";
    }

    public void PauseGame()
    {
        //tankControls.Player.Disable();
        //GameInputManager.ToggleActionMap(tankControls.UI);

        //tankControls.Player.Disable();
        //tankControls.UI.Enable();

        InputSystem.DisableAllEnabledActions();
        playerInput.SwitchCurrentActionMap("UI");


        //Parar el tiempo   tambi�n para los menus que se deber�an generar(no los genera)
        Time.timeScale = 0; //Todo lo que depende de la f�sica se para
        //Time.timeScale = 0.5f;  //C�mara lenta
        //Time.timeScale = 10;  //C�mara r�pida

        menuManager.OpenView(pauseView);
    }
}