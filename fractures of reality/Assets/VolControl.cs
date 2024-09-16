using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolControl : MonoBehaviour
{
    [SerializeField] string volParam = "MasterVol";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] float mult = 20f;
    [SerializeField] private Toggle musicMute;
    private bool disableMusicMute;
    private bool disableSFXMute;


    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(ValueChangedMusic);
        musicMute.onValueChanged.AddListener(ToggleChangedMusic);
    }

    private void ToggleChangedMusic(bool enable)
    {
        if (disableMusicMute)
        {
            return;
        }

        if (enable)
        {
            musicSlider.value = musicSlider.maxValue;
        }

        else
        {
            musicSlider.value = musicSlider.minValue;
        }
    }


    private void ValueChangedMusic(float val)
    {
        mixer.SetFloat(volParam, Mathf.Log10(val) * mult);
        disableMusicMute = true;
        musicMute.isOn = musicSlider.value > musicSlider.minValue;
        disableMusicMute =  false;
    }
    
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volParam, musicSlider.value);
    }

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(volParam, musicSlider.value);
    }
}