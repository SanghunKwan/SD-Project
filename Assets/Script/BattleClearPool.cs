using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleClearManager))]
public class BattleClearPool : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    [SerializeField] StageFloorComponent[] stages;
    [SerializeField] GameObject link;
    [SerializeField] int poolNum;

    void Start()
    {
        new GameObject("Objects").transform.SetParent(transform);
        new GameObject("Stages").transform.SetParent(transform);
        new GameObject("Link").transform.SetParent(transform);

        MakePoolObject(0, objects, poolNum);

        for (int i = 0; i < poolNum; i++)
            Instantiate(link, transform.GetChild(2));

    }
    void MakePoolObject<T>(int folderIndex, in T[] array, int poolStorageCount) where T : Object
    {
        int length = array.Length;
        Transform objectsFolder = transform.GetChild(folderIndex);

        for (int i = 0; i < length; i++)
        {
            new GameObject(array[i].name).transform.SetParent(objectsFolder);
            for (int j = 0; j < poolStorageCount; j++)
            {
                Instantiate(objects[i], objectsFolder.GetChild(i));
            }
        }
    }
    public StageFloorComponent MakeStage(int stageIndex)
    {
        return Instantiate(stages[stageIndex], transform.GetChild(1));
    }
    public GameObject MakeLink()
    {
        return transform.GetChild(2).GetChild(0).gameObject;
    }
}