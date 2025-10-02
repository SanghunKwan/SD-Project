using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightClickButton : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        // Selection tracking
        if (IsInteractable() && navigation.mode != Navigation.Mode.None && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Normal, false);
    }
}
