using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;

public class SettlementCharacterParent : MonoBehaviour
{
    SettlementCharacter[] settlementCharacters;
    [SerializeField] float chanceToNewQuirk;
    [SerializeField] float chanceToNewDisease;

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


            if (ChanceNewQuirk(chanceToNewQuirk))
                settlementCharacters[i].CreateNewQuirk(heroData.quirks);
            else
                Debug.Log("새 기벽 생성하지 않는 확률");

            if (ChanceNewQuirk(chanceToNewDisease))
                settlementCharacters[i].CreateNewDisease(heroData.disease);
            else
                Debug.Log("새 질병 생성하지 않는 확률");


        }



    }


    bool ChanceNewQuirk(float chance)
    {
        return chance > Random.Range(0, 100.0f);
    }


}
