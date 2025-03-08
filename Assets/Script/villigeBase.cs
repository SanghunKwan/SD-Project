using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class villigeBase : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    protected ScrollRect scrollRect;
    public Image image { get; protected set; }


    private void Awake()
    {
        image = GetComponent<Image>();
        VirtualAwake();
    }
    protected abstract void VirtualAwake();
    protected virtual void Start()
    {
        scrollRect = transform.parent.parent.parent.GetComponentInParent<ScrollRect>();

    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }
}
