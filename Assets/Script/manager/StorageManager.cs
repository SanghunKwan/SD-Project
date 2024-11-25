using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System;

public class StorageManager : MonoBehaviour
{
    [SerializeField] InventoryComponent[] m_inventoryComponents;
    GameManager gameManager;

    [SerializeField] Vector3 offset;

    public InventoryComponent inventoryComponents(InventoryComponent.InventoryType type)
    {
        return m_inventoryComponents[(int)type];
    }

    void Start()
    {
        gameManager = GameManager.manager;
        gameManager.storageManager = this;
    }
    public void AddItem(int itemCode, int itemCount)
    {
        m_inventoryComponents[(int)InventoryComponent.InventoryType.Stage].inventoryStorage.ItemCountChange(itemCode, itemCount);
    }
    public void ThrowAwaySlot(in Vector3 clickPoint, InventoryStorage.Slot slot)
    {
        int itemCount = slot.itemCount;
        int poolCode = ItemCode2ItemPoolCode(slot.itemCode);
        int layerMask = 1 << LayerMask.NameToLayer("UI");

        ThrowAwayAct(clickPoint, layerMask, itemCount, poolCode);
    }
    public void ThrowAwayItem(in Vector3 clickPoint, int itemCode)
    {
        int itemCount = 1;
        int poolCode = ItemCode2ItemPoolCode(itemCode);
        int layerMask = 1 << LayerMask.NameToLayer("UI");

        ThrowAwayAct(clickPoint, layerMask, itemCount, poolCode);
    }
    void ThrowAwayAct(in Vector3 clickPoint, int layerMask, int itemCount, int poolCode)
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(clickPoint), out RaycastHit hit, 30, ~layerMask);

        List<CUnit> clist = (from character in PlayerNavi.nav.lists
                             select character.cUnit).ToList();

        CObject target = gameManager.GetNearest(clist, hit.point, (character) => true, 35);


        ThrowAwaySave(itemCount, poolCode, target, gameManager.IsOnOneRight(target, hit.point), out _);
    }

    void ThrowAwaySave(int itemCount, int poolCode, CObject target, bool isRight, out Task task)
    {
        task = RepeatThrowAway(itemCount, poolCode, target, isRight);
    }
    async Task RepeatThrowAway(int itemCount, int poolCode, CObject target, bool isRight)
    {
        float addAngle = Convert.ToInt32(!isRight) * 180;
        for (int i = 0; i < itemCount; i++)
        {
            ThrowAwayItem(poolCode, target, addAngle);
            await Task.Delay(100);
        }
    }
    void ThrowAwayItem(int poolCode, in CObject target, float addAngle)
    {
        GameObject dropItem = DropManager.instance.pool.CallItem(poolCode);
        dropItem.SetActive(true);

        dropItem.transform.position = target.transform.position + offset;
        dropItem.transform.RotateAround(target.transform.position, Vector3.up, target.transform.eulerAngles.y + addAngle);


    }
    int ItemCode2ItemPoolCode(int itemCode) => itemCode - 1;
}
