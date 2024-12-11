using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class StageUnitSpawner : UnitSpawner
{
    [Header("StageOnly")]
    [SerializeField] MonNavi monNavi;

    [SerializeField] Monster[] monsters;

    protected override void VirtualStart()
    {
        foreach (var item in SpawnManager.monsterDatas)
        {
            SpawnMonsterData(item);
        }
    }
    void SpawnMonsterData(MonsterData data)
    {
        Monster newOjb = Instantiate(monsters[(data.unitData.objectData.id - 1) % 100],
                                               data.unitData.objectData.position,
                                               data.unitData.objectData.quaternion,
                                               monNavi.transform);

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
}
