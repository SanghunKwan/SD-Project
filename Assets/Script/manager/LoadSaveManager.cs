using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveManager : JsonSaveLoad
{
    [Serializable]
    public class SaveDataInfo
    {
        //ž �ִ����� ���õ� ������
        public FloorManager.FloorData floorData;
        public int day;
        //���� ��¥
        public int nextScene;
        //���� �������ִ� ����(���� 1, ž 2)
        public int floor;
        //���� �������ִ� ����(�������� 0)
        public Unit.Hero[] hero;
        public Unit.CObject[] building;
        public Unit.CUnit[] monsters;
        public float spaceNum;
        public InventoryManager.itemInfo inventory;


        /*
         �� �� �߰��� ����
        7. ���� �� ������ �ִ� ���.
         
         
         */



        public SaveDataInfo()
        {
            floor = 0;
            day = 0;
            nextScene = 1;
            floorData = new FloorManager.FloorData();
        }
    }
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
        Debug.Log(info.floor);
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
