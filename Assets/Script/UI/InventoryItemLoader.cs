using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemLoader : MonoBehaviour
{
    SaveData.InventoryData itemDatas;
    SaveData.HeroData[] heroDatas;
    int[] stageHeros;
    InventoryStorage inventory;

    private void Start()
    {
        SaveData.SaveDataInfo info = GameManager.manager.battleClearManager.SaveDataInfo;

        itemDatas = info.stageData.inventoryData;
        heroDatas = info.hero;
        stageHeros = info.stageData.heros;
        inventory = GameManager.manager.storageManager.inventoryComponents(InventoryComponent.InventoryType.Stage).inventoryStorage;

        LoadInventoryData();
    }
    void LoadInventoryData()
    {
        if (itemDatas.itemDatas == null)
            return;

        foreach (var item in itemDatas.itemDatas)
        {
            inventory.LoadItem(item.slotIndex, item.itemIndex, item.itemCount);
        }

        foreach (var item in itemDatas.corpseIndex)
        {
            inventory.LoadCorpseDataOnly(heroDatas[stageHeros[item]], item);
        }

    }
}
