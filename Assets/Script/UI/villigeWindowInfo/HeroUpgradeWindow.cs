using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUpgradeWindow : InitObject
{
    public enum UpgradeType
    {
        None,
        Equip,
        Skill,
        Max
    }
    [SerializeField] Button button;
    [Space]
    [SerializeField] ItemPreview itemPreview;
    [SerializeField] SkillPreview skillPreview;
    [Space]
    [SerializeField] SetBuildingMat setBuildingMat;
    public SetBuildingMat SetBuildingMat { get { return setBuildingMat; } }
    [SerializeField] StorageComponent storageComponent;
    public StorageComponent StorageComponent { get { return storageComponent; } }
    [Space]
    [SerializeField] UpgradeType[] buildIndex2heroUpgrade;

    public override void Init()
    {
        button.onClick.AddListener(OnButtonEvent);
        itemPreview.Awake();
        skillPreview.Awake();
    }
    private void OnDisable()
    {
        OnHeroAllocated(false);
    }
    public void OnButtonEvent()
    {
        OnHeroAllocated(!gameObject.activeSelf);
    }
    public void OnHeroAllocated(bool onoff)
    {
        gameObject.SetActive(onoff);

        if (onoff)
        {
            AddressableLoadData();
        }

    }
    public void OnHeroDeallocated(bool onoff)
    {
        if (!onoff)
            OnHeroAllocated(false);
    }
    void SetHeroUpradeWindow(UpgradeType type, bool onoff = true)
    {
        if (type == UpgradeType.None)
            return;

        int length = (int)UpgradeType.Max;

        for (int i = 1; i < length; i++)
        {
            transform.GetChild(i - 1).gameObject.SetActive((UpgradeType)i == type);
        }
    }

    public void SetWindowType(AddressableManager.BuildingImage type)
    {
        SetHeroUpradeWindow(buildIndex2heroUpgrade[(int)type]);
    }
    public void SetHero(Unit.Hero hero)
    {
        itemPreview.ActiveItemPreview(hero);
        skillPreview.ActivateSkillPreview(hero);
    }

    void AddressableLoadData()
    {
        //if(AddressableManager.manager.)

        AddressableManager.manager.LoadData("RangeNormal");
        AddressableManager.manager.LoadData("MeleeNormal");
    }
}
