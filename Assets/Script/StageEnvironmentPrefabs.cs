using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnvironmentPrefabs : StagePrefabsCaller
{
    int m_index;
    [SerializeField] StageObject[] stageObjects;

    [System.Serializable]
    public class StageObject
    {
        public string name;
        public GameObject[] objects;
    }

    void Start()
    {
        int length = stagePoolManager.stageFloors.Length;
        for (int i = stagePoolManager.startFloorIndex + System.Convert.ToInt32(stagePoolManager.isLoaded); i < length; i++)
        {
            PlacePrefab(GameManager.manager.battleClearManager.stageFloorComponents[i], RandomInRange(stagePoolManager.stageFloors[i], out m_index));
            if (i == 0) transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public int RandomInRange(int floors, out int index)
    {
        index = GameManager.manager.battleClearManager.PoolStageIndex(floors);
        int length = stageObjects[index].objects.Length;

        return Random.Range(0, length);
    }
    public override GameObject PlacePrefab(StageFloorComponent floor, int floorNum)
    {
        return Instantiate(stageObjects[m_index].objects[floorNum], floor.transform.position, stageObjects[m_index].objects[floorNum].transform.rotation, transform);
    }

}
