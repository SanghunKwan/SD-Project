using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePoolManager : MonoBehaviour
{
    public int[] stageFloors { get; private set; }
    public int nowFloorIndex { get; private set; }
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
        nowFloorIndex = saveDataInfo.stageData.nowFloorIndex;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

    }

}
