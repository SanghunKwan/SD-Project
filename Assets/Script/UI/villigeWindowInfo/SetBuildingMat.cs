using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SetBuildingMat : MonoBehaviour
{
    public enum MaterialsType
    {
        BuildingNeed,
        UpgradeNeed,
        SkillUpgradeNeed
    }

    [SerializeField] MaterialsData manager;
    TextMeshProUGUI title;

    [Serializable]
    public class Materials
    {
        public GameObject[] gameobj;
        public TextMeshProUGUI numText;
    }

    [SerializeField] Materials[] materials;
    TextMeshProUGUI gold;
    TextMeshProUGUI time;
    TextMeshProUGUI description;
    [SerializeField] VilligeStorage villigeStorage;
    [SerializeField] Color[] textColors;
    RectTransform rectTransform;
    float defaultHeight;


    public bool isBuildable { get; private set; }

    private void Awake()
    {
        title = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        gold = title.transform.Find("Gold&Time").Find("GoldText").GetComponent<TextMeshProUGUI>();
        time = gold.transform.parent.Find("TimeText").GetComponent<TextMeshProUGUI>();
        description = title.transform.Find("desc").GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        defaultHeight = rectTransform.sizeDelta.y;
    }
    public MaterialsData.NeedMaterials GetData(int materialIndex, MaterialsType materialsType)
    {
        MaterialsData.NeedMaterials needMaterials = manager.data.needsArray[(int)materialsType][materialIndex];
        GetData(needMaterials);
        return needMaterials;
    }
    void GetData(MaterialsData.NeedMaterials needMaterial)
    {
        isBuildable = true;

        bool isMidMaterialExist =
        SetActiveMaterialsDesc(0, needMaterial.grayNum) |
        SetActiveMaterialsDesc(1, needMaterial.timberNum) |
        SetActiveMaterialsDesc(2, needMaterial.blackNum) |
        SetActiveMaterialsDesc(3, needMaterial.whiteNum);

        gold.text = needMaterial.money.ToString();
        time.text = needMaterial.turn.ToString();
        description.text = needMaterial.desc;

        CompareCountToChangeColor(needMaterial);
        SetRectTransformSize(isMidMaterialExist);
    }
    void CompareCountToChangeColor(MaterialsData.NeedMaterials needMaterial)
    {
        CompareCountToChangeColor(gold, needMaterial.money, 12, textColors[1]);

        CompareCountToChangeColor(materials[0].numText, needMaterial.grayNum, 1, textColors[0]);
        CompareCountToChangeColor(materials[1].numText, needMaterial.timberNum, 4, textColors[0]);
        CompareCountToChangeColor(materials[2].numText, needMaterial.blackNum, 2, textColors[0]);
        CompareCountToChangeColor(materials[3].numText, needMaterial.whiteNum, 3, textColors[0]);
    }
    void CompareCountToChangeColor(TextMeshProUGUI textTarget, int needCount, int itemIndex, in Color enableColor)
    {
        if (needCount > villigeStorage.storageComponent.ItemCounts[itemIndex])
        {
            isBuildable = false;
            textTarget.color = textColors[2];
        }
        else
            textTarget.color = enableColor;
    }

    bool SetActiveMaterialsDesc(int i, int numCount)
    {
        bool onoff = !numCount.Equals(0);

        for (int j = 0; j < materials[i].gameobj.Length; j++)
        {
            materials[i].gameobj[j].SetActive(onoff);
        }
        materials[i].numText.gameObject.SetActive(onoff);

        if (onoff)
            materials[i].numText.text = numCount.ToString();

        return onoff;
    }
    void SetRectTransformSize(bool onoff)
    {
        if (onoff)
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, defaultHeight);
        else
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, defaultHeight - 50);
    }
    public void HighLightNotEnoughMaterials(int materialIndex, MaterialsType type = MaterialsType.BuildingNeed)
    {
        villigeStorage.NotEnoughNodeHighLight(manager.data.needsArray[(int)type][materialIndex]);
    }
}
