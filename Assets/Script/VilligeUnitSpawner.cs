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
            SpawnFromHeroData(villigeHeros.Item1, villigeHeros.Item2);
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
    void SpawnFromHeroData(HeroData heroData, int heroIndex)
    {
        Unit.Hero tempHero;
        tempHero = SpawnHeroData(heroData, heroIndex);
        characterList.MatchingHeroWithInteract(heroIndex, tempHero);
        Debug.Log(heroIndex);
    }
    public villigeInteract SummonHeroFromHeroData(HeroData heroData)
    {
        //spawnfromherodata 에서 index도 확인 필요.
        characterList.SpawnVilligeInteract(heroData.keycode, heroData);
        villigeInteract newNametag = characterList.trViewPort[characterList.keyToTeamsNum[heroData.keycode]].characters[^1];
        SpawnFromHeroData(heroData, newNametag.transform.GetSiblingIndex() - 1);
        return newNametag;
    }
}
