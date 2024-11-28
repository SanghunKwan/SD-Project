using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;

namespace SaveData
{
    [Serializable]
    public class SaveDataInfo
    {
        public int day;
        //���� ��¥
        public int nextScene;
        //���� �������ִ� ����(���� 1, ž 2)


        public Unit.Hero[] hero;
        public Unit.CObject[] building;
        public InventoryManager.itemInfo inventory;
        //ž �ִ����� ���õ� ������
        public FloorManager.FloorData floorData;

        public SaveDataInfo()
        {
            day = 0;
            nextScene = 2;
            floorData = new FloorManager.FloorData();
        }
    }
    public class HeroData
    {
        UnitData unitData;
        string name;

    }
    public class UnitData
    {
        ObjectData objectData;
        Unit.TypeNum typeNum;
        Unit.Species species;

    }
    public class ObjectData
    {
        Vector3 position;
        Quaternion quaternion;
        bool selected;
        Unit.unit_status cur_status;
    }
    public class DropItemData
    {
        Vector3 position;
        int index;
    }
}

public class LoadSaveManager : JsonSaveLoad
{

    #region ������ üũ
    public bool IsValidData(int index)
    {
        return SaveDataExist(index);
    }
    public void DataNowFloor(int index, out int floor, out int day)
    {
        SaveDataFloor(index, out floor, out day);
    }
    #endregion
    #region ������ ����
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
    [ContextMenu("json���� ����")]
    public void SDF()
    {
        SDF<SaveDataInfo>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
