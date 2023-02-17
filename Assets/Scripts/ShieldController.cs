using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShieldController : MonoBehaviour
{
    public BoxCollider _boxCollider;
    private Shield shield;
    private Player player;

    void OnValidate()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        player = collision.GetComponent<Player>();
        if (player != null)
        {
            shield = player.GetComponent<Shield>();
            if (shield != null)
            {
                //Debug.Log("Escudo!!");
                shield.activateShield();    //Activar escudo
                Destroy(gameObject);        //Quitar escudo recogido del mapa
            }
        }
    }
}