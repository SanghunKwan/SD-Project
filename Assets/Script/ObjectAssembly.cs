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
    [SerializeField] FloorsHeight[] floors;
    [SerializeField] FloorsHeight bottom;


    List<FloorsHeight> floorsList = new List<FloorsHeight>();

    private void Awake()
    {
        floorsList.Add(bottom);
        AddFloor();
        floorsList.Add(top);

        CreateFloor();

    }
    void AddFloor()
    {
        
    }
    void CreateFloor()
    {
        float y = 0;
        foreach (var item in floorsList)
        {
            Instantiate(item.floor, Vector3.up * y, item.floor.transform.rotation, transform.GetChild(0).GetChild(0)).layer = 9;
            y += item.height;
        }
    }

}