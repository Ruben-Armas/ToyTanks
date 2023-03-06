using UnityEngine.InputSystem;

public class GameInputManager
{
    //Controles
    public static TankControls tankControls = new TankControls();

    //Cambiar entre controles
    public static void ToggleActionMap(InputActionMap inputActionMap)
    {
        tankControls.Player.Disable();
        tankControls.UI.Disable();
        //inputActionMap.Enable();
    }
}
