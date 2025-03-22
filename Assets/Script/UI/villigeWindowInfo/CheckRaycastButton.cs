using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRaycastButton : CheckRaycast
{
    UpgradeSlot upgradeSlot;
    UpgradeSlot.SlotType type;
    AddressableManager.EquipsImage equipImage;
    AddressableManager.PreviewImage previewImage;
    [SerializeField] int upgradedNum;
    private void Awake()
    {
        upgradeSlot = transform.parent.parent.GetComponent<UpgradeSlot>();
        type = upgradeSlot.type;
        equipImage = upgradeSlot.equipImage;
        previewImage = upgradeSlot.previewImage;
    }
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
