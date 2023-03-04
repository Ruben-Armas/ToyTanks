using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Bullet : MonoBehaviour
{
    public AudioClip bulletShoot;
    public AudioClip bulletBounce;
    //public AudioClip bulletDestroy;

    public float pitchVariation = 11;

    [SerializeField]
    private AudioSource audioSourceStatic;
    [SerializeField]
    private AudioSource audioSource;

    private AudioClip lastClip;
    private float lastClipStamp;


    private void OnEnable()
    {
        Bullet.onBulletSFXShoot += OnBulletSFXShoot;
        Bullet.onBulletSFXBounce += OnBulletSFXBounce;
        //Bullet.onBulletSFXDestroy += OnBulletSFXDestroy;
    }

    private void OnDisable()
    {
        Bullet.onBulletSFXShoot -= OnBulletSFXShoot;
        Bullet.onBulletSFXBounce -= OnBulletSFXBounce;
        //Bullet.onBulletSFXDestroy -= OnBulletSFXDestroy;
    }


    private void OnBulletSFXShoot()
    {
        PlaySFX(bulletShoot, false, 0);
    }
    private void OnBulletSFXBounce()
    {
        PlaySFX(bulletBounce, true, 0.1f);
    }
    private void OnBulletSFXDestroy()
    {
        PlaySFX(bulletBounce, true, 0.1f);
    }

    //Para evitar el acople
    void PlaySFX(AudioClip clip, bool changePitch, float pitchRange = 0)
    {
        //Compruebo si el audio es el mismo que el anterior y cuánto hace que lo reproduje
        if (clip == lastClip)
        {
            if (Time.time - lastClipStamp < 0.1f)
                return;
        }
        lastClip = clip;
        lastClipStamp = Time.time;

        //Play
        if (changePitch)
        {
            audioSource.pitch = Random.Range(1 - pitchRange, pitchVariation + pitchRange);
            audioSource.PlayOneShot(clip);
        }
        else
            audioSourceStatic.PlayOneShot(clip);
    }
}
