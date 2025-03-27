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
        int teamArrayCorrection = 0;
        string lastKeycode = "";
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
            }
        }
    }

}
