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
        Unit.Hero tempHero;

        int length = spawnVilligeManager.buildingDatas.Length;

        foreach (var allHeros in GameManager.manager.battleClearManager.SaveDataInfo.hero)
        {
            characterList.SpawnVilligeInteract(allHeros.keycode, allHeros);
        }

        for (int i = 0; i < length; i++)
        {
            SpawnBuildingData(spawnVilligeManager.buildingDatas[i]);
        }

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
                        .PoolBuilding((AddressableManager.BuildingImage)(data.objectData.cur_status.ID - 201), data.objectData.position);
        NewSpawnedBuildingSet(newBuilding, data);
        NewSpawnedObjectSet(newBuilding.buildingComponent.CObject, data.objectData);

        int length = data.workHero.Length;
        for (int i = 0; i < length; i++)
        {
            BuildingWorkHeroLoad(newBuilding.buildingComponent, data.workHero[i], i);
        }
    }
    void NewSpawnedBuildingSet(BuildingConstructDelay building, BuildingData buildingData)
    {
        building.LoadConstructionData(buildingData);
    }
    void BuildingWorkHeroLoad(BuildingComponent newBuilding, int workHeroIndex, int buildingWorkPlaceIndex)
    {
        villigeInteract tempInteract = characterList.GetInteractByIndex(workHeroIndex);

        if (tempInteract != null)
            newBuilding.saveVilligeInteract[buildingWorkPlaceIndex] = tempInteract;
        else
            return;

        AddressableManager.manager.DelayUntilLoadingComplete(() =>
        newBuilding.saveVilligeInteract[buildingWorkPlaceIndex].LoadWorkPlace(newBuilding, buildingWorkPlaceIndex));
    }
}
