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
    public BuildingData[] buildingDatas { get; private set; }
    public DropItemData[] dropItemDatas { get; private set; }



    public int[] competeIndex { get; private set; }

    private void Start()
    {
        GameManager.manager.onBattleClearManagerRegistered += DelayStart;
    }
    void DelayStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        monsterDatas = saveDataInfo.stageData.monsterData;
        objectDatas = saveDataInfo.stageData.objectDatas;
        buildingDatas = saveDataInfo.building;
        dropItemDatas = saveDataInfo.stageData.dropItemDatas;


        competeIndex = saveDataInfo.stageData.heros;
        heroDatas = new HeroData[competeIndex.Length];

        for (int i = 0; i < competeIndex.Length; i++)
        {
            heroDatas[i] = saveDataInfo.hero[competeIndex[i]];
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
