using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryStorage))]
public class InventoryPriceController : MonoBehaviour
{
    InventoryShowPrice[] prices;
    InventoryStorage inventoryStorage;
    InventoryComponent inventoryComponent;
    [SerializeField] VilligeStorage itemUIStorageComponent;

    private void Start()
    {
        inventoryStorage = GetComponent<InventoryStorage>();
        inventoryComponent = GetComponent<InventoryComponent>();

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

        inventoryComponent.SlotCountZeroFunc = GetCountFromGold;
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
        int curMoney = itemUIStorageComponent.storageComponent.ItemCounts[13];

        if (curMoney < payment)
        {
            itemUIStorageComponent.NotEnoughNodeHighLight(payment, curMoney, 4);
            return false;
        }
        itemUIStorageComponent.storageComponent.ItemCountChange(13, -payment);
        return true;
    }

    int GetCountFromGold(int itemCode)
    {
        //골드 보유량을 아이템 개수로 환산해서 반환

        //itemCode 12 : 골드
        int maxBuy = itemUIStorageComponent.storageComponent.ItemCounts[13] / InventoryManager.i.info.prices[itemCode];
        if (maxBuy == 0)
        {
            itemUIStorageComponent.NotEnoughNodeHighLight(1, 0, 4);
            return 0;
        }

        int maxCount = InventoryManager.i.info.items[itemCode].MaxCount;
        int needMoreSlotIndex = inventoryStorage.itemCode2slotData[itemCode].itemCode;
        int fillCount = maxCount - ((needMoreSlotIndex != -1) ? inventoryStorage.slots[needMoreSlotIndex].itemCount % maxCount : 0);


        return Mathf.Min(maxBuy, Mathf.Min(maxCount, fillCount));
    }
}
