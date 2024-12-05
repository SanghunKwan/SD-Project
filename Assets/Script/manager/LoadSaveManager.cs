using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;
using Unit;

namespace SaveData
{
    [Serializable]
    public class SaveDataInfo
    {
        public int day;
        public int nextScene;
        public FloorManager.FloorData floorData;

        public HeroData[] hero;
        public BuildingData[] building;

        public int[] items;

        public StageData stageData;


        public SaveDataInfo()
        {
            day = 0;
            nextScene = 1;
            hero = new HeroData[1] { new HeroData() };
            floorData = new FloorManager.FloorData();
            stageData = new StageData();
        }
    }
    [Serializable]
    public class HeroData
    {
        public string name;
        public int lv;
        public int[] quirks;
        public int[] disease;
        public string keycode;
        public int[] equipNum;
        public int[] skillNum;
        public int workBuilding;
        public bool isDead;
        public UnitData unitData;

        public HeroData()
        {
            name = "디스마스";
            lv = 1;
            quirks = new int[5] { 6, 2, 0, 0, 0 };
            disease = new int[4] { 0, 0, 0, 0 };
            keycode = "=";
            equipNum = new int[3] { 1, 1, 1 };
            skillNum = new int[4] { 1, 1, 1, 1 };
            workBuilding = 0;
            isDead = false;
            unitData = new UnitData();
        }
    }
    public class BuildingData
    {
        public int[] workHero = new int[3];
        public ObjectData objectData;
    }
    [Serializable]
    public class StageData
    {
        public int[] heros;
        public int[] floors = new int[5];
        public int nowFloorIndex;
        public bool isAllClear;

        public InventoryStorage.Slot[] slots;

        public UnitData[] unitData;
        public ObjectData[] objectDatas;
        public DropItemData[] dropItemDatas;
        public StageData()
        {
            heros = new int[1] { 0 };
            floors = new int[5] { 0,1,2,3,4 };
            nowFloorIndex = 0;
            isAllClear = false;
        }
    }
    [Serializable]
    public class UnitData
    {
        public bool detected;
        public ObjectData objectData;
        public UnitData()
        {
            detected = true;
            objectData = new ObjectData();
        }
        public UnitData(CUnit unit)
        {
            detected = unit.detected;
            objectData = new ObjectData(unit);
        }
    }
    [Serializable]
    public class ObjectData
    {
        public Vector3 position;
        public Quaternion quaternion;
        public bool selected;
        public unit_status cur_status;
        public int id;
        public int[] dots;
        public ObjectData()
        {
            position = Vector3.zero;
            quaternion = Quaternion.Euler(0, 0, 0);
            selected = false;
            id = 6;
            cur_status = new unit_status();
            dots = new int[(int)SkillData.EFFECTINDEX.MAX] { 0, 0, 0, 0 };
        }
        public ObjectData(CObject cObject)
        {
            position = cObject.transform.position;
            quaternion = cObject.transform.rotation;
            selected = cObject.selected;
            cur_status = cObject.curstat;
            id = cObject.id;
            dots = cObject.dots;
        }
    }
    [Serializable]
    public class DropItemData
    {
        public Vector3 position;
        public int index;
        public DropItemData(ItemComponent itemComponent)
        {
            position = itemComponent.transform.position;
            index = itemComponent.Index;
        }
    }
}

public class LoadSaveManager : JsonSaveLoad
{

    #region 데이터 체크
    public bool IsValidData(int index)
    {
        return SaveDataExist(index);
    }
    public void DataNowFloor(int index, out int floor, out int day)
    {
        SaveDataFloor(index, out floor, out day);
    }
    #endregion
    #region 데이터 관리
    public void LoadData(int index, out SaveDataInfo info)
    {
        info = LoadSave<SaveDataInfo>(index);
    }
    public void CreateSaveFile(int index)
    {
        SaveData(new SaveDataInfo(), "save" + (index + 1).ToString());
    }
    public void DeleteSaveFile(int index)
    {
        DeleteSave("save" + (index + 1).ToString());
    }
    #endregion
    [ContextMenu("json파일 생성")]
    public void SDF()
    {
        SDF<SaveDataInfo>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
