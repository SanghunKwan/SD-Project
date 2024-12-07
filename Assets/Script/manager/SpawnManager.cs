using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] int startDelayMiliSec;
    public HeroData[] heroDatas { get; private set; }
    public MonsterData[] monsterDatas { get; private set; }
    public ObjectData[] objectDatas { get; private set; }
    public DropItemData[] dropItemDatas { get; private set; }
    int[] competeIndex;

    private void Start()
    {
        GameManager.manager.onBattleClearManagerRegistered += DelayStart;
        WaitforSeconds(1500, SpawnHeroRepeat);
    }
    async void WaitforSeconds(int miliseconds, Action action)
    {
        await Task.Delay(miliseconds);
        action();
    }
    void DelayStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        competeIndex = saveDataInfo.stageData.heros;
        heroDatas = new HeroData[competeIndex.Length];

        for (int i = 0; i < competeIndex.Length; i++)
        {
            heroDatas[i] = saveDataInfo.hero[competeIndex[i]];
        }
        monsterDatas = saveDataInfo.stageData.monsterData;
        objectDatas = saveDataInfo.stageData.objectDatas;
        dropItemDatas = saveDataInfo.stageData.dropItemDatas;

    }
    void SpawnHeroRepeat()
    {
        int waitTime = 0;
        int length = competeIndex.Length;

        for (int i = 0; i < length; i++)
        {
            WaitforSeconds(waitTime, SpawnHero);
            waitTime += 900;
        }
    }
    void SpawnHero()
    {

    }

    void SpawnCobject(ObjectData data)
    {
    }
}
