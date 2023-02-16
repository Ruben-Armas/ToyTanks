using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //EVENTO (DELEGADO)   --> características a las que llama
    public delegate void PlayerCreated(Player player, Vector3 position);
    public static event PlayerCreated onPlayerCreated;        //(EVENTO)
    //EVENTO (DELEGADO)   --> características a las que llama
    public delegate void PlayerDestroyed(Player player, Vector3 position);
    public static event PlayerDestroyed onPlayerDestroyed;    //(EVENTO)

    public Vector3 startPosition { get; private set; }

    private void Awake()
    {
        startPosition = transform.position;

        //Evento
        if (onPlayerCreated != null)
            onPlayerCreated(this, transform.position);
    }

    public void SetDestroyed()
    {
        //invulnerabilityTime = time;
        //Evento
        if (onPlayerDestroyed != null)
            onPlayerDestroyed(this, transform.position);
        Destroy(gameObject);
    }
}
