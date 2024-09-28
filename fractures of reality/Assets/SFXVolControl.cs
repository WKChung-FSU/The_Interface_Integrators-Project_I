using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXVolControl : MonoBehaviour
{
    [SerializeField] string sfxvolParam = "MusicVol";
    [SerializeField] AudioMixer sfxmixer;
    [SerializeField] Slider sfxSlider;
    [SerializeField] float mult = 20f;
    [SerializeField] private Toggle sfxMute;
    private bool disablesfxMute;

    private float prevVolume;

    private void Awake()
    {
        sfxSlider.onValueChanged.AddListener(ValueChangedMusic);
        sfxMute.onValueChanged.AddListener(ToggleChangedMusic);
    }

    private void ToggleChangedMusic(bool enable)
    {
        if (disablesfxMute)
        {
            return;
        }

        if (enable)
        {
            sfxSlider.value = prevVolume > sfxSlider.minValue ? prevVolume : sfxSlider.maxValue;
        }

        else
        {
            prevVolume = sfxSlider.value;
            sfxSlider.value = sfxSlider.minValue;
        }
    }


    private void ValueChangedMusic(float val)
    {
        sfxmixer.SetFloat(sfxvolParam, Mathf.Log10(val) * mult);
        disablesfxMute = true;
        sfxMute.isOn = sfxSlider.value > sfxSlider.minValue;
        disablesfxMute =  false;
    }
    
    private void OnDisable()
    {
        sfxmixer.SetFloat(sfxvolParam, sfxSlider.value);
    }

    private void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxvolParam, sfxSlider.value);
    }
}