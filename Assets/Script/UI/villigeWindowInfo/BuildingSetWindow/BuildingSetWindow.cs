using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine.EventSystems;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;

public class BuildingSetWindow : InitInterface
{
    public BuildingComponent buildingComponent { get; private set; }

    Image buildingImage;
    Image upgradeImage;
    TextMeshProUGUI buildingName;
    TextMeshProUGUI infoText;

    [SerializeField] AddressableManager addressableManager;
    [SerializeField] MaterialsData materialsData;
    [SerializeField] ThreeBools[] children;

    class BuildSetCharacter
    {
        public TextMeshProUGUI team;
        public TextMeshProUGUI heroName;
        string defaultName;
        public GameObject gameObject { get; private set; }

        public BuildSetCharacter(Transform tr)
        {
            team = tr.GetChild(0).GetComponent<TextMeshProUGUI>();
            heroName = tr.GetChild(1).GetComponent<TextMeshProUGUI>();
            gameObject = tr.gameObject;
            defaultName = heroName.text;
        }
        public void ResetTeam()
        {
            team.text = "";
            heroName.text = defaultName;
        }
        public void ChangeTeam(in string nameText, in string teamText)
        {

            team.text = teamText;
            heroName.text = nameText;
        }
    }
    [Serializable]
    class ThreeBools
    {
        public bool isHeroNeed;
        public bool isSubManagerNeed;
        public bool isUpgradeNeed;
    }

    BuildSetCharacter selectHero;
    BuildSetCharacter manager;
    BuildSetCharacter subManager;

    Dictionary<GameObject, BuildSetCharacter> buildSetDic = new Dictionary<GameObject, BuildSetCharacter>();

    public bool isDrag { get; set; }
    public villigeInteract vill_Interact { get; set; }
    public Action DragEnd;
    [SerializeField] GameObject[] UIObjectToggle;
    [SerializeField] GameObject[] UIObjectClose;

    public override void Init()
    {
        buildingImage = transform.Find("BuildingName").GetChild(0).GetComponent<Image>();
        buildingName = buildingImage.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();

        infoText = transform.Find("NoSelectInfo").GetComponent<TextMeshProUGUI>();

        upgradeImage = transform.Find("Upgrade").GetChild(0).GetComponent<Image>();

        selectHero = new BuildSetCharacter(transform.Find("SelectHero"));
        manager = new BuildSetCharacter(transform.Find("manager"));
        subManager = new BuildSetCharacter(transform.Find("SubManager"));

        buildSetDic.Add(selectHero.gameObject, selectHero);
        buildSetDic.Add(manager.gameObject, manager);
        buildSetDic.Add(subManager.gameObject, subManager);
    }

    public void SetOpen(BuildingComponent buildingComp, bool onoff, AddressableManager.BuildingImage buildType,
        AddressableManager.BuildingImage upgradeType, in string str, villigeInteract[] saveData)
    {
        buildingComponent = buildingComp;
        gameObject.SetActive(onoff);
        SetBuildingNameImg(buildType);

        int typeIndex = (int)buildType;

        SetBuildingNameText(typeIndex);
        SetChild(typeIndex);

        infoText.text = str;

        addressableManager.GetData("Building", upgradeType, out Sprite sprite);
        upgradeImage.sprite = sprite;

        LoadNeed(saveData);
    }
    void SetBuildingNameImg(in AddressableManager.BuildingImage buildType)
    {
        addressableManager.GetData("Building", buildType, out Sprite sprite);
        buildingImage.sprite = sprite;
    }
    void SetBuildingNameText(int type)
    {
        string str = materialsData.data.Needs[type + 1].name;
        buildingName.text = str.Substring(0, str.Length - 3);
    }
    void SetChild(int type)
    {
        NeedHero(children[type].isHeroNeed);
        NeedSubManager(children[type].isSubManagerNeed);
        NeedUpgrade(children[type].isUpgradeNeed);
    }

    void NeedHero(bool need)
    {
        infoText.gameObject.SetActive(!need);
        selectHero.gameObject.SetActive(need);
    }
    void NeedSubManager(bool need)
    {
        subManager.gameObject.SetActive(need);
    }
    void NeedUpgrade(bool need)
    {
        upgradeImage.transform.parent.gameObject.SetActive(need);
    }
    void LoadNeed(villigeInteract[] saveData)
    {
        LoadRepeat(selectHero, saveData[0]);
        LoadRepeat(manager, saveData[1]);
        LoadRepeat(subManager, saveData[2]);

    }
    void LoadRepeat(BuildSetCharacter buildset, villigeInteract data)
    {
        if (data == null)
            buildset.ResetTeam();
        else
            buildset.ChangeTeam(data.hero.stat.NAME, data.hero.keycode);
    }
    public void ToggleWindow()
    {
        buildingComponent.ToggleWindow();
    }

    public void SetHeroInDic(GameObject key)
    {
        BuildSetCharacter tempCharacter = buildSetDic[key];

        tempCharacter.ChangeTeam(vill_Interact.hero.stat.NAME, vill_Interact.hero.keycode);
        buildingComponent.SaveData(vill_Interact, key.transform.GetSiblingIndex());


    }
    public void SetBackHeroInDic(GameObject key)
    {
        buildSetDic[key].ResetTeam();
        buildingComponent.ResetData(key.transform.GetSiblingIndex());
    }

    public void SetisDragFalse()
    {
        isDrag = false;

        DragEnd();
    }
    public void AddDragEnd(Action action)
    {
        DragEnd += action;
    }

    public void Collider_UIActive(bool onoff)
    {
        foreach (var item in UIObjectToggle)
        {
            item.SetActive(onoff);
        }
        foreach (var item in UIObjectClose)
        {
            item.SetActive(false);
        }
    }
}
