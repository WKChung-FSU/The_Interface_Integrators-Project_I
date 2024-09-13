using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    [Header ("------- Audio Source -------")]
    [SerializeField] AudioSource theme;
    //[SerializeField] AudioSource SFXSource;

    [Header("------- Audio Clip -------")]
    public AudioClip background;
    //public AudioClip death;
    //public AudioClip checkpoint;
    //public AudioClip wallTouch;
    //public AudioClip spellpickup;
    //public AudioClip bookPickup;

    private void Start()
    {
       theme.clip = background;
       theme.Play();
    }
}
