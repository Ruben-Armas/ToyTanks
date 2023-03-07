using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FXManager : MonoBehaviour
{
    public GameObject tankDestroyActivatedParticles;
    private IObjectPool<ParticleSystem> _tankDestroyActivatedPool;

    public GameObject bulletDestroyActivatedParticles;
    private IObjectPool<ParticleSystem> _bulletDestroyActivatedPool;

    public GameObject bulletBounceActivatedParticles;
    private IObjectPool<ParticleSystem> _bulletBounceActivatedPool;

    private void Awake()
    {
        _tankDestroyActivatedPool = new ObjectPool<ParticleSystem>(OnCreateTDAP);
        _bulletDestroyActivatedPool = new ObjectPool<ParticleSystem>(OnCreateBDAP);
        _bulletBounceActivatedPool = new ObjectPool<ParticleSystem>(OnCreateBBAP);
    }

    ParticleSystem OnCreateTDAP()
    {
        GameObject ps = Instantiate(tankDestroyActivatedParticles, Vector3.one * 1000, Quaternion.identity);
        ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
        ReturnToPool rtp = ps.GetComponent<ReturnToPool>();
        rtp.pool = _tankDestroyActivatedPool;
        return particleSystem;
    }
    ParticleSystem OnCreateBDAP()
    {
        GameObject ps = Instantiate(bulletDestroyActivatedParticles, Vector3.one * 1000, Quaternion.identity);
        ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
        ReturnToPool rtp = ps.GetComponent<ReturnToPool>();
        rtp.pool = _bulletDestroyActivatedPool;
        return particleSystem;
    }
    ParticleSystem OnCreateBBAP()
    {
        GameObject ps = Instantiate(bulletBounceActivatedParticles, Vector3.one * 1000, Quaternion.identity);
        ParticleSystem particleSystem = ps.GetComponent<ParticleSystem>();
        ReturnToPool rtp = ps.GetComponent<ReturnToPool>();
        rtp.pool = _bulletBounceActivatedPool;
        return particleSystem;
    }

    //SUSCRIPCIÓN al EVENTO
    void OnEnable()
    {
        Player.onPlayerEffectDestroy += OnTankFXDestroy;
        Enemy.onEnemyEffectDestroy += OnTankFXDestroy;
        Bullet.onBulletFXDestroy += OnBulletFXDestroy;
        Bullet.onBulletFXBounce += OnBulletFXBounce;
    }
    //DESUSCRIPCIÓN al EVENTO
    void OnDisable()
    {
        Player.onPlayerEffectDestroy -= OnTankFXDestroy;
        Enemy.onEnemyEffectDestroy -= OnTankFXDestroy;
        Bullet.onBulletFXDestroy -= OnBulletFXDestroy;
        Bullet.onBulletFXBounce -= OnBulletFXBounce;
    }
    //DELEGADOS
    private void OnTankFXDestroy(Vector3 position)
    {
        ParticleSystem ps = _tankDestroyActivatedPool.Get();
        ps.transform.position = position;
        //Debug.Log("position: " + position);
        //Debug.Log("Particle position: " + ps.transform.position);
        ps.Play();
    }
    private void OnBulletFXBounce(Vector3 position)
    {
        ParticleSystem ps = _bulletBounceActivatedPool.Get();
        ps.transform.position = position;
        //Debug.Log("position: " + position);
        //Debug.Log("Particle bullet bounce position: " + ps.transform.position);
        ps.Play();
    }
    private void OnBulletFXDestroy(Vector3 position)
    {
        ParticleSystem ps = _bulletDestroyActivatedPool.Get();
        ps.transform.position = position;
        //Debug.Log("position: " + position);
        //Debug.Log("Particle bullet position: " + ps.transform.position);
        ps.Play();
    }
}
