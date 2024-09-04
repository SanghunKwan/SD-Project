using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRaycastSkill : CheckRaycast
{
    [SerializeField] AddressableManager.PreviewImage previewImage;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        raycastMgr.OnPointerEnter(eventData.position, previewImage);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
