using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFX_Load : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    private void Awake()
    {
        LoadSFX();
    }
    /*private void Update()
    {
        float m;
        audioMixer.GetFloat("MusicVolume", out m);
        Debug.Log($"Music --> {m}");
    }*/

    public void LoadSFX()
    {
        Debug.Log("----CARGANDO SONIDO----");
        //Si hay volumen guardado lo Pone
        float MasterSaved = PlayerPrefs.GetFloat("MasterVolume", 0);
        //if (MasterSaved > -1)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterSaved) * 20);

        //Si hay volumen guardado lo Pone
        float MusicSaved = PlayerPrefs.GetFloat("MasterVolume", 0);
        if (MusicSaved > -1)
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicSaved) * 20);


        //Si hay volumen guardado lo Pone
        float EffectsSaved = PlayerPrefs.GetFloat("EffectsVolume", 0);
        if (EffectsSaved > -1)
            audioMixer.SetFloat("EffectsVolume", Mathf.Log10(EffectsSaved) * 20);
    }
}
