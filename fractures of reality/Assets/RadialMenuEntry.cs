using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    
    [SerializeField] RawImage Icon;

    RectTransform Rect;

    private void Start()
    {
        Rect = GetComponent<RectTransform>();
    }
  
  public void SetIcon(Texture pIcon)
    {
        Icon.texture = pIcon;
    }
    public Texture GetIcon()
    {
        return(Icon.texture);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
       

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        
    }
}
