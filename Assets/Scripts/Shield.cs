using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public MeshRenderer renderer;
    public bool shield;

    private void Update()
    {
        //Activa o desactiva el render según si es invulnerable o no
        //renderer.enabled = destructibleEntity.IsInvulnerable();
        if (shield)
            renderer.enabled = true;
        else
            renderer.enabled = false;
    }

    public void activateShield()
    {
        shield = true;
    }
    public void deactivateShield()
    {
        shield = false;
    }
}
