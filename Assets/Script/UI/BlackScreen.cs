using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class BlackScreen : MonoBehaviour, IPointerClickHandler
{
    Action action;
    Action onButtonDelay;
    bool buttonDelayReady = false;

    [SerializeField] KeyWindow keyWindow;
    private void OnEnable()
    {
        InputSystem.onAnyButtonPress.CallOnce(OnAnyButtonPressed);
    }
    public void GetActionClick(Action apiAction)
    {
        action = apiAction;
        ObjectOnoff(true);
    }
    void ObjectOnoff(bool onoff)
    {
        gameObject.SetActive(onoff);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Escape();
    }
    public void Escape()
    {
        ObjectOnoff(false);
        action();
    }

    public void OnAnyButtonPressed(InputControl control)
    {
#if PLATFORM_STANDALONE_WIN
        if (control.device != Keyboard.current.device || control.displayName == "Esc")
            return;

#endif

        onButtonDelay = () =>
        {
            keyWindow.ChangeKey(control);
        };
        buttonDelayReady = true;

    }

    public void OnAnyKeyEnd(InputAction.CallbackContext callback)
    {
        Debug.Log("end");
        if (!callback.canceled && buttonDelayReady)
            return;

        onButtonDelay();
        buttonDelayReady = false;
    }
}
