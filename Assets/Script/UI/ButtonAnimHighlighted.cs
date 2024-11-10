using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class ButtonAnimHighlighted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] bool active = true;
    [SerializeField] Image lid;
    public bool Set
    {
        get => active;
        set
        {
            active = value;
            button.interactable = !value;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (active)
            button.interactable = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (active)
            button.interactable = false;
    }
}
