using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StorageComponent))]
public class VilligeStorage : MonoBehaviour, IStorageVisible
{
    VilligeStroageNode[] nodes;
    public StorageComponent storageComponent { get; private set; }
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

    #region 상점 세팅 action
    void LinkWithStore()
    {
        for (int i = 6; i <= 11; i++)
        {
            storeInventory.ItemCountChange(i, storageComponent.ItemCounts[i]);
        }
    }
    #endregion
    #region NodeHighLight
    public void NotEnoughNodeHighLight(MaterialsData.NeedMaterials needMaterials)
    {
        NotEnoughNodeHighLight(needMaterials.grayNum, storageComponent.ItemCounts[1], 0);
        NotEnoughNodeHighLight(needMaterials.blackNum, storageComponent.ItemCounts[2], 1);
        NotEnoughNodeHighLight(needMaterials.whiteNum, storageComponent.ItemCounts[3], 2);
        NotEnoughNodeHighLight(needMaterials.timberNum, storageComponent.ItemCounts[4], 3);
        NotEnoughNodeHighLight(needMaterials.money, storageComponent.ItemCounts[12], 4);
    }
    public void NotEnoughNodeHighLight(int needItem, int itemCount, int nodeIndex)
    {
        if (needItem > itemCount)
            nodes[nodeIndex].PlayHighLight();
    }
    #endregion
}
