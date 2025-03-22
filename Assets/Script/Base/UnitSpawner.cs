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
    public Transform objectTransform;


    private void Start()
    {
        DefaultStart();
        if (SpawnManager.isEnter)
            return;

        int heroIndex = 0;
        foreach (var item in SpawnManager.heroDatas)
        {
            SpawnHeroData(item, heroIndex);
            heroIndex++;
        }

        foreach (var item in SpawnManager.objectDatas)
        {
            SpawnObjectData(item);
        }

        VirtualStart();
    }
    protected abstract void VirtualStart();
    protected abstract void DefaultStart();
    void SpawnObjectData(ObjectData data)
    {
        CObject newOjb = Instantiate(cobjects[(data.cur_status.ID - 1) % 100], data.position, data.quaternion, objectTransform);
        NewSpawnedObjectSet(newOjb, data);
        newOjb.gameObject.SetActive(true);
    }
    protected void NewSpawnedObjectSet(CObject newObject, ObjectData data)
    {
        newObject.GetStatusEffect(data.dots, data.dotsDirection);
        newObject.OnUICompleteAction += (ref unit_status stat) =>
        {
            stat.Clone(data.cur_status);
            newObject.Selected(data.selected);
            if (data.isDead)
                newObject.DelayAfterResigter();
        };

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
        newObject.SetData(newObject.FieldEquipsNum, data.fieldEquipNum);

        if ((ActionAlert.ActionType)data.villigeAction == ActionAlert.ActionType.buildingWork)
            newObject.alloBuilding((AddressableManager.BuildingImage)data.workBuilding);
        else
            newObject.alloBuilding((ActionAlert.ActionType)data.villigeAction);
    }

    public Hero SpawnHeroData(HeroData data, int heroIndex)
    {
        Hero newHero = Instantiate(heroes[(data.unitData.objectData.cur_status.ID - 1) % 100],
                                           data.unitData.objectData.position, data.unitData.objectData.quaternion,
                                           PlayerNavi.nav.transform);
        NewSpawnedObjectSet(newHero, data.unitData.objectData);
        NewSpawnedUnitSet(newHero, data.unitData);
        NewSpawnedHeroSet(newHero, data);
        newHero.heroInStageIndex = heroIndex;


        return newHero;
    }
}
