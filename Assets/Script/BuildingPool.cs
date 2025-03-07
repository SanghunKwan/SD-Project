using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPool : MonoBehaviour
{
    [SerializeField] Transform buildingList;
    [SerializeField] BuildingConstructDelay[] buildingPrefabs;
    [SerializeField] int repeatNum = 3;

    Dictionary<GameObject, BuildingConstructDelay> dicBuilding = new();
    Dictionary<bool, Func<int, Vector3, BuildingConstructDelay>> actions = new();

    void Start()
    {
        GameObject folder;
        AddressableManager.BuildingImage build;

        for (int i = 0; i < buildingList.childCount; i++)
        {
            folder = new GameObject(buildingList.GetChild(i).name);
            folder.transform.SetParent(transform);
            folder.SetActive(false);
            build = Enum.Parse<AddressableManager.BuildingImage>(buildingList.GetChild(i).name);

            for (int j = 0; j < repeatNum; j++)
            {
                NewBuilding((int)build, Vector3.zero, folder.transform);
            }
        }

        actions.Add(true, GetBuilding);
        actions.Add(false, NewBuilding);
    }

    public BuildingConstructDelay PoolBuilding(AddressableManager.BuildingImage building, in Vector3 createPosition)
    {
        int index = (int)building;
        bool boolConvert = Convert.ToBoolean(transform.GetChild(index).childCount);

        return actions[boolConvert](index, createPosition);
    }
    BuildingConstructDelay GetBuilding(int index, Vector3 vec)
    {
        BuildingConstructDelay obj = dicBuilding[transform.GetChild(index).GetChild(0).gameObject];
        obj.transform.position = vec;
        obj.transform.SetParent(transform);

        return obj;
    }

    BuildingConstructDelay NewBuilding(int index, Vector3 vec)
    {
        return NewBuilding(index, vec, transform);
    }
    BuildingConstructDelay NewBuilding(int index, Vector3 vec, Transform parentTransform)
    {
        BuildingConstructDelay tempBuildingComponent
            = Instantiate(buildingPrefabs[index], vec, Quaternion.identity, parentTransform);
        dicBuilding.Add(tempBuildingComponent.gameObject, tempBuildingComponent);

        return tempBuildingComponent;
    }
}
