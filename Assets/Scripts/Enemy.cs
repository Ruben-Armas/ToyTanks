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


    private void Awake()
    {
        //Evento
        if (onEnemyCreated != null)
            onEnemyCreated(this, transform.position);
    }
    public void SetDestroyed()
    {
        //invulnerabilityTime = time;
        //Evento
        if (onEnemyDestroyed != null)
            onEnemyDestroyed(this, transform.position);
        Destroy(gameObject);
    }
}
