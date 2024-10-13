using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    int saveIndex;
    [Serializable]
    public class FloorData
    {
        public int nowFloor;
        public int[] floorLooks;
        public float[] floorAngles;

        public FloorData()
        {
            nowFloor = 10;
            floorLooks = new int[9] { 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            floorAngles = new float[9] { 27, 47, 180, -46, 27, 60, 90, -127, 50 };
        }
        public void SetData(int floorLevel, int[] nowTowerLooks, float[] nowAngles)
        {
            nowFloor = floorLevel;
            floorLooks = nowTowerLooks;
            floorAngles = nowAngles;

        }
    }
    class FloorDatas
    {
        public FloorData[] data;
        public FloorDatas()
        {
            data = new FloorData[3] { new FloorData(), new FloorData(), new FloorData() };
        }
    }
    FloorDatas floorDatas;

    private void Awake()
    {
        string wnth = Path.Combine(Application.dataPath, "DataTable/FloorSaveData.json");

        floorDatas = (FloorDatas)JsonUtility.FromJson(File.ReadAllText(wnth), typeof(FloorDatas));

    }

    public bool GetData(out FloorData data)
    {
        data = floorDatas.data[saveIndex];

        return floorDatas.data.Length > saveIndex;
    }
    public void SetData(int index, in int[] nowTowerLooks, in float[] nowAngles)
    {
        floorDatas.data[index].SetData(1, nowTowerLooks, nowAngles);
        SaveData();
    }
    public void SaveData()
    {
        string wnth = Path.Combine(Application.dataPath, "DataTable/FloorSaveData.json");
        File.WriteAllText(wnth, JsonUtility.ToJson(floorDatas, true));
    }


    [ContextMenu("json¸¸µé±â")]
    public void SDF()
    {
        FloorDatas asdf = new FloorDatas();

        string wnth = Path.Combine(Application.dataPath, "DataTable/asdf.json");

        File.WriteAllText(wnth, JsonUtility.ToJson(asdf, true));

    }
}
