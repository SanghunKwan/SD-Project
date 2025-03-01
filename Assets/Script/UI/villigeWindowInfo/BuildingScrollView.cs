using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingScrollView : MonoBehaviour, IPointerExitHandler
{
    Action action;
    bool isExit = false;
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isExit)
            return;

        action();
        isExit = true;
    }
    public void AddAction(in Action enter)
    {
        action += enter;
    }
}
