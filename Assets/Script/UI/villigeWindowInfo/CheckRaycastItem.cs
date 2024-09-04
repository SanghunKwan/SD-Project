using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRaycastItem : CheckRaycast
{
    [SerializeField] AddressableManager.EquipsImage type;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        raycastMgr.OnPointerEnter(eventData.position, type);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
