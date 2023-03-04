using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FXManager : MonoBehaviour
{
    public GameObject tankActivatedParticles;
    private IObjectPool<ParticleSystem> tankActivatedPool;


    private void Awake()
    {
        tankActivatedPool = new ObjectPool<ParticleSystem>(OnCreateBAP);
    }

    ParticleSystem OnCreateBAP()
    {
        GameObject ps = Instantiate(tankActivatedParticles, Vector3.one * 1000, Quaternion.identity);
        ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
        ReturnToPool rtp = ps.GetComponent<ReturnToPool>();
        rtp.pool = tankActivatedPool;
        return particleSystem;
    }


    //SUSCRIPCIÓN al EVENTO
    void OnEnable()
    {
        Player.onPlayerEffectDestroy += OnEffectsDestroy;
        Enemy.onEnemyEffectDestroy += OnEffectsDestroy;
    }
    //DESUSCRIPCIÓN al EVENTO
    void OnDisable()
    {
        Player.onPlayerEffectDestroy -= OnEffectsDestroy;
        Enemy.onEnemyEffectDestroy -= OnEffectsDestroy;
    }
    //DELEGADOS
    private void OnEffectsDestroy(Vector3 position)
    {
        //PlaySFX(tankDestroy, false, 0);
        ParticleSystem ps = tankActivatedPool.Get();
        ps.transform.position = position;
        Debug.Log("position: " + position);
        Debug.Log("Particle position: " + ps.transform.position);
        ps.Play();
    }
}
