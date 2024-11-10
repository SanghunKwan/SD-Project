using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveManager : JsonSaveLoad
{
    [Serializable]
    public class SaveDataInfo
    {
        //탑 최대층에 관련된 데이터
        public FloorManager.FloorData floorData;
        public int day;
        //현재 날짜
        public int nextScene;
        //현재 입장해있는 공간(마을 1, 탑 2)
        public int floor;
        //현재 입장해있는 층수(마을에선 0)
        public Unit.Hero[] hero;
        public Unit.CObject[] building;
        public Unit.CUnit[] monsters;
        public float spaceNum;
        public InventoryManager.itemInfo inventory;


        /*
         이 외 추가할 내용
        7. 마을 내 가지고 있는 재산.
         
         
         */



        public SaveDataInfo()
        {
            floor = 0;
            day = 0;
            nextScene = 1;
            floorData = new FloorManager.FloorData();
        }
    }
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
