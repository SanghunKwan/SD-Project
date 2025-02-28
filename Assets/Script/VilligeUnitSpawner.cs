using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilligeUnitSpawner : UnitSpawner
{
    [Header("VilligeOnly")]
    [SerializeField] BuildingComponent[] buildings;
    SpawnVilligeManager spawnVilligeManager;
    [SerializeField] CharacterList characterList;

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
