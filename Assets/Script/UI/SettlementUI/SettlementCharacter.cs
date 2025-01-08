using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettlementCharacter : MonoBehaviour
{
    [SerializeField] SettleNamePlate settleNamePlate;
    [SerializeField] GameObject IsHeroDeadObject;
    [SerializeField] TextMeshProUGUI quirkText;
    [SerializeField] TextMeshProUGUI diseaseText;

    public void SetCharcterNameTag(HeroData heroData)
    {

        settleNamePlate.SetTexts(heroData.keycode, heroData.lv, heroData.name);
        gameObject.SetActive(true);

        IsHeroDeadObject.SetActive(heroData.isDead);

    }
    public void CreateNewQuirk(QuirkSaveData quirkSaveData)
    {
        CreateNewQuirks(quirkSaveData, QuirkData.manager.quirkInfo, 5, quirkText, "기벽 최대 개수 도달");
    }

    int GetRandomUnusedQuirkIndex(in int[] quirks, int quirksLength, int quirkDataLength)
    {
        int arrayLength = quirks.Length;
        int length = quirkDataLength - quirks.Length;

        int[] sortedQuirks = new int[arrayLength];

        int[] noneUsedQuirks;
        Array.Copy(quirks, sortedQuirks, quirksLength);
        Array.Sort(sortedQuirks);

        noneUsedQuirks = GetNoneUnusedQuirkIndexArray(sortedQuirks, length);

        return noneUsedQuirks[UnityEngine.Random.Range(0, length)];
    }
    int[] GetNoneUnusedQuirkIndexArray(int[] sortedArray, int length)
    {
        int nowArrayIndex = 0;
        int nowComponent = 0;

        int sortedArrayLength = sortedArray.Length;
        int[] indexArray = new int[length];

        for (int i = 0; i < length; i++)
        {
            while (nowArrayIndex < sortedArrayLength && sortedArray[nowArrayIndex] == nowComponent)
            {
                nowArrayIndex++;
                nowComponent++;
            }

            indexArray[i] = nowComponent++;
        }

        return indexArray;
    }

    public void CreateNewDisease(QuirkDefaultData diseaseData)
    {
        CreateNewQuirks(diseaseData, QuirkData.manager.diseaseInfo, 4, diseaseText, "질병 최대 개수 도달");
    }
    void CreateNewQuirks(QuirkDefaultData quirkDefaultData, in QuirkData.QuirkS quirksInfo, int maxLength, TextMeshProUGUI textComponent, in string DebugLog)
    {
        QuirkData.Quirk[] quirks = quirksInfo.quirks;

        if (quirkDefaultData.length < maxLength)
        {
            textComponent.text = quirks[GetRandomUnusedQuirkIndex(quirkDefaultData.quirks, quirkDefaultData.length, quirks.Length)].name;
        }
        else
            Debug.Log(DebugLog);

    }
}
