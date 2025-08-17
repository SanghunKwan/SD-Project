using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class StagePoolManager : MonoBehaviour
{
    public int[] stageFloors { get; private set; }
    public int startFloorIndex { get; private set; }
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
        GameManager.manager.battleClearManager.onStageSave = SaveStageObjects;
        GameManager.manager.battleClearManager.HasStageAliveMonsters = HasAliveMonsters;
    }
    void SpawnFloorByIndex(int floorIndex, bool isOn)
    {
        int length = transform.childCount;
        for (int i = 0; i < length; i++)
        {
            transform.GetChild(i).GetChild(floorIndex).gameObject.SetActive(isOn);
        }
    }

    void SaveStageObjects(FloorUnitData data, int index)
    {
        ObjectManager manager = GameManager.manager.objectManager;
        Transform tr = transform;
        Transform folder;
        int length;

        //objects
        folder = tr.GetChild(0).GetChild(index);

        length = folder.childCount;
        data.objectDatas = new ObjectData[length];

        for (int i = 0; i < length; i++)
        {
            GameObject go = folder.GetChild(i).gameObject;
            if (manager.ObjectDictionary[2].ContainsKey(go))
                data.objectDatas[i] = new ObjectData(manager.ObjectDictionary[2][go].Value);
        }

        //monster&corpse
        folder = tr.GetChild(1).GetChild(index);
        length = folder.childCount;
        data.monsterData = new MonsterData[length];
        for (int i = 0; i < length; i++)
        {
            GameObject go = folder.GetChild(i).gameObject;
            if (go.layer == 16)
            {
                //시체
                data.monsterData[i] = new MonsterData(manager.CorpseDictionary[go]);
            }
            else
            {
                //살아있는 몬스터
                data.monsterData[i] = new MonsterData((Monster)manager.GetNode(go).Value);
            }
            //folder 자식들 모두 monster 데이터로 저장.
        }

        //items
        folder = tr.GetChild(2).GetChild(index);
        length = folder.childCount;
        data.dropItemDatas = new List<DropItemData>(length);
        for (int i = 0; i < length; i++)
        {
            GameObject go = folder.GetChild(i).gameObject;
            if (manager.NoneObjectDictionary[0].ContainsKey(go))
                data.dropItemDatas.Add(new DropItemData((ItemComponent)(manager.NoneObjectDictionary[0][go].Value)));
            //data.dropItemDatas[i] = new DropItemData((ItemComponent)(manager.NoneObjectDictionary[0][go].Value));
        }
    }

    bool HasAliveMonsters(int floorIndex)
    {
        Transform monsterParentTransform = transform.GetChild(1).GetChild(floorIndex);
        int length = monsterParentTransform.childCount;

        for (int i = 0; i < length; i++)
        {
            if (monsterParentTransform.GetChild(i).gameObject.layer != 16)
                return true;
        }
        return false;
    }
}
