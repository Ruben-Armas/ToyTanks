using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Bullet : MonoBehaviour
{
    public AudioClip bulletShoot;
    public AudioClip bulletBounce;

    public float pitchVariation = 11;

    [SerializeField]
    private AudioSource audioSource;

    private AudioClip lastClip;
    private float lastClipStamp;


    private void OnEnable()
    {
        Bullet.onBulletSFXShoot += OnBulletSFXShoot;
        Bullet.onBulletSFXBounce += OnBulletSFXBounce;
    }

    private void OnDisable()
    {
        Bullet.onBulletSFXShoot -= OnBulletSFXShoot;
        Bullet.onBulletSFXBounce -= OnBulletSFXBounce;
    }


    private void OnBulletSFXShoot()
    {
        PlaySFX(bulletShoot, 0.1f);
    }
    private void OnBulletSFXBounce()
    {
        PlaySFX(bulletBounce, 0.1f);
    }

    //Para evitar el acople
    void PlaySFX(AudioClip clip, float pitchRange = 0)
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
        audioSource.pitch = Random.Range(1 - pitchRange, pitchVariation + pitchRange);
        audioSource.PlayOneShot(clip);
    }
}
