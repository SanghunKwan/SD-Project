using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMonsterPrefabs : StagePrefabsCaller
{
    [SerializeField] GameObject[] stageObjects;
    void Start()
    {
        int length = stagePoolManager.stageFloors.Length;
        for (int i = stagePoolManager.startFloorIndex + System.Convert.ToInt32(stagePoolManager.isLoaded); i < length; i++)
        {
            PlacePrefab(GameManager.manager.battleClearManager.stageFloorComponents[i], stagePoolManager.stageFloors[i]);
            if (i == 0) transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public override GameObject PlacePrefab(StageFloorComponent floor, int floorNum)
    {
        return Instantiate(stageObjects[floorNum], floor.transform.position, stageObjects[floorNum].transform.rotation, transform);
    }

}
