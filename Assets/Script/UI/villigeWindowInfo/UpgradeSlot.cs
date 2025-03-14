using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : InitObject
{
    public enum SlotType
    {
        Item,
        Skill
    }
    [HideInInspector] public SlotType type;
    [HideInInspector] public ItemPreview itemPreview;
    [HideInInspector] public SkillPreview skillPreview;


    bool isInitialized;
    [SerializeField] Button[] slotViewers;

    public override void Init()
    {
        if (type == SlotType.Item)
            itemPreview.upgradeViewerUpdate[transform.GetSiblingIndex()] += UpgradeViewerUpdate;
        else
            skillPreview.upgradeViewerUpdate[transform.GetSiblingIndex()] += UpgradeViewerUpdate;
    }
    void UpgradeViewerUpdate(int itemLevel)
    {
        //itemLevel이 1이면 업글 하나도 안 된 것.
        AddressableManager.VilligeWindowImage imageType;

        int length = slotViewers.Length;
        Sprite tempImage;
        for (int i = 0; i < length; i++)
        {
            imageType = (AddressableManager.VilligeWindowImage)Mathf.Clamp(i - itemLevel + 1, -1, 1) + 1;
            AddressableManager.manager.GetData(AddressableManager.LabelName.VilligeWindowImage, imageType, out tempImage);
            slotViewers[i].image.sprite = tempImage;
            slotViewers[i].interactable = imageType == AddressableManager.VilligeWindowImage.Upgrade;
        }
    }
    private void OnValidate()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        if (transform.parent == null)
            return;

        transform.parent.TryGetComponent(out itemPreview);
        transform.parent.TryGetComponent(out skillPreview);

        slotViewers = transform.Find("UpgradeViewer").GetComponentsInChildren<Button>();
    }


}
