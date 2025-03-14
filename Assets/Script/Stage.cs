using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectPrefabs : StagePrefabsCaller
{
    [SerializeField] GameObject[] stageObjects;
    GameObject[] stagePool;
    void Start()
    {
        int length = stagePoolManager.stageFloors.Length;
        stagePool = new GameObject[length];
        for (int i = stagePoolManager.startFloorIndex; i < length; i++)
        {
            stagePool[i]
                = PlacePrefab(GameManager.manager.battleClearManager.stageFloorComponents[i], stagePoolManager.stageFloors[i]);
        }
        stagePool[stagePoolManager.startFloorIndex].SetActive(true);
    }
    public override GameObject PlacePrefab(StageFloorComponent floor, int floorNum)
    {
        return Instantiate(stageObjects[floorNum], floor.transform.position, stageObjects[floorNum].transform.rotation, transform);
    }

}
