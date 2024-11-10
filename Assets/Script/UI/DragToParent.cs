using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToParent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] DragFromChild parentHandler;
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentHandler.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentHandler.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentHandler.OnEndDrag(eventData);
    }
}
