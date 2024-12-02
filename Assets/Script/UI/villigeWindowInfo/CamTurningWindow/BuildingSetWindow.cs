using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using SDUI;

public class BuildingSetWindow : CamTuringWindow
{
    public BuildingComponent buildingComponent { get; private set; }

    Image buildingImage;
    Image upgradeImage;
    TextMeshProUGUI buildingName;
    TextMeshProUGUI infoText;

    [SerializeField] AddressableManager addressableManager;
    [SerializeField] MaterialsData materialsData;
    [SerializeField] ThreeBools[] children;


    [Serializable]
    class ThreeBools
    {
        public bool isHeroNeed;
        public bool isSubManagerNeed;
        public bool isUpgradeNeed;
    }

    BuildSetCharacter[] buildSetCharacters;

    BuildSetCharacter selectHero;
    BuildSetCharacter manager;
    BuildSetCharacter subManager;

    Dictionary<GameObject, BuildSetCharacter> buildSetDic = new Dictionary<GameObject, BuildSetCharacter>();

    public bool isDrag { get; set; }
    public villigeInteract vill_Interact { get; set; }
    public Action DragEnd = () => { };

    AddressableManager.BuildingImage ImageIndex;
    public AddressableManager AddressableManager { get { return addressableManager; } }

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

        buildSetCharacters = new BuildSetCharacter[] { selectHero, manager, subManager };
    }

    public void SetOpen(BuildingComponent buildingComp, bool onoff, AddressableManager.BuildingImage buildType,
        AddressableManager.BuildingImage upgradeType, in string str, villigeInteract[] saveData)
    {
        gameObject.SetActive(onoff);

        if (onoff)
        {
            buildingComponent = buildingComp;
            ImageIndex = buildType;
            SetBuildingNameImg(ImageIndex);

            int typeIndex = (int)ImageIndex;

            SetBuildingNameText(typeIndex);
            SetChild(typeIndex);

            infoText.text = str;

            addressableManager.GetData(AddressableManager.LabelName.Building, upgradeType, out Sprite sprite);
            upgradeImage.sprite = sprite;

            LoadNeed(saveData);
        }


    }
    void SetBuildingNameImg(in AddressableManager.BuildingImage buildType)
    {
        addressableManager.GetData(AddressableManager.LabelName.Building, buildType, out Sprite sprite);
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
    public void SetHeroInDic(GameObject key)
    {
        buildSetDic[key].ChangeTeam(vill_Interact.hero.stat.NAME, vill_Interact.hero.keycode);
        vill_Interact.NoMove();

    }
    public void SetBackHeroText(GameObject key)
    {
        if (buildingComponent.IsDataNull(key.transform.GetSiblingIndex(), out villigeInteract saveVillige))
            buildSetDic[key].ResetTeam();
        else
            buildSetDic[key].ChangeTeam(saveVillige.hero.stat.NAME, saveVillige.hero.keycode);
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
    public void SaveHeroData(GameObject key)
    {
        int siblingIndex = key.transform.GetSiblingIndex();

        if (vill_Interact.isCanLoad(out BuildingComponent beforeWork, out int beforeIndex) && siblingIndex != 0)
        {
            if (beforeWork == buildingComponent)
            {
                buildSetCharacters[beforeIndex].ResetTeam();
                buildSetDic[transform.GetChild(beforeIndex).gameObject].ResetTeam();
            }

            beforeWork.ResetData(beforeIndex);

        }

        buildingComponent.SaveData(vill_Interact, siblingIndex);
        if (siblingIndex != 0)
        {
            vill_Interact.SaveWorkPlace(buildingComponent, siblingIndex);
            vill_Interact.ChangeImage(ImageIndex);
            vill_Interact.hero.alloBuilding(ImageIndex);
        }
    }
    public void BuildSetCharactersCheck(int index, villigeInteract vill)
    {
        buildSetCharacters[index].ChangeTeam(vill.hero.stat.NAME, vill.hero.keycode);
    }
}
