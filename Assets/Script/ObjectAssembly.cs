using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAssembly : MonoBehaviour
{
    [Serializable]
    public class FloorsHeight
    {
        public float height;
        public GameObject floor;
    }

    [SerializeField] FloorsHeight top;
    public FloorsHeight[] floors;
    [SerializeField] FloorsHeight bottom;

    public Action init { get; set; } = () => { };
    public Action<TowerAssembleClick[]> init2 { get; set; } = (tower) => { };



    List<FloorsHeight> floorsList = new List<FloorsHeight>();

    float[] floorAngles;
    public float towerHeight { get; private set; }

    private void Start()
    {
        if (GameManager.manager.battleClearManager != null)
            LateStart();
        else
            GameManager.manager.onBattleClearManagerRegistered += LateStart;
    }
    void LateStart()
    {
        AddFloor();

        floorsList.Add(top);

        CreateFloor();

        init();
        init2(GetComponentsInChildren<TowerAssembleClick>());
    }
    void AddFloor()
    {
        SaveData.FloorData data = GameManager.manager.battleClearManager.SaveDataInfo.floorData;

        int[] floorlooks = data.floorLooks;
        floorAngles = data.floorAngles;

        foreach (int type in floorlooks)
        {
            floorsList.Add(floors[type]);
        }
    }
    void CreateFloor()
    {
        towerHeight = 0;
        GameObject tempObject;
        for (int i = 0; i < floorsList.Count; i++)
        {
            tempObject = Instantiate(floorsList[i].floor, Vector3.up * towerHeight, floorsList[i].floor.transform.rotation,
                                        transform.GetChild(0).GetChild(0));

            if (i != 0 && i != floorsList.Count - 1)
                tempObject.transform.Rotate(Vector3.up, floorAngles[i - 1], Space.World);

            towerHeight += floorsList[i].height;
        }
    }

}
