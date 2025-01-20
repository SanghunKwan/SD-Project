using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementCharacter : MonoBehaviour
{
    [SerializeField] SettleNamePlate settleNamePlate;
    [SerializeField] GameObject isHeroDeadObject;
    [SerializeField] GameObject isHeroCopseGetBackObject;
    [SerializeField] TextMeshProUGUI quirkText;
    [SerializeField] TextMeshProUGUI diseaseText;
    [SerializeField] RectTransform rebuildRectTransform;

    public void SetCharcterNameTag(HeroData heroData, int heroInStageIndex)
    {
        InventoryStorage storage = GameManager.manager.storageManager.inventoryComponents(InventoryComponent.InventoryType.Stage).inventoryStorage;

        settleNamePlate.SetTexts(heroData.keycode, heroData.lv, heroData.name);
        gameObject.SetActive(true);

        if (heroData.unitData.objectData.isDead)
        {
            isHeroDeadObject.SetActive(true);
            isHeroCopseGetBackObject.SetActive(!storage.IsCorpseExist(heroInStageIndex));
        }
        
    }
    public void CreateNewQuirk(QuirkSaveData quirkSaveData)
    {
        CreateNewQuirks(quirkSaveData, QuirkData.manager.quirkInfo, 5, quirkText, "기벽 최대 개수 도달");
    }



    public void CreateNewDisease(QuirkDefaultData diseaseData)
    {
        CreateNewQuirks(diseaseData, QuirkData.manager.diseaseInfo, 4, diseaseText, "질병 최대 개수 도달");
    }
    void CreateNewQuirks(QuirkDefaultData quirkBeforeData, in QuirkData.QuirkS quirksInfo, int maxLength, TextMeshProUGUI textComponent, in string DebugLog)
    {
        QuirkData.Quirk[] quirks = quirksInfo.quirks;

        int quirkCount = GetQuirksData(maxLength, quirkBeforeData);

        if (quirkCount <= maxLength)
        {
            textComponent.text = quirks[GetRandomUnusedQuirkIndex(quirkBeforeData.quirks, quirks.Length, quirkCount)].name;
        }
        else
            Debug.Log(DebugLog);

    }
    int GetQuirksData(int maxLength, QuirkDefaultData heroQuirkData)
    {
        int quirkCount = 0;

        for (int i = 0; i < maxLength; i++)
        {
            if (heroQuirkData.quirks[i] == 0)
                continue;

            quirkCount++;

        }
        return quirkCount;
    }
    int GetRandomUnusedQuirkIndex(in int[] beforeQuirks, int quirkInfoLength, int beforeQuirkCount)
    {
        int[] sortedQuirks = new int[beforeQuirkCount];

        Array.Copy(beforeQuirks, sortedQuirks, beforeQuirkCount);
        Array.Sort(sortedQuirks);

        int length = quirkInfoLength - beforeQuirkCount;

        return GetNoneUnusedQuirkIndexArray(sortedQuirks, length)[UnityEngine.Random.Range(1, length)];
    }
    int[] GetNoneUnusedQuirkIndexArray(in int[] sortedArray, int length)
    {
        int sortedArrayIndex = 0;
        int nowComponent = 0;

        int sortedArrayLength = sortedArray.Length;
        int[] indexArray = new int[length];

        for (int i = 0; i < length; i++)
        {

            //sortedArray의 요소와 중첩되지 않을 것.

            while (sortedArrayIndex < sortedArrayLength && sortedArray[sortedArrayIndex] == nowComponent)
            {
                sortedArrayIndex++;
                nowComponent++;
            }

            indexArray[i] = nowComponent++;
        }

        return indexArray;
    }
}
