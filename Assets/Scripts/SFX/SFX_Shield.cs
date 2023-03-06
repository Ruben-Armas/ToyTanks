using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Shield : MonoBehaviour
{
    public AudioClip getShield;
    public AudioClip lostShield;

    public float pitchVariation = 11;

    [SerializeField]
    private AudioSource audioSource;

    private AudioClip lastClip;
    private float lastClipStamp;


    private void OnEnable()
    {
        Shield.onActivateShield += OnActivateShield;
        Shield.onDeactivateShield += OnDeactivateShield;
    }

    private void OnDisable()
    {
        Shield.onActivateShield -= OnActivateShield;
        Shield.onDeactivateShield -= OnDeactivateShield;
    }


    private void OnActivateShield()
    {
        PlaySFX(getShield, true, 0.1f);
    }
    private void OnDeactivateShield()
    {
        PlaySFX(lostShield, true, 0.1f);
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
        audioSource.pitch = Random.Range(1 - pitchRange, pitchVariation + pitchRange);
        audioSource.PlayOneShot(clip);
    }
}
