using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public FloorUnitData[] floorUnitDatas { get; private set; }
    public HeroData[] heroDatas { get; protected set; }
    public BuildingData[] buildingDatas { get; private set; }
    public List<YetDroppedItem> droppingItems { get; private set; }

    public int[] competeIndexs { get; protected set; }
    public int nowFloorIndex { get; private set; }
    public virtual bool isEnter { get; protected set; }


    private void Start()
    {
        WaitUntilManagerRegistered(DelayStart);
    }
    void WaitUntilManagerRegistered(in System.Action action)
    {
        if (GameManager.manager.battleClearManager != null)
            action();
        else
            GameManager.manager.onBattleClearManagerRegistered += action;
    }
    void DelayStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        floorUnitDatas = saveDataInfo.stageData.floorUnitDatas;
        buildingDatas = saveDataInfo.building;

        nowFloorIndex = saveDataInfo.stageData.nowFloorIndex;
        competeIndexs = saveDataInfo.stageData.heros;
        droppingItems = saveDataInfo.stageData.yetDroppedItems;

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
        isEnter = GameManager.manager.battleClearManager.SaveDataInfo.stageData.isEnter;
    }
    public virtual void SetEnter(bool newIsEnter)
    {
        isEnter = newIsEnter;
        GameManager.manager.battleClearManager.SaveDataInfo.stageData.isEnter = isEnter;
    }
}
