using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CheckRaycast : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected MouseOnImage raycastMgr;
    public abstract void OnPointerEnter(PointerEventData eventData);

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        raycastMgr.OnPointerExit();
    }
}
