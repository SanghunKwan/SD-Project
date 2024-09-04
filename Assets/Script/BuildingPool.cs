using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPool : MonoBehaviour
{
    [SerializeField] Transform buildingList;
    [SerializeField] GameObject[] buildingPrefabs;
    [SerializeField] int repeatNum = 3;

    Dictionary<bool, Func<int, GameObject>> actions = new Dictionary<bool, Func<int, GameObject>>();

    void Start()
    {
        for (int i = 0; i < buildingList.childCount; i++)
        {
            GameObject folder = new GameObject(buildingList.GetChild(i).name);
            folder.transform.SetParent(transform);
            folder.SetActive(false);
            for (int j = 0; j < repeatNum; j++)
            {

                AddressableManager.BuildingImage build = Enum.Parse<AddressableManager.BuildingImage>(buildingList.GetChild(i).name);

                Instantiate(buildingPrefabs[(int)build], Vector3.zero, Quaternion.identity, folder.transform);
            }

        }

        actions.Add(true, GetObject);
        actions.Add(false, NewObject);
    }
    public GameObject PoolObject(AddressableManager.BuildingImage building)
    {
        int index = (int)building;
        bool boolConvert = Convert.ToBoolean(transform.GetChild(index).childCount);

        return actions[boolConvert](index);
    }

    GameObject GetObject(int index)
    {
        GameObject obj = transform.GetChild(index).GetChild(0).gameObject;
        obj.transform.SetParent(transform);

        return obj;
    }
    GameObject NewObject(int index)
    {
        return Instantiate(buildingPrefabs[index], Vector3.zero, Quaternion.identity, transform);
    }
}
