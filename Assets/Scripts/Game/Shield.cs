using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    //EVENTO (DELEGADO)   --> Sonido Activar
    public delegate void ActivateShield();
    public static event ActivateShield onActivateShield;    //(EVENTO)
    //EVENTO (DELEGADO)   --> Sonido Desactivar
    public delegate void DeactivateShield();
    public static event DeactivateShield onDeactivateShield;    //(EVENTO)

    public MeshRenderer renderer;
    private bool _shield;

    private void Update()
    {
        //Activa o desactiva el render según si es invulnerable o no
        //renderer.enabled = destructibleEntity.IsInvulnerable();
        if (_shield)
            renderer.enabled = true;
        else
            renderer.enabled = false;
    }

    public bool getShield()
    {
        return _shield;
    }
    public void activateShield()
    {
        //Evento --> Cambiar Foco al botón ResumeButton
        if (onActivateShield != null)
            onActivateShield();
        _shield = true;
    }
    public void deactivateShield()
    {//Evento --> Cambiar Foco al botón ResumeButton
        if (onDeactivateShield != null)
            onDeactivateShield();
        _shield = false;
    }
}
