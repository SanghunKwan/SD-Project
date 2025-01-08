using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;

public class SettlementCharacterParent : MonoBehaviour
{
    SettlementCharacter[] settlementCharacters;


    private void Awake()
    {
        int length = transform.childCount;
        settlementCharacters = new SettlementCharacter[length];

        for (int i = 0; i < length; i++)
        {
            settlementCharacters[i] = transform.GetChild(i).GetComponent<SettlementCharacter>();
        }
    }

    private void Start()
    {
        SaveDataInfo saveInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        HeroData[] heros = saveInfo.hero;
        HeroData heroData;
        int[] herosIndex = saveInfo.stageData.heros;
        int heroCount = herosIndex.Length;

        for (int i = 0; i < heroCount; i++)
        {
            heroData = heros[herosIndex[i]];
            settlementCharacters[i].SetCharcterNameTag(heroData);
        }

    }


}
