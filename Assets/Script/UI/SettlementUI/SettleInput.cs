using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettleInput : MonoBehaviour, IPointerDownHandler
{
    public Action onClickEvent { get; set; }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Å¬¸¯");
        onClickEvent?.Invoke();
    }
}
