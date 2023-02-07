using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShieldController : MonoBehaviour
{
    public BoxCollider _boxCollider;
    private Shield shield;

    void OnValidate()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        shield = collision.GetComponentInParent<Shield>();
        if (shield != null)
        {
            //Debug.Log("Escudo!!");
            shield.activateShield();    //Activar escudo
            Destroy(gameObject);        //Quitar escudo recogido del mapa
        }
    }
}

/*
 * HACER QUE EL ESCUDO DESAPAREZCA AL COGERLO
 * HACER QUE EL ESCUDO SE ROMPA CUANDO TE IMPACTAN
*/ 