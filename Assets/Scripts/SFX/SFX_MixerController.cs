using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFX_MixerController : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    public void MasterControll(float sliderMaster)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderMaster) * 20);
    }
    public void MusicControll(float sliderMusic)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderMusic) * 20);
    }
    public void EffectsControll(float sliderEffects)
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(sliderEffects) * 20);
    }
}
