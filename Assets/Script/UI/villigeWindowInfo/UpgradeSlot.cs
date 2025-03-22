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
    [HideInInspector] public AddressableManager.EquipsImage equipImage;
    [HideInInspector] public AddressableManager.PreviewImage previewImage;


    bool isInitialized;
    [SerializeField] Button[] slotViewers;
    int siblingIndex;

    HeroUpgradeWindow upgradeWindow;
    SetBuildingMat materialBox;
    StorageComponent storageComponent;
    public override void Init()
    {
        upgradeWindow = transform.parent.parent.GetComponent<HeroUpgradeWindow>();
        materialBox = upgradeWindow.SetBuildingMat;
        storageComponent = upgradeWindow.StorageComponent;

        if (type == SlotType.Item)
            itemPreview.upgradeViewerUpdate[transform.GetSiblingIndex()] += UpgradeViewerUpdate;
        else
            skillPreview.upgradeViewerUpdate[transform.GetSiblingIndex()] += UpgradeViewerUpdate;
        siblingIndex = transform.GetSiblingIndex();
    }
    void UpgradeViewerUpdate(int itemLevel)
    {
        //itemLevel이 1이면 업글 하나도 안 된 것.
        AddressableManager.VilligeWindowImage imageType;
        int tempLevel;

        int length = slotViewers.Length;
        Sprite tempImage;
        for (int i = 0; i < length; i++)
        {
            tempLevel = Mathf.Clamp(i - itemLevel, -1, 1) + 1;

            if (tempLevel == 1)
                tempLevel += System.Convert.ToInt32(itemLevel ==
                    GameManager.manager.battleClearManager.SaveDataInfo.playInfo.enableUpgrades[(int)type * 3 + siblingIndex]);

            imageType = (AddressableManager.VilligeWindowImage)tempLevel;

            AddressableManager.manager.GetData(AddressableManager.LabelName.VilligeWindowImage, imageType, out tempImage);
            slotViewers[i].image.sprite = tempImage;
            slotViewers[i].interactable = imageType == AddressableManager.VilligeWindowImage.Upgrade;
        }
    }
    public void OnUpgradeButtonClick(int buttonSiblingIndex)
    {

        CalculateMaterial(buttonSiblingIndex, out bool isBuildable);
        if (!isBuildable)
            return;

        Unit.Hero hero;
        int newLv = buttonSiblingIndex + 1;

        if (type == SlotType.Item)
        {
            hero = itemPreview.interact.mhero;
            hero.EquipsNum[siblingIndex] = newLv;
            itemPreview.SetImage(hero.Getnum, (AddressableManager.ItemQuality)newLv, (AddressableManager.EquipsImage)siblingIndex);
            itemPreview.upgradeViewerUpdate[siblingIndex].Invoke(newLv);
        }
        else
        {
            hero = skillPreview.interact.mhero;
            hero.SkillsNum[transform.GetSiblingIndex()] = newLv;
            skillPreview.upgradeViewerUpdate[siblingIndex].Invoke(newLv);
        }
    }
    void CalculateMaterial(int buttonSiblingIndex, out bool isBuildable)
    {
        int materialIndex;
        SetBuildingMat.MaterialsType materialType = (SetBuildingMat.MaterialsType)type + 1;

        if (type == SlotType.Item)
            materialIndex = (int)equipImage + (buttonSiblingIndex * 3) + 1;
        else
            materialIndex = (int)previewImage + (buttonSiblingIndex * 4) + 1;

        MaterialsData.NeedMaterials materialData = materialBox.GetData(materialIndex, materialType);

        if (!materialBox.isBuildable)
            materialBox.HighLightNotEnoughMaterials(materialIndex, materialType);
        else
            storageComponent.CalculateMaterials(materialData);

        isBuildable = materialBox.isBuildable;
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
