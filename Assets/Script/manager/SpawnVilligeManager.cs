using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVilligeManager : SpawnManager
{
    public (HeroData, int)[] heroBeforeDatas { get; set; }
    public override bool isEnter
    {
        get
        {
            if (!base.isEnter)
                towerCompoent.ReadytoUse();

            return base.isEnter;
        }
        protected set => base.isEnter = value;
    }
    [SerializeField] TowerComponent towerCompoent;

    protected override void VirtualStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        isEnter = saveDataInfo.playInfo.isEnter;

        int length = saveDataInfo.hero.Length;

        if (competeIndexs.Length > 0)
        {
            Array.Sort(competeIndexs);
            length -= competeIndexs.Length;
        }

        heroBeforeDatas = new (HeroData, int)[length];
        int nowCompeteIndex = 0;
        int heroIndex = 0;
        int teamArrayCorrection = 0;
        string lastKeycode = saveDataInfo.hero[0].keycode;
        HeroData tempHeroData;
        for (int i = 0; i < length; i++)
        {
            if (competeIndexs.Length > 0)
                while (heroIndex == competeIndexs[nowCompeteIndex])
                {
                    nowCompeteIndex++;
                    HeroIndexAdd();
                }

            heroBeforeDatas[i] = (saveDataInfo.hero[heroIndex], heroIndex - teamArrayCorrection);
            HeroIndexAdd();
        }

        void HeroIndexAdd()
        {
            heroIndex++;

            if (heroIndex >= length)
                return;

            tempHeroData = saveDataInfo.hero[heroIndex];
            if (!tempHeroData.keycode.Equals(lastKeycode))
            {
                teamArrayCorrection = heroIndex;
                lastKeycode = tempHeroData.keycode;
                Debug.Log(heroIndex + "ÆÀ º¯°æ");
            }
        }
    }
    public override void SetEnter(bool newIsEnter)
    {
        isEnter = newIsEnter;
        GameManager.manager.battleClearManager.SaveDataInfo.playInfo.isEnter = isEnter;
    }
}
