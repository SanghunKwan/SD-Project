using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unit;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        if (heroIndex < 0)
        {
            //spawnedHeroCorpse
            //saveData 영웅 목록에 hero를 heroData로 바꿔 저장.
            int allHeroCount = gameManager.battleClearManager.SaveDataInfo.hero.Length;
            heroIndex = gameManager.battleClearManager.SaveDataInfo.stageData.heros.Length;
            //saveData의 마지막에 추가.
            Array.Resize(ref gameManager.battleClearManager.SaveDataInfo.stageData.heros, heroIndex + 1);
            Array.Resize(ref gameManager.battleClearManager.SaveDataInfo.hero, allHeroCount + 1);
            //heroIndex를 수정.
            SaveData.HeroData heroData = new SaveData.HeroData(deadHeroData);
            heroData.unitData.objectData.isDead = true;
            heroData.SetDefaultName();
            gameManager.battleClearManager.SaveDataInfo.hero[allHeroCount] = heroData;

            gameManager.battleClearManager.SaveDataInfo.stageData.heros[heroIndex] = allHeroCount;
        }


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

        CObject target = gameManager.GetNearest(gameManager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero], hit.point);

        ItemDrop(new SaveData.YetDroppedItem(itemCount, poolCode, target, gameManager.IsOnOneRight(target, hit.point)));
        //ThrowAwaySave(itemCount, poolCode, target, gameManager.IsOnOneRight(target, hit.point), out _);
    }
    void ItemDrop(SaveData.YetDroppedItem dropItems)
    {
        GameManager.manager.objectManager.AddYetDroppedItem(dropItems);
        DropManager.instance.pool.CallItems(dropItems);
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
            lastFinderCObject = gameManager.objectManager.GetNode(lastFinder).Value;

        if (lastFinderCObject != null)
        {
            ItemDrop(new SaveData.YetDroppedItem(itemCount, poolCode, lastFinderCObject, gameManager.IsOnOneRight(lastFinderCObject, hit.point)));
        }
        //ThrowAwaySave(itemCount, poolCode, lastFinderCObject, gameManager.IsOnOneRight(lastFinderCObject, hit.point), out _);
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
        GameObject dropItem = DropManager.instance.pool.CallItem(poolCode, target.stageIndex);
        dropItem.SetActive(true);

        dropItem.transform.position = target.transform.position + offset;
        DropManager.instance.pool.CheckPosition(dropItem);
        dropItem.transform.RotateAround(target.transform.position, Vector3.up, target.transform.eulerAngles.y + addAngle);
    }



    int ItemCode2ItemPoolCode(int itemCode) => itemCode - 1;

}
