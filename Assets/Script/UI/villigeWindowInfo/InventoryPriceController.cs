using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryStorage))]
public class InventoryPriceController : MonoBehaviour
{
    InventoryShowPrice[] prices;
    InventoryStorage inventoryStorage;

    private void Start()
    {
        inventoryStorage = GetComponent<InventoryStorage>();
        Transform priceParent = transform.GetChild(1);
        int length = priceParent.childCount;
        prices = new InventoryShowPrice[length];

        for (int i = 0; i < length; i++)
        {
            prices[i] = priceParent.GetChild(i).GetComponent<InventoryShowPrice>();
            prices[i].Init();
            SetPrice(i);
            CheckSupplyCount(i, inventoryStorage.slots[i].itemCount);
        }
        inventoryStorage.StoreEventCountFallUnderZero = CheckSupplyCount;
    }
    public void CheckSupplyCount(int slotIndex, int itemCount)
    {
        SupplyShow(slotIndex, itemCount <= 0);
    }
    public void SupplyShow(int slotIndex, bool onoff)
    {
        prices[slotIndex].SetShow(onoff);
    }
    public void SetPrice(int slotIndex)
    {
        prices[slotIndex].ChangePrice(InventoryManager.i.info.prices[inventoryStorage.slots[slotIndex].itemCode]);
    }
}
