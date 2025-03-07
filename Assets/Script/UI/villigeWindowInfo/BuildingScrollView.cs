using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingScrollView : MonoBehaviour, IPointerExitHandler
{
    Action action;
    public void OnPointerExit(PointerEventData eventData)
    {
        action();
    }
    public void AddAction(in Action enter)
    {
        action += enter;
        Debug.Log("³ª°¨");
    }
}
