using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ButtonInteractableHighlighted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] bool active = true;
    public bool Set
    {
        get => active;
        set
        {
            active = value;
            button.interactable = !value;
        }
    }
    public Button GetButton
    {
        get { return button; }
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
