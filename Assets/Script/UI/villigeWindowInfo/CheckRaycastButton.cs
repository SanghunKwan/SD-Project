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
        {
            raycastMgr.OnPointerEnter(eventData.position, equipImage, upgradedNum);
            GameManager.manager.onGetMaterials.eventAction?.Invoke((int)System.Enum.Parse<GameManager.GetMaterialsNum>(equipImage.ToString()), Vector3.zero);
        }
        else
        {
            raycastMgr.OnPointerEnter(eventData.position, previewImage, upgradedNum);
            GameManager.manager.onGetMaterials.eventAction?.Invoke((int)System.Enum.Parse<GameManager.GetMaterialsNum>(previewImage.ToString()), Vector3.zero);
        }

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
