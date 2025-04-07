using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryStorage))]
public class InventoryPriceController : MonoBehaviour
{
    InventoryShowPrice[] prices;
    InventoryStorage inventoryStorage;
    [SerializeField] VilligeStorage itemUIStorageComponent;

    private void Start()
    {
        inventoryStorage = GetComponent<InventoryStorage>();

        Transform priceParent = transform.GetChild(1);
        int length = priceParent.childCount;
        prices = new InventoryShowPrice[length];
        StorageComponent.Item item;


        for (int i = 6; i <= 11; i++)
        {
            item = InventoryManager.i.info.items[i];
            inventoryStorage.IncreaseItemCount(item, 0);
        }

        for (int i = 0; i < length; i++)
        {
            prices[i] = priceParent.GetChild(i).GetComponent<InventoryShowPrice>();
            prices[i].Init();
            SetPrice(i);
            CheckSupplyCount(i, inventoryStorage.slots[i].itemCount);
        }
        inventoryStorage.StoreEventCountFallUnderZero = CheckSupplyCount;
        inventoryStorage.StorePaymentEvent = PayPrice;

    }
    void CheckSupplyCount(int slotIndex, int itemCount)
    {
        SupplyShow(slotIndex, itemCount <= 0);
    }
    void SupplyShow(int slotIndex, bool onoff)
    {
        prices[slotIndex].SetShow(onoff);
    }
    void SetPrice(int slotIndex)
    {
        prices[slotIndex].ChangePrice(InventoryManager.i.info.prices[inventoryStorage.slots[slotIndex].itemCode]);
    }
    public bool PayPrice(int itemCode, int firstCount, int lastCount)
    {
        int multiplyBase = Mathf.Min(firstCount, 0) - Mathf.Min(lastCount, 0);
        int payment = InventoryManager.i.info.prices[itemCode] * multiplyBase;
        int curMoney = itemUIStorageComponent.storageComponent.ItemCounts[12];

        if (curMoney < payment)
        {
            itemUIStorageComponent.NotEnoughNodeHighLight(payment, curMoney, 4);
            return false;
        }
        itemUIStorageComponent.storageComponent.ItemCountChange(12, -payment);
        return true;
    }
}
