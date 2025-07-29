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

        VirtualStart();
    }
    protected abstract void VirtualStart();
    protected abstract void DefaultStart();

    protected void NewSpawnedObjectSet(CObject newObject, ObjectData data)
    {
        newObject.OnUICompleteAction += (ref unit_status stat) =>
        {
            newObject.GetStatusEffect(data.dots, data.dotsDirection);
            stat.Clone(data.cur_status);
            newObject.Selected(data.selected);

            if (data.isDead)
                newObject.DelayAfterResigter();
        };

    }
    protected void NewSpawnedUnitSet(CUnit newObject, UnitData data)
    {
        newObject.SetDetected(data.detected);
        newObject.unitMove.destination = data.destination;
        if (data.attackMove)
            newObject.unitMove.AttackPosition(data.destination);
        else
            newObject.unitMove.Navi_Destination(data.destination);

        newObject.unitMove.ChangeHold(data.ishold);
        newObject.unitMove.LoadDepart(data.depart);
    }
    void NewSpawnedHeroSet(Hero newObject, HeroData data)
    {
        newObject.LoadTeamString(data.keycode);
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

        if (data.needGetName)
            newObject.isDefaultName = true;
    }

    public Hero SpawnHeroData(HeroData data, int heroIndex)
    {
        Hero newHero = Instantiate(heroes[(data.unitData.objectData.cur_status.ID - 1) % 100],
                                           data.unitData.objectData.position, data.unitData.objectData.quaternion,
                                           PlayerNavi.nav.transform);
        NewSpawnedObjectSet(newHero, data.unitData.objectData);
        NewSpawnedUnitSet(newHero, data.unitData);
        NewSpawnedHeroSet(newHero, data);


        return newHero;
    }
}
