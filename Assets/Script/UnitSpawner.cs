using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] SpawnManager SpawnManager;

    [SerializeField] Monster[] monsters;
    [SerializeField] Hero[] heroes;
    [SerializeField] CObject[] cobjects;
    [SerializeField] BuildingComponent[] buildings;

    Action[] actions;

    public enum SpawnType
    {
        Monsters,
        Objects,
        Buildings,
        Heros,
    }

    private void Awake()
    {
        actions = new Action[3];
    }


    public void SpawnObject()
    {

    }
}
