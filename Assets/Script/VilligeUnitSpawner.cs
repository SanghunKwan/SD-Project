using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilligeUnitSpawner : UnitSpawner
{
    [Header("VilligeOnly")]
    [SerializeField] BuildingComponent[] buildings;
    SpawnVilligeManager spawnVilligeManager;

    protected override void VirtualStart()
    {
    }
    protected override void DefaultStart()
    {
        spawnVilligeManager = SpawnManager as SpawnVilligeManager;

        foreach (var building in spawnVilligeManager.buildingDatas)
        {
            SpawnBuildingData(building);
        }

        foreach (var heros in spawnVilligeManager.heroBeforeDatas)
        {
            SpawnHeroData(heros.Item1, heros.Item2);
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
