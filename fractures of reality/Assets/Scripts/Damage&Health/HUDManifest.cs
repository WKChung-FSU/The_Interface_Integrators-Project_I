using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

[CreateAssetMenu]
public class HUDManifest : ScriptableObject
{
    [Header("Icons")]
    [SerializeField] public Sprite defaultIco;
    [SerializeField] public Sprite fireIco;
    [SerializeField] public Sprite lightningIco;
    [SerializeField] public Sprite iceIco;
    [SerializeField] public Sprite windIco;
    [SerializeField] public Sprite earthIco;
    [SerializeField] public Sprite waterIco;

}
