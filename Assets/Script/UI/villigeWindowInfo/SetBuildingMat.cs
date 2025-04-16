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
        SkillUpgradeNeed,
        Max
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

    HashSet<int>[] questEventProceed;
    public bool isBuildable { get; private set; }

    private void Awake()
    {
        title = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        gold = title.transform.Find("Gold&Time").Find("GoldText").GetComponent<TextMeshProUGUI>();
        time = gold.transform.parent.Find("TimeText").GetComponent<TextMeshProUGUI>();
        description = title.transform.Find("desc").GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        int length = (int)MaterialsType.Max;
        questEventProceed = new HashSet<int>[length];
        for (int i = 0; i < length; i++)
        {
            questEventProceed[i] = new HashSet<int>();
        }

        defaultHeight = rectTransform.sizeDelta.y;
    }
    public MaterialsData.NeedMaterials GetData(int materialIndex, MaterialsType materialsType)
    {
        MaterialsData.NeedMaterials needMaterials = manager.data.needsArray[(int)materialsType][materialIndex];
        GetData(needMaterials);
        CheckTutorial(materialIndex, materialsType);
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
        title.text = needMaterial.name;

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
    #region QuestEvent
    void CheckTutorial(int materialIndex, MaterialsType materialsType)
    {
        //튜토리얼 시 빌딩 재료 무료.
        //설명에 튜토리얼 중에 무료라고 표기.
        if (!GetIsContainsQuestEventProceed(materialsType, materialIndex))
            return;

        description.text += "\n<color=#33CA3E>튜토리얼 진행을 위해 한시적 무료.</color>";
        if (gold.text != "0")
        {
            gold.text = "0";
            gold.color = textColors[3];
        }

        foreach (var item in materials)
        {
            item.numText.text = "0";
            item.numText.color = textColors[3];
        }
        isBuildable = true;
    }
    bool GetIsContainsQuestEventProceed(MaterialsType materialsType, int materialIndex)
    {
        return questEventProceed[(int)materialsType].Contains(materialIndex);
    }
    public void AddQuest(MaterialsType materialsType, int materialIndex)
    {
        questEventProceed[(int)materialsType].Add(materialIndex);
    }
    public void RemoveQuest(MaterialsType materialsType, int materialIndex)
    {
        questEventProceed[(int)materialsType].Remove(materialIndex);
    }
    #endregion

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
