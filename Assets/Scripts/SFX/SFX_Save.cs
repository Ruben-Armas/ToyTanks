using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFX_Save : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    private float _tmpMasterVal;
    private float _tmpMusicVal;
    private float _tmpEffectsVal;
    public void SaveSFX()
    {
        Debug.Log("----GUARDANDO SONIDO----");

        audioMixer.GetFloat("MasterVolume", out _tmpMasterVal);
        PlayerPrefs.SetFloat("MasterVolume", _tmpMasterVal);

        audioMixer.GetFloat("MusicVolume", out _tmpMusicVal);
        PlayerPrefs.SetFloat("MusicVolume", _tmpMusicVal);

        audioMixer.GetFloat("EffectsVolume", out _tmpEffectsVal);
        PlayerPrefs.SetFloat("EffectsVolume", _tmpEffectsVal);
    }
}
