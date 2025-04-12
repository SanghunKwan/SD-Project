using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePoolManager : MonoBehaviour
{
    public int[] stageFloors { get; private set; }
    public int startFloorIndex { get; private set; }
    public int floorIndexAdd { get; private set; }
    public int nowFloorIndex { get { return startFloorIndex + floorIndexAdd; } }
    public bool isLoaded { get; private set; }


    private void Start()
    {
        if (GameManager.manager.battleClearManager == null)
            GameManager.manager.onBattleClearManagerRegistered += DelayStart;
        else
            DelayStart();
    }
    void DelayStart()
    {
        SaveDataInfo saveDataInfo = GameManager.manager.battleClearManager.SaveDataInfo;

        stageFloors = saveDataInfo.stageData.floors;
        startFloorIndex = saveDataInfo.stageData.nowFloorIndex;

        isLoaded = saveDataInfo.stageData.floorUnitDatas.Length > 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        GameManager.manager.battleClearManager.SetActiveNewStageObjects = SpawnFloorByIndex;
    }
    void SpawnFloorByIndex(int floorIndex)
    {
        floorIndexAdd = floorIndex - startFloorIndex;

        int length = transform.childCount;
        for (int i = 0; i < length; i++)
        {
            transform.GetChild(i).GetChild(floorIndexAdd).gameObject.SetActive(true);
        }
    }
}
