using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilligeItemsLoad : MonoBehaviour
{
    StorageComponent storageComponent;
    private void Awake()
    {
        storageComponent = GetComponent<StorageComponent>();
    }

    private void Start()
    {
        GameManager.manager.onBattleClearManagerRegistered += ItemDataLoad;
    }

    void ItemDataLoad()
    {
        storageComponent.Init();

        SaveData.SaveDataInfo info = GameManager.manager.battleClearManager.SaveDataInfo;
        int length = info.items.Length;

        for (int i = 0; i < length; i++)
        {
            storageComponent.ItemCountChange(i, info.items[i]);
        }

        foreach (var item in info.stageData.inventoryData.itemDatas)
        {
            storageComponent.ItemCountChange(item.itemIndex, item.itemCount);
            Debug.Log("스테이지 아이템 로드");
        }
    }

}
