using SaveData;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class StageUnitSpawner : UnitSpawner
{
    [Header("StageOnly")]
    [SerializeField] MonNavi monNavi;
    [SerializeField] Transform stagePoolTransform;
    [SerializeField] Monster[] monsters;
    [SerializeField] CObject[] cobjects;

    Transform[] prefabsTransform;
    public enum StagePoolPrefabs
    {
        Object,
        Monster,
        Item,
        Max
    }
    protected override void DefaultStart()
    {
        int length = (int)StagePoolPrefabs.Max;
        prefabsTransform = new Transform[length];

        for (int i = 0; i < length; i++)
        {
            prefabsTransform[i] = stagePoolTransform.GetChild(i);
        }
    }
    protected override void VirtualStart()
    {
        int length = SpawnManager.floorUnitDatas.Length;
        int prefabLength = prefabsTransform.Length;
        FloorUnitData item;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < prefabLength; j++)
            {
                GameObject folder = new GameObject(((StagePoolPrefabs)i).ToString());
                folder.transform.SetParent(prefabsTransform[j]);
            }
        }

        for (int i = 0; i < length; i++)
        {
            item = SpawnManager.floorUnitDatas[i];
            foreach (var monster in item.monsterData)
            {
                SpawnMonsterData(monster, i);
            }

            foreach (var drop in item.dropItemDatas)
            {
                SpawnItemData(drop, i);
            }

            foreach (var obj in item.objectDatas)
            {
                SpawnObjectData(obj, i);
            }
        }
        SpawnDroppingData();
    }
    void SpawnMonsterData(MonsterData data, int folderIndex)
    {
        Monster newOjb = Instantiate(monsters[data.unitData.objectData.cur_status.ID % 100],
                                               data.unitData.objectData.position,
                                               data.unitData.objectData.quaternion,
                                               prefabsTransform[(int)StagePoolPrefabs.Monster].GetChild(folderIndex));

        NewSpawnedObjectSet(newOjb, data.unitData.objectData);
        NewSpawnedUnitSet(newOjb, data.unitData);
        NewSpawnedMonsterSet(newOjb, data);
    }
    void NewSpawnedMonsterSet(Monster newObj, MonsterData data)
    {
        MonsterMove move = newObj.unitMove as MonsterMove;
        move.originTransform = data.originTransform;
        move.patrolDestination = data.patrolDestination;
        move.waitType = data.waitingTypeNum;
        move.standType = data.standType;
    }

    void SpawnItemData(DropItemData data, int folderIndex)
    {
        GameObject item = DropManager.instance.pool.CallItem(data.index - 1, data.stageIndex);
        item.transform.SetParent(prefabsTransform[(int)StagePoolPrefabs.Item].GetChild(folderIndex));

        item.transform.position = data.position;
        item.SetActive(true);
    }
    void SpawnObjectData(ObjectData data, int folderIndex)
    {
        CObject newOjb = Instantiate(cobjects[(data.cur_status.ID - 1) % 100], data.position, data.quaternion,
                                prefabsTransform[(int)StagePoolPrefabs.Object].GetChild(folderIndex));
        NewSpawnedObjectSet(newOjb, data);
        newOjb.gameObject.SetActive(true);
    }
    void SpawnDroppingData()
    {
        ObjectManager manager = GameManager.manager.objectManager;
        foreach (var item in SpawnManager.droppingItems)
        {
            manager.AddYetDroppedItem(item);
            //아이템 드랍 이벤트 콜.
            DropManager.instance.pool.CallItems(item);
        }
    }

}
