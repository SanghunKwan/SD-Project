using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerdownTransfer : MonoBehaviour, IPointerDownHandler
{
    IPointerDownHandler handler;
    [SerializeField] Transform targetHandler;
    private void Awake()
    {
        handler = targetHandler.GetComponent<IPointerDownHandler>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        handler.OnPointerDown(eventData);
    }
}
