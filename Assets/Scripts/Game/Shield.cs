using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
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
        _shield = true;
    }
    public void deactivateShield()
    {
        _shield = false;
    }
}
