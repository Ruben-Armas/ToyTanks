using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //EVENTO (DELEGADO)   --> características a las que llama
    public delegate void EnemyCreated(Enemy enemyCreated, Vector3 position);
    public static event EnemyCreated onEnemyCreated;        //(EVENTO)
    //EVENTO (DELEGADO)   --> características a las que llama
    public delegate void EnemyDestroyed(Enemy enemyDestroyed, Vector3 position);
    public static event EnemyDestroyed onEnemyDestroyed;    //(EVENTO)
    //EVENTO (DELEGADO)   --> Sound Destroy
    public delegate void EnemySoundDestroy();
    public static event EnemySoundDestroy onEnemySoundDestroy;    //(EVENTO)
    //EVENTO (DELEGADO)   --> Effect Destroy
    public delegate void EnemyEffectDestroy(Vector3 position);
    public static event EnemyEffectDestroy onEnemyEffectDestroy;    //(EVENTO)

    public Vector3 startPosition { get; private set; }
    public GameObject prefab { get; private set; }

    private void Awake()
    {
        startPosition = transform.position;

        //Evento
        if (onEnemyCreated != null)
            onEnemyCreated(this, transform.position);
    }

    public void SetPrefab(GameObject enemyPrefab)
    {
        prefab = enemyPrefab;
    }

    public void SetDestroyed()
    {
        //invulnerabilityTime = time;

        //Evento Sound Destroy
        if (onEnemySoundDestroy != null)
            onEnemySoundDestroy();
        //Evento Effect Destroy
        if (onEnemyEffectDestroy != null)
            onEnemyEffectDestroy(transform.position);

        //Evento Destroy
        if (onEnemyDestroyed != null)
            onEnemyDestroyed(this, transform.position);
        Destroy(gameObject);
    }
}
