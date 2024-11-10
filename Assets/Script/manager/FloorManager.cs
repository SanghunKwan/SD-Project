using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : JsonLoad
{
    int saveIndex;
    [Serializable]
    public class FloorData
    {
        public int topFloor;
        public int[] floorLooks;
        public float[] floorAngles;

        public FloorData()
        {
            topFloor = 1;

            //floorLooks = new int[9] { 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            floorLooks = new int[1] { 0 };
            //floorAngles = new float[9] { 27, 47, 180, -46, 27, 60, 90, -127, 50 };
            floorAngles = new float[1] { 27 };
        }
        public void SetData(int floorLevel, int[] nowTowerLooks, float[] nowAngles)
        {
            topFloor = floorLevel;
            floorLooks = nowTowerLooks;
            floorAngles = nowAngles;

        }
    }
    FloorData floorData;

    private void Awake()
    {
        floorData = LoadData<FloorData>("FloorSaveData");
    }

    public bool GetData(out FloorData data)
    {
        data = floorData;
        return true;
    }
    public void SetData(int index, in int[] nowTowerLooks, in float[] nowAngles)
    {
        //floorDatas.data[index].SetData(1, nowTowerLooks, nowAngles);
        //SaveData();
    }
    public void SaveData()
    {
        Debug.Log("수정 필요 - 이후 save파일 생성에 통합 예정");
        //SaveData(floorDatas, "FloorSaveData");
    }


    [ContextMenu("json만들기")]
    public void SDF()
    {
        SDF<FloorData>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
