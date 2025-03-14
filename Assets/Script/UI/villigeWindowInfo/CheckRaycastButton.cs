using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRaycastButton : CheckRaycast
{
    [HideInInspector] public UpgradeSlot.SlotType type;
    [HideInInspector] public AddressableManager.EquipsImage equipImage;
    [HideInInspector] public AddressableManager.PreviewImage previewImage;
    [SerializeField] int upgradedNum;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (type == UpgradeSlot.SlotType.Item)
            raycastMgr.OnPointerEnter(eventData.position, equipImage, upgradedNum);
        else
            raycastMgr.OnPointerEnter(eventData.position, previewImage, upgradedNum);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
