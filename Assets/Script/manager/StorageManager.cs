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
    [SerializeField] StorageComponent villigeStorage;
    GameManager gameManager;

    [SerializeField] Vector3 offset;
    GameObject lastFinder;
    CObject lastFinderCObject;

    public InventoryComponent inventoryComponents(InventoryComponent.InventoryType type)
    {
        return m_inventoryComponents[(int)type];
    }

    void Start()
    {
        gameManager = GameManager.manager;
        gameManager.storageManager = this;
    }
    public void AddItem(int itemCode, int itemCount, GameObject itemFinder)
    {
        m_inventoryComponents[(int)InventoryComponent.InventoryType.Stage].inventoryStorage.ItemCountChange(itemCode, itemCount);

        SetLastFinder(itemFinder);
    }
    void SetLastFinder(GameObject itemFinder)
    {
        if (lastFinder == itemFinder)
            return;

        lastFinder = itemFinder;
        lastFinderCObject = null;
    }
    public void AddCorpse(GameObject itemFinder, Hero deadHeroData)
    {
        int heroIndex = deadHeroData.heroInStageIndex;

        m_inventoryComponents[(int)InventoryComponent.InventoryType.Stage].inventoryStorage.
            AddCorpse(gameManager.battleClearManager.SaveDataInfo.hero[heroIndex], heroIndex);

        SetLastFinder(itemFinder);
    }
    public void ThrowAwaySlotAll(in Vector3 clickPoint, InventoryStorage.Slot slot)
    {
        int itemCount = slot.itemCount;

        ThrowAwaySlot(clickPoint, itemCount, slot.itemCode);
    }
    public void ThrowAwaySlot(in Vector3 clickPoint, int itemCount, int itemCode)
    {
        int layerMask = 1 << LayerMask.NameToLayer("UI");
        int poolCode = ItemCode2ItemPoolCode(itemCode);
        CheckCorpseThrowAway(itemCode, itemCount);

        Physics.Raycast(Camera.main.ScreenPointToRay(clickPoint), out RaycastHit hit, 30, ~layerMask);

        Dictionary<GameObject, CUnit> clist = (from character in PlayerNavi.nav.lists
                                               select character.cUnit).ToDictionary((unit) => unit.gameObject, (unit) => unit);

        CObject target = gameManager.GetNearest(clist, hit.point, (character) => true, 35);

        ThrowAwaySave(itemCount, poolCode, target, gameManager.IsOnOneRight(target, hit.point), out _);
    }
    void CheckCorpseThrowAway(int itemCode, int itemCount)
    {
        if (itemCode != 12)
            return;

        for (int i = 0; i < itemCount; i++)
        {
            inventoryComponents(InventoryComponent.InventoryType.Stage).inventoryStorage.LoseCorpse();
        }
    }
    public void ThrowAwayItem(in Vector3 clickPoint, int itemCode)
    {
        int itemCount = 1;
        int poolCode = ItemCode2ItemPoolCode(itemCode);
        int layerMask = 1 << LayerMask.NameToLayer("UI");
        CheckCorpseThrowAway(itemCode, itemCount);

        Physics.Raycast(Camera.main.ScreenPointToRay(clickPoint), out RaycastHit hit, 30, ~layerMask);

        if (lastFinderCObject == null)
            lastFinderCObject = lastFinder.GetComponent<CObject>();

        ThrowAwaySave(itemCount, poolCode, lastFinderCObject, gameManager.IsOnOneRight(lastFinderCObject, hit.point), out _);
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
        DropManager.instance.pool.CheckPosition(dropItem);
        dropItem.transform.RotateAround(target.transform.position, Vector3.up, target.transform.eulerAngles.y + addAngle);


    }
    int ItemCode2ItemPoolCode(int itemCode) => itemCode - 1;

}
