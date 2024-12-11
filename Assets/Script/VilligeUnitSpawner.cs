using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilligeUnitSpawner : UnitSpawner
{
    [Header("StageOnly")]
    [SerializeField] BuildingComponent[] buildings;

    protected override void VirtualStart()
    {
        foreach (var building in SpawnManager.buildingDatas)
        {
            SpawnBuildingData(building);
        }
    }
    void SpawnBuildingData(BuildingData data)
    {
        BuildingComponent newBuilding
            = Instantiate(buildings[(data.objectData.id - 1) % 100], data.objectData.position, data.objectData.quaternion);
        NewSpawnedObjectSet(newBuilding.CObject, data.objectData);

        int length = data.workHero.Length;
        for (int i = 0; i < length; i++)
        {
            BuildingWorkHeroLoad(newBuilding, data.workHero[i], i);
        }
    }
    void BuildingWorkHeroLoad(BuildingComponent newBuilding, int workHeroIndex, int buildingWorkPlaceIndex)
    {
        //newBuilding.SaveData(workHeroIndex 로 히어로를 가져올 것., buildingWorkPlaceIndex);
    }
}
