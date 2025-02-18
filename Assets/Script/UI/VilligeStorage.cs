using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StorageComponent))]
public class VilligeStorage : MonoBehaviour, IStorageVisible
{
    VilligeStroageNode[] nodes;
    StorageComponent storageComponent;
    [SerializeField] InventoryStorage storeInventory;

    public Action<int>[] OnItemCountChanged { get; set; }
    private void Start()
    {
        storageComponent = GetComponent<StorageComponent>();
        nodes = GetComponentsInChildren<VilligeStroageNode>();

        storageComponent.AddListener(ChangeNum);

        int length = InventoryManager.i.info.items.Length;
        OnItemCountChanged = new Action<int>[length];
        for (int i = 0; i < length; i++)
            OnItemCountChanged[i] = (num) => { };

        foreach (var item in nodes)
        {
            item.Init();
        }

        storeInventory.AddInit(LinkWithStore);
    }
    public void ChangeNum(int itemCode)
    {
        OnItemCountChanged[itemCode](storageComponent.ItemCounts[itemCode]);
    }

    #region ���� ���� action
    void LinkWithStore()
    {
        for (int i = 6; i <= 11; i++)
        {
            storeInventory.ItemCountChange(i, storageComponent.ItemCounts[i]);
        }
    }
    #endregion
}
