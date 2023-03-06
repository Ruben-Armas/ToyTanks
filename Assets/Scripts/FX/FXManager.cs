using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FXManager : MonoBehaviour
{
    public GameObject tankActivatedParticles;
    private IObjectPool<ParticleSystem> _tankActivatedPool;

    public GameObject bulletActivatedParticles;
    private IObjectPool<ParticleSystem> _bulletActivatedPool;


    private void Awake()
    {
        _tankActivatedPool = new ObjectPool<ParticleSystem>(OnCreateTAP);
        _bulletActivatedPool = new ObjectPool<ParticleSystem>(OnCreateBAP);
    }

    ParticleSystem OnCreateTAP()
    {
        GameObject ps = Instantiate(tankActivatedParticles, Vector3.one * 1000, Quaternion.identity);
        ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
        ReturnToPool rtp = ps.GetComponent<ReturnToPool>();
        rtp.pool = _tankActivatedPool;
        return particleSystem;
    }
    ParticleSystem OnCreateBAP()
    {
        GameObject ps = Instantiate(bulletActivatedParticles, Vector3.one * 1000, Quaternion.identity);
        ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
        ReturnToPool rtp = ps.GetComponent<ReturnToPool>();
        rtp.pool = _bulletActivatedPool;
        return particleSystem;
    }


    //SUSCRIPCIÓN al EVENTO
    void OnEnable()
    {
        Player.onPlayerEffectDestroy += OnEffectsDestroy;
        Enemy.onEnemyEffectDestroy += OnEffectsDestroy;
        Bullet.onBulletFXDestroy += OnBulletFXDestroy;
    }
    //DESUSCRIPCIÓN al EVENTO
    void OnDisable()
    {
        Player.onPlayerEffectDestroy -= OnEffectsDestroy;
        Enemy.onEnemyEffectDestroy -= OnEffectsDestroy;
        Bullet.onBulletFXDestroy -= OnBulletFXDestroy;
    }
    //DELEGADOS
    private void OnEffectsDestroy(Vector3 position)
    {
        ParticleSystem ps = _tankActivatedPool.Get();
        ps.transform.position = position;
        Debug.Log("position: " + position);
        Debug.Log("Particle position: " + ps.transform.position);
        ps.Play();
    }
    private void OnBulletFXDestroy(Vector3 position)
    {
        ParticleSystem ps = _bulletActivatedPool.Get();
        ps.transform.position = position;
        Debug.Log("position: " + position);
        Debug.Log("Particle bullet position: " + ps.transform.position);
        ps.Play();
    }
}
