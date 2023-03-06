using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Tank : MonoBehaviour
{
    public AudioClip tankDestroy;
    //public AudioClip tank;

    //public float pitchVariation = 2;

    [SerializeField]
    private AudioSource audioSourceStatic;
    //[SerializeField]
    //private AudioSource audioSource;

    private AudioClip lastClip;
    private float lastClipStamp;


    private void OnEnable()
    {
        Player.onPlayerSoundDestroy += OnPlayerSoundDestroy;
        Enemy.onEnemySoundDestroy += OnEnemySoundDestroy;
    }

    private void OnDisable()
    {
        Player.onPlayerSoundDestroy -= OnPlayerSoundDestroy;
        Enemy.onEnemySoundDestroy -= OnEnemySoundDestroy;
    }


    private void OnPlayerSoundDestroy()
    {
        PlaySFX(tankDestroy, false, 0);
    }
    private void OnEnemySoundDestroy()
    {
        PlaySFX(tankDestroy, false, 0);
    }
    private void OnBulletSFXBounce()
    {
        //PlaySFX(bulletBounce, true, 0.1f);
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
        /*if (changePitch)
        {
            audioSource.pitch = Random.Range(1 - pitchRange, pitchVariation + pitchRange);
            audioSource.PlayOneShot(clip);
        }
        else*/
            audioSourceStatic.PlayOneShot(clip);
    }
}
