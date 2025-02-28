using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public HeroData[] heroDatas { get; protected set; }
    public MonsterData[] monsterDatas { get; private set; }
    public ObjectData[] objectDatas { get; private set; }
    public BuildingData[] buildingDatas { get; private set; }
    public DropItemData[] dropItemDatas { get; private set; }

    public int[] competeIndexs { get; protected set; }
    public int nowFloorIndex { get; private set; }
    public bool isEnter { get; private set; }

    private void Start()
    {
        GameManager.manager.onBattleClearManagerRegistered += DelayStart;
    }
    void DelayStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        isEnter = saveDataInfo.stageData.isEnter;

        monsterDatas = saveDataInfo.stageData.monsterData;
        objectDatas = saveDataInfo.stageData.objectDatas;
        buildingDatas = saveDataInfo.building;
        dropItemDatas = saveDataInfo.stageData.dropItemDatas;

        nowFloorIndex = saveDataInfo.stageData.nowFloorIndex;
        competeIndexs = saveDataInfo.stageData.heros;

        heroDatas = new HeroData[competeIndexs.Length];
        for (int i = 0; i < competeIndexs.Length; i++)
        {
            heroDatas[i] = saveDataInfo.hero[competeIndexs[i]];
        }

        VirtualStart();

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    protected virtual void VirtualStart()
    {

    }
}
