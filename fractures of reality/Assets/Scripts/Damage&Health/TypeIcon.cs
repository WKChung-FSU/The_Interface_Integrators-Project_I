using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DamageEngine;

public class TypeIcon : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField] GameObject icon;
    [SerializeField] Sprite normalIco;
    [SerializeField] Sprite fireIco;
    [SerializeField] Sprite lightningIco;
    [SerializeField] Sprite iceIco;
    [SerializeField] Sprite windIco;
    [SerializeField] Sprite earthIco;
    [SerializeField] Sprite waterIco;

    public void EnableElementTypeGraphic(DamageEngine.ElementType type)
    {
        switch (type)
        {
            case DamageEngine.ElementType.Normal:
                {
                    //enable the default image
                    icon.GetComponent<Image>().sprite = normalIco;
                    break;
                }
            case DamageEngine.ElementType.fire:
                {
                    icon.GetComponent<Image>().sprite = fireIco;
                    break;
                }
            case DamageEngine.ElementType.Lightning:
                {
                    icon.GetComponent<Image>().sprite = lightningIco;
                    break;
                }
            case DamageEngine.ElementType.Ice:
                {
                    icon.GetComponent<Image>().sprite = iceIco;
                    break;
                }
            case DamageEngine.ElementType.Earth:
                {
                    icon.GetComponent<Image>().sprite = earthIco;
                    break;
                }
            case DamageEngine.ElementType.Wind_tempHeal:
                {
                    icon.GetComponent<Image>().sprite = windIco;
                    break;
                }
            case DamageEngine.ElementType.Water:
                {
                    icon.GetComponent<Image>().sprite = waterIco;
                    break;
                }
        }
    }

}
