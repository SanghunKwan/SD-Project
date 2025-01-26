using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;
using System.Threading.Tasks;

public class SettlementCharacterParent : SettleCanSkip
{
    SettlementCharacter[] settlementCharacters;
    [SerializeField] float chanceToNewQuirk;
    [SerializeField] float chanceToNewDisease;

    public void Init()
    {
        int length = transform.childCount;
        SaveDataInfo info = GameManager.manager.battleClearManager.SaveDataInfo;
        settlementCharacters = new SettlementCharacter[length];

        for (int i = 0; i < length; i++)
        {
            settlementCharacters[i] = transform.GetChild(i).GetComponent<SettlementCharacter>();
        }

        SettleCharacters(info);

        GameManager.manager.battleClearManager.OverrideSaveDataSettle();

    }
    async void SettleCharacters(SaveDataInfo saveInfo)
    {
        int[] herosIndex = saveInfo.stageData.heros;
        int heroCount = herosIndex.Length;

        HeroData[] heros = saveInfo.hero;
        HeroData heroData;

        for (int i = 0; i < heroCount; i++)
        {
            heroData = heros[herosIndex[i]];
            settlementCharacters[i].gameObject.SetActive(true);
            settlementCharacters[i].SetCharcterNameTag(heroData, i);

            if (ChanceNewQuirk(chanceToNewQuirk))
                settlementCharacters[i].CreateNewQuirk(heroData.quirks);
            else
                Debug.Log("새 기벽 생성하지 않는 확률");

            if (ChanceNewQuirk(chanceToNewDisease))
                settlementCharacters[i].CreateNewDisease(heroData.disease);
            else
                Debug.Log("새 질병 생성하지 않는 확률");

            await Task.Delay(interval);
        }
    }


    bool ChanceNewQuirk(float chance)
    {
        return chance > Random.Range(0, 100.0f);
    }
}
