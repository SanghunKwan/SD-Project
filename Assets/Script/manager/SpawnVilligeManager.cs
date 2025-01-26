using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVilligeManager : SpawnManager
{
    public (HeroData, int)[] heroBeforeDatas { get; set; }
    protected override void VirtualStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        Array.Sort(competeIndexs);

        int length = saveDataInfo.hero.Length - competeIndexs.Length;

        heroBeforeDatas = new (HeroData, int)[length];
        int nowCompeteIndex = 0;
        int heroIndex = 0;
        for (int i = 0; i < length; i++)
        {
            while (heroIndex == competeIndexs[nowCompeteIndex])
            {
                heroIndex++;
                nowCompeteIndex++;
            }
            heroBeforeDatas[i] = (saveDataInfo.hero[heroIndex], heroIndex);
            heroIndex++;
        }
    }
}
