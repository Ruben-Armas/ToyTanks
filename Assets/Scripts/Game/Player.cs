using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //EVENTO (DELEGADO)   --> Crear
    public delegate void PlayerCreated(Player player, Vector3 position);
    public static event PlayerCreated onPlayerCreated;        //(EVENTO)
    //EVENTO (DELEGADO)   --> Destruir
    public delegate void PlayerDestroyed(Player player, Vector3 position);
    public static event PlayerDestroyed onPlayerDestroyed;    //(EVENTO)
    //EVENTO (DELEGADO)   --> Sound Destroy
    public delegate void PlayerSoundDestroy();
    public static event PlayerSoundDestroy onPlayerSoundDestroy;    //(EVENTO)
    //EVENTO (DELEGADO)   --> Effect Destroy
    public delegate void PlayerEffectDestroy(Vector3 position);
    public static event PlayerEffectDestroy onPlayerEffectDestroy;    //(EVENTO)

    public enum Colors
    {
        blue,
        green
    }
    public Colors color;
    public int ID;

    public Vector3 startPosition { get; private set; }

    private void Awake()
    {
        startPosition = transform.position;

        //Evento Create
        if (onPlayerCreated != null)
            onPlayerCreated(this, transform.position);
    }

    public void SetDestroyed()
    {
        //invulnerabilityTime = time;

        //Evento Sound Destroy
        if (onPlayerSoundDestroy != null)
            onPlayerSoundDestroy();
        //Evento Effect Destroy
        if (onPlayerEffectDestroy != null)
            onPlayerEffectDestroy(transform.position);

        //Evento Destroy
        if (onPlayerDestroyed != null)
            onPlayerDestroyed(this, transform.position);
        Destroy(gameObject);
    }
}
