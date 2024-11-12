using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StorageComponent))]
public class VilligeStorage : MonoBehaviour, IStorageVisible
{
    VilligeStroageNode[] nodes;
    StorageComponent storageComponent;

    public Action<int>[] OnItemCountChanged { get; set; }
    private void Start()
    {
        storageComponent = GetComponent<StorageComponent>();
        nodes = GetComponentsInChildren<VilligeStroageNode>();

        storageComponent.AddListener(ChangeNum);

        OnItemCountChanged = new Action<int>[InventoryManager.i.info.items.Length];

        foreach (var item in nodes)
        {
            item.Init();
        }
    }
    public void ChangeNum(int itemCode)
    {
        OnItemCountChanged[itemCode](storageComponent.ItemCounts[itemCode]);
    }
}
