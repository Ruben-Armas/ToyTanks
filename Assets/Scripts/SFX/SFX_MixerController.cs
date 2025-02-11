using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFX_MixerController : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;
    public TextMeshProUGUI valueMaster;
    public TextMeshProUGUI valueMusic;
    public TextMeshProUGUI valueEffects;

    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderEffects;

    private float _tmpValue;

    //Poner valor en el text
    //Comprobar si existe ese valor guardado
    //  Asignar ese valor
    //Guardarlo
    private void Awake()
    {
        /*float valSaved = PlayerPrefs.GetFloat("MasterVolume", 0);
        Debug.Log(valSaved);
        float valMSaved = PlayerPrefs.GetFloat("MusicVolume", 0);
        Debug.Log($"Music --> {valSaved}");*/

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        float effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 1);
        //audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        sliderMaster.value = masterVolume;
        sliderMusic.value = musicVolume;
        sliderEffects.value = effectsVolume;
    }

    public void MasterControll(float sliderMaster)
    {
        /*//Si hay volumen guardado lo Pone /Sino lo guarda
        float valSaved = PlayerPrefs.GetFloat("MasterVolume", 0);
        if (valSaved >= -1)
            sliderMaster = valSaved;
        else*/
            
        PlayerPrefs.SetFloat("MasterVolume", sliderMaster);

        //Slider / Mixer
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderMaster) * 20);
        //Valor en %
        _tmpValue = sliderMaster * 100;
        valueMaster.text = $"{_tmpValue.ToString("F0")} %";
    }
    public void MusicControll(float sliderMusic)
    {
        PlayerPrefs.SetFloat("MusicVolume", sliderMusic);

        //Slider / Mixer
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderMusic) * 20);
        //Valor en %
        _tmpValue = sliderMusic * 100;
        valueMusic.text = $"{_tmpValue.ToString("F0")} %";
    }
    public void EffectsControll(float sliderEffects)
    {
        PlayerPrefs.SetFloat("EffectsVolume", sliderEffects);

        //Slider / Mixer
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(sliderEffects) * 20);
        //Valor en %
        _tmpValue = sliderEffects * 100;
        valueEffects.text = $"{_tmpValue.ToString("F0")} %";
    }
}
