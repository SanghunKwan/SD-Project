using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilligeUnitSpawner : UnitSpawner
{
    [Header("VilligeOnly")]
    SpawnVilligeManager spawnVilligeManager;
    BuildingPool buildingPool;
    [SerializeField] CharacterList characterList;

    protected override void VirtualStart()
    {

    }
    protected override void DefaultStart()
    {
        spawnVilligeManager = SpawnManager as SpawnVilligeManager;
        buildingPool = objectTransform.GetComponent<BuildingPool>();
        int length = spawnVilligeManager.buildingDatas.Length;
        for (int i = 0; i < length; i++)
        {
            SpawnBuildingData(spawnVilligeManager.buildingDatas[i]);
        }


        foreach (var building in spawnVilligeManager.buildingDatas)
        {

        }

        foreach (var allHeros in GameManager.manager.battleClearManager.SaveDataInfo.hero)
        {
            characterList.SpawnVilligeInteract(allHeros.keycode, allHeros);
        }

        Unit.Hero tempHero;
        foreach (var villigeHeros in spawnVilligeManager.heroBeforeDatas)
        {
            tempHero = SpawnHeroData(villigeHeros.Item1, villigeHeros.Item2);
            characterList.MatchingHeroWithInteract(villigeHeros.Item2, tempHero);
        }
        characterList.ReArrage();
    }
    void SpawnBuildingData(BuildingData data)
    {
        BuildingConstructDelay newBuilding = buildingPool
                        .PoolBuilding((AddressableManager.BuildingImage)(data.objectData.id - 201), data.objectData.position);
        NewSpawnedBuildingSet(newBuilding, data);
        NewSpawnedObjectSet(newBuilding.buildingComponent.CObject, data.objectData);

        int length = data.workHero.Length;
        for (int i = 0; i < length; i++)
        {
            BuildingWorkHeroLoad(newBuilding.buildingComponent, data.workHero[i], i);
        }
    }
    void BuildingWorkHeroLoad(BuildingComponent newBuilding, int workHeroIndex, int buildingWorkPlaceIndex)
    {
        //newBuilding.SaveData(workHeroIndex 로 히어로를 가져올 것., buildingWorkPlaceIndex);
    }
    void NewSpawnedBuildingSet(BuildingConstructDelay building, BuildingData buildingData)
    {
        building.LoadConstructionData(buildingData);
    }

}
