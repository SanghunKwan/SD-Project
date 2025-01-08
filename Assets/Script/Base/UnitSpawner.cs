using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public abstract class UnitSpawner : MonoBehaviour
{
    [SerializeField] protected SpawnManager SpawnManager;

    [SerializeField] Hero[] heroes;
    [SerializeField] CObject[] cobjects;

    protected Action<int>[] spawnActions;


    private void Start()
    {
        if (SpawnManager.nowFloorIndex <= 0)
            return;

        foreach (var item in SpawnManager.heroDatas)
        {
            SpawnHeroData(item);
        }

        foreach (var item in SpawnManager.objectDatas)
        {
            SpawnObjectData(item);
        }

        VirtualStart();
    }
    protected abstract void VirtualStart();
    void SpawnObjectData(ObjectData data)
    {
        CObject newOjb = Instantiate(cobjects[(data.id - 1) % 100], data.position, data.quaternion);
        NewSpawnedObjectSet(newOjb, data);
    }
    protected void NewSpawnedObjectSet(CObject newObject, ObjectData data)
    {
        newObject.GetStatusEffect(data.dots, data.dotsDirection);
        newObject.LoadCurStat(data.cur_status.Clone(data.cur_status));
        newObject.Selected(data.selected);
    }
    protected void NewSpawnedUnitSet(CUnit newObject, UnitData data)
    {
        newObject.SetDetected(data.detected);
        newObject.unitMove.Navi_Destination(data.destination);
        newObject.unitMove.attackMove = data.attackMove;
        newObject.unitMove.ChangeHold(data.ishold);
        newObject.unitMove.LoadDepart(data.depart);

    }
    void NewSpawnedHeroSet(Hero newObject, HeroData data)
    {
        newObject.TeamChange(data.keycode);
        newObject.SetLevel(data.lv);
        newObject.SetQuirk(data.quirks.quirks);
        newObject.SetDisease(data.disease.quirks);
        newObject.name = data.name;
        newObject.SetData(newObject.EquipsNum, data.equipNum);
        newObject.SetData(newObject.SkillsNum, data.skillNum);

        if ((ActionAlert.ActionType)data.villigeAction == ActionAlert.ActionType.buildingWork)
            newObject.alloBuilding((AddressableManager.BuildingImage)data.workBuilding);
        else
            newObject.alloBuilding((ActionAlert.ActionType)data.villigeAction);


        Debug.Log("isDead 기능 추가 예정");

    }
    public Hero SpawnHeroData(HeroData data)
    {
        Hero newHero = Instantiate(heroes[(data.unitData.objectData.id - 1) % 100],
                                           data.unitData.objectData.position, data.unitData.objectData.quaternion, PlayerNavi.nav.transform);
        NewSpawnedObjectSet(newHero, data.unitData.objectData);
        NewSpawnedUnitSet(newHero, data.unitData);
        NewSpawnedHeroSet(newHero, data);

        return newHero;
    }
}
