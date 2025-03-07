using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnVilligeManager : SpawnManager
{
    public (HeroData, int)[] heroBeforeDatas { get; set; }
    protected override void VirtualStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;
        int length = saveDataInfo.hero.Length;

        if (competeIndexs.Length > 0)
        {
            Array.Sort(competeIndexs);
            length -= competeIndexs.Length;
        }

        heroBeforeDatas = new (HeroData, int)[length];
        int nowCompeteIndex = 0;
        int heroIndex = 0;
        for (int i = 0; i < length; i++)
        {
            while (competeIndexs.Length > 0 && heroIndex == competeIndexs[nowCompeteIndex])
            {
                heroIndex++;
                nowCompeteIndex++;
            }
            heroBeforeDatas[i] = (saveDataInfo.hero[heroIndex], heroIndex);
            heroIndex++;
        }
    }
}
