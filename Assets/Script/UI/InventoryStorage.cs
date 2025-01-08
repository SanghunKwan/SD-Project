using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unit;
using UnityEngine;

[RequireComponent(typeof(CheckUICallChange))]
public class InventoryStorage : StorageComponent
{
    enum CannotInputItemReply
    {
        Full,
        TooBig,
        NeedArrangement
    }
    public enum Figure
    {
        One,
        Horizon,
        Vertic,
        Max
    }
    [Serializable]
    public class Slot
    {
        public int itemCode;
        public int itemCount { get; private set; }
        public List<int> brunchIndex;
        public int beforeSlotIndex = -1;
        Action<int> onChangeSlotIndex { get; set; } = (num) => { };

        public Slot()
        {
            itemCode = 0;
            itemCount = 0;
            brunchIndex = new List<int>(5);
        }
        public Slot(in Slot deepCopy)
        {
            itemCode = deepCopy.itemCode;
            itemCount = deepCopy.itemCount;
            brunchIndex = deepCopy.brunchIndex;
            beforeSlotIndex = deepCopy.beforeSlotIndex;

            ChangeListener(deepCopy);
        }
        public void SetSlot(int code, int count)
        {
            itemCode = code;
            itemCount = count;
        }
        public void SwapSlot(int targetSlotIndex, in Slot targetSlot)
        {
            onChangeSlotIndex = targetSlot.onChangeSlotIndex;
            onChangeSlotIndex(targetSlotIndex);
            itemCode = targetSlot.itemCode;
            itemCount = targetSlot.itemCount;
            brunchIndex = targetSlot.brunchIndex;

            beforeSlotIndex = targetSlot.beforeSlotIndex;

        }
        public void ChangeListener(Action<int> action)
        {
            onChangeSlotIndex = action;
        }
        public void ChangeListener(in Slot slot)
        {
            onChangeSlotIndex = (Action<int>)slot.onChangeSlotIndex.Clone();
        }
        public void SetBrunchIndex(int slotIndex)
        {
            brunchIndex.Add(slotIndex);
        }
        public void SetEmpty()
        {
            onChangeSlotIndex(beforeSlotIndex);
            onChangeSlotIndex = (num) => { };
            itemCode = 0;
            itemCount = 0;
            brunchIndex = new List<int>(5);
            beforeSlotIndex = -1;
        }
    }
    public Slot[] slots;

    [SerializeField] int slotInALIne;
    public (int itemCode, List<int> slotNeedMore)[] itemCode2slotData;

    int[] figure2step;
    public List<int> emptySlotList;

    delegate bool RefFunc(int num1, int num2, ref Slot slot);
    RefFunc[] checkEmptyFigure = new RefFunc[(int)Figure.Max];
    Action<CUnit, Item, float>[] itemAffectAction = new Action<CUnit, Item, float>[(int)SkillData.ItemSkillEffect.MAX];
    Action onAfterInit = () => { };
    Action<int>[] inventoryFullAction = new Action<int>[(int)InventoryComponent.InventoryType.Max];
    public Action<int, int> StoreEventCountFallUnderZero { get; set; } = (slotIndex, count) => { };
    public Action<int, int, int> StorePaymentEvent { get; set; } = (itemCode, firstCount, lastCount) => { };
    Action<int, int> onCountChanged = (slotIndex, forIndex) => { };

    CheckUICallChange uiCallChange;

    InventoryComponent.InventoryType m_type;

    public override void Init()
    {
        base.Init();

        uiCallChange = GetComponent<CheckUICallChange>();
        slots = Array.ConvertAll(new Slot[transform.GetChild(0).childCount], (i) => new Slot());

        itemCode2slotData = new ValueTuple<int, List<int>>[m_itemCounts.Length];

        for (int i = 0; i < m_itemCounts.Length; i++)
        {
            itemCode2slotData[i] = (-1, new List<int>(slots.Length));
        }

        figure2step = new int[(int)Figure.Max] { 0, 1, slotInALIne };

        emptySlotList = Enumerable.Range(0, slots.Length).ToList();

        SetInitFunc();
        SetInitAffectAction();
        SetAction();
        onAfterInit();
    }
    void SetInitFunc()
    {
        checkEmptyFigure[(int)Figure.One] = (int slotIndex, int needSlotCount, ref Slot slot)
                                => IsInventoryEmpty(slots[slotIndex]);

        checkEmptyFigure[(int)Figure.Horizon] = (int slotIndex, int needSlotCount, ref Slot slot)
                                => IsSlotEmpty(slotIndex, needSlotCount, IsInOneLine, Figure.Horizon, ref slot);

        checkEmptyFigure[(int)Figure.Vertic] = (int slotIndex, int needSlotCount, ref Slot slot)
                                => IsSlotEmpty(slotIndex, needSlotCount, IsInOneVerticalLine, Figure.Vertic, ref slot);
    }
    void SetInitAffectAction()
    {
        itemAffectAction[(int)SkillData.ItemSkillEffect.HEALING] = (cunit, item, percent) => AffectItemHealing(cunit, item, percent);
        itemAffectAction[(int)SkillData.ItemSkillEffect.Reinforcement] = (cunit, item, percent) => EquiptNewArm(cunit, item, percent);
    }
    void SetAction()
    {
        inventoryFullAction[(int)InventoryComponent.InventoryType.Stage] = ThrowAway;
        inventoryFullAction[(int)InventoryComponent.InventoryType.Villige] = GiveBack2Store;
        inventoryFullAction[(int)InventoryComponent.InventoryType.Store] = (num) => { };
    }
    public void AddInit(Action action)
    {
        onAfterInit += action;
    }
    public void SetType(InventoryComponent.InventoryType type)
    {
        m_type = type;
    }

    #region 기본 개수 증가
    public override void ItemCountChange(int itemCode, int addNum)
    {
        Item item = InventoryManager.i.info.items[itemCode];

        int codeIndex = itemCode2slotData[itemCode].itemCode;
        if (codeIndex != -1)
            SlotOperateWithCodeIndex(codeIndex, ref addNum, item);

        if (addNum == 0)
            return;

        CheckNeedMore(item, codeIndex);
        if (addNum >= 0)
        {

            IncreaseItemCount(item, addNum);
        }
        else
        {
            DecreaseItemCount(item.itemCode, addNum);
        }
    }
    public void IncreaseItemCount(in Item item, int addNum)
    {
        if (itemCode2slotData[item.itemCode].slotNeedMore.Count > 0)
        {
            itemCode2slotData[item.itemCode].itemCode = itemCode2slotData[item.itemCode].slotNeedMore[0];
            itemCode2slotData[item.itemCode].slotNeedMore.RemoveAt(0);
            ItemCountChange(item.itemCode, addNum);
            return;
        }

        if (AllocateSlot(item, out int emptyIndex))
        {
            ItemCountChangeBySlot(emptyIndex, addNum, item);
        }
        else
        {
            uiCallChange.nowIndex = (int)CannotInputItemReply.NeedArrangement;
            if (emptySlotList.Count < item.needSlots)
                uiCallChange.nowIndex = Convert.ToInt32(emptySlotList.Count > 0);

            uiCallChange.SetUICall();
            uiCallChange.PopUp(true);

            inventoryFullAction[(int)m_type](item.itemCode);
        }
    }
    void ThrowAway(int itemCode)
    {
        GameManager.manager.storageManager.ThrowAwayItem(Vector3.zero, itemCode);
    }
    void GiveBack2Store(int itemCode)
    {
        GameManager.manager.storageManager.inventoryComponents(InventoryComponent.InventoryType.Store)
            .inventoryStorage.ItemCountChange(itemCode, 1);
    }

    void DecreaseItemCount(int itemCode, int addNum)
    {
        int newSlotIndex = slots[itemCode2slotData[itemCode].itemCode].beforeSlotIndex;
        if (newSlotIndex == -1)
        {
            Debug.Log("전부 소모");
            //팝업 or 경고
            //모두 소모했습니다.
        }
        else
        {
            itemCode2slotData[itemCode].itemCode = newSlotIndex;
            ItemCountChange(itemCode, addNum);
        }
    }
    bool AllocateSlot(in Item item, out int emptyIndex)
    {
        int length = emptySlotList.Count;
        Slot slot = new Slot();

        for (int i = 0; i < length; i++)
        {
            if (GetEmptySlot(i, item, ref slot, out emptyIndex))
            {
                return true;
            }
        }
        emptyIndex = -1;
        return false;
    }
    bool GetEmptySlot(int index, in Item item, ref Slot slot, out int emptyIndex)
    {
        emptyIndex = -1;

        int length = item.needSlots;
        int slotIndex = emptySlotList[index];

        if (checkEmptyFigure[(int)item.figure](slotIndex, length, ref slot))
        {
            emptyIndex = slotIndex;
            EmptyListRemove(index, length, item.figure);
            return true;
        }
        else
            return false;
    }
    public void EmptySlotIndexRemove(int slotIndex, int itemSlots, Figure figure, int startIndex = 0)
    {
        int step = figure2step[(int)figure];
        for (int i = startIndex; i < itemSlots; i++)
        {
            emptySlotList.Remove(slotIndex + i * step);
        }
    }
    void EmptyListRemove(int listIndex, int itemSlots, Figure figure)
    {
        int slotIndex = emptySlotList[listIndex];
        emptySlotList.RemoveAt(listIndex);

        EmptySlotIndexRemove(slotIndex, itemSlots, figure, 1);
    }
    #endregion
    #region slot 개수 추가
    public void SlotOperateWithCodeIndex(int codeIndex, ref int addNum, in Item item)
    {
        Slot slot = slots[codeIndex];
        int count = addNum + slot.itemCount;
        int[] brunchArray = slot.brunchIndex.ToArray();
        if (m_type != InventoryComponent.InventoryType.Store)
        {
            count = Mathf.Clamp(count, 0, item.MaxCount);
        }
        addNum += slot.itemCount - count;
        //count는 최종 개수
        //addNum은 추가로 더해야할 수

        StorePaymentEvent(item.itemCode, slot.itemCount, count);

        ChangeCountByItem(slot, brunchArray, count);


        StoreEventCountFallUnderZero(codeIndex, count);


    }
    void CheckNeedMore(in Item item, int slotIndex)
    {
        List<int> needMore = itemCode2slotData[item.itemCode].slotNeedMore;

        int binaryIndex = needMore.BinarySearch(slotIndex);
        if (binaryIndex >= 0)
            needMore.RemoveAt(binaryIndex);

    }
    void ZeroSetEmpty(int codeIndex, int count)
    {
        if (count > 0)
            return;

        if (m_type != InventoryComponent.InventoryType.Store)
        {
            CheckCodetoSlot(codeIndex);
            SetSlotEmpty(codeIndex);
        }
    }
    void ChangeCountByItem(in Slot slot, in int[] brunchArray, int count)
    {
        Slot tempslot;
        int temp;
        int itemCode = slot.itemCode;
        int itemCount = count - slot.itemCount;
        int needSlots = brunchArray.Length;
        Debug.Log(itemCode.ToString() + "    " + count.ToString() + "    " + itemCount.ToString());

        BaseChangeItemCount(itemCode, itemCount);

        if (count == 0)
        {
            ZeroSetEmpty(brunchArray[0], count);
            itemCode = 0;
        }

        for (int i = 0; i < needSlots; i++)
        {
            temp = brunchArray[i];

            tempslot = slots[temp];
            tempslot.SetSlot(itemCode, count);

            onCountChanged(temp, i);
        }
    }
    public void BaseChangeItemCount(int itemCode, int itemCount)
    {
        base.ItemCountChange(itemCode, itemCount);
    }
    public void ItemCountChangeBySlot(int slotIndex, int addNum, in Item item)
    {
        SetNewItemSlot(slotIndex, item);

        int refAddNum = addNum;
        SlotOperateWithCodeIndex(slotIndex, ref refAddNum, item);
    }
    void SetNewItemSlot(int slotIndex, in Item item)
    {
        Slot slot = slots[slotIndex];
        int beforeIndex = itemCode2slotData[item.itemCode].itemCode;
        int length = item.needSlots;
        itemCode2slotData[item.itemCode].itemCode = slotIndex;
        itemCode2slotData[item.itemCode].slotNeedMore.Add(slotIndex);
        LinkIndex(beforeIndex, slotIndex);
        slot.SetSlot(item.itemCode, 0);
        int tempIndex;

        for (int i = 0; i < length; i++)
        {
            tempIndex = slotIndex + (figure2step[(int)item.figure] * i);
            slot.SetBrunchIndex(tempIndex);
            slots[tempIndex].brunchIndex = slot.brunchIndex;
            slots[tempIndex].SetSlot(item.itemCode, 0);
        }
    }
    public void LinkIndex(int beforeIndex, int emptyIndex)
    {
        if (beforeIndex >= 0)
        {
            slots[beforeIndex].ChangeListener((newIndex) => slots[emptyIndex].beforeSlotIndex = newIndex);
            slots[emptyIndex].beforeSlotIndex = beforeIndex;
        }
    }
    #endregion
    #region Slot empty 확인
    bool IsInventoryEmpty(in Slot slot)
    {
        return slot.itemCode == 0;
    }
    bool IsInOneLine(int slotIndex, int needSlotCount)
    {
        return (slotIndex % slotInALIne) + needSlotCount <= slotInALIne;
    }
    bool IsSlotEmpty(int slotIndex, int needSlotCount, Func<int, int, bool> lineCheck, Figure figure, ref Slot slot)
    {
        if (!lineCheck(slotIndex, needSlotCount))
            return false;

        int step = figure2step[(int)figure];
        int forCount = needSlotCount * step;
        for (int i = step; i < forCount; i += step)
        {
            slot = slots[slotIndex + i];
            if (!IsInventoryEmpty(slot))
                return false;
        }
        return true;
    }
    bool IsInOneVerticalLine(int slotIndex, int needSlotCount)
    {
        return slotIndex + ((needSlotCount - 1) * slotInALIne) < slots.Length;
    }
    #endregion

    #region 외부 데이터 공유
    public bool IsEnoughSpace(int slotIndex, int offset)
    {
        int[] tempArray = slots[slotIndex].brunchIndex.ToArray();
        int[] offsetArray = (int[])tempArray.Clone();
        int length = tempArray.Length;
        for (int i = 0; i < length; i++)
        {
            offsetArray[i] += offset;
        }
        return IsEnoughSpace(tempArray, slotInALIne, offsetArray, slotInALIne);
    }
    bool IsEnoughSpace(in int[] brunchArray1, int slotLine1, in int[] brunchArray2, int slotLine2)
    {
        int length = brunchArray1.Length;
        int originBrunch;
        int offsetBrunch;
        int brunchDifference = (brunchArray1[0] / slotLine1) - (brunchArray2[0] / slotLine2);

        for (int i = 0; i < length; i++)
        {
            originBrunch = brunchArray1[i] / slotLine1;
            offsetBrunch = brunchArray2[i] / slotLine2;

            if (originBrunch - brunchDifference != offsetBrunch)
                return false;
        }
        return true;
    }
    public bool IsEnoughSpaceWithOthers(in InventoryStorage inventory1, int slotIndex1, in InventoryStorage inventory2, int slotIndex2)
    {
        int[] tempArray = inventory1.slots[slotIndex1].brunchIndex.ToArray();
        int[] tempArray2 = (int[])tempArray.Clone();
        int length = tempArray.Length;
        int offset = slotIndex2 - slotIndex1;

        for (int i = 0; i < length; i++)
        {
            tempArray2[i] += offset;
        }

        return IsEnoughSpace
            (
            tempArray, inventory1.slotInALIne,
            tempArray2, inventory2.slotInALIne
            );
    }

    #endregion

    #region 외부 이벤트

    public void SetSlotEmpty(int slotIndex)
    {
        Slot slot = slots[slotIndex];
        int[] indexs = slot.brunchIndex.ToArray();
        int beforeNum = slot.beforeSlotIndex;
        int length = indexs.Length;
        if (beforeNum >= 0)
            slots[beforeNum].ChangeListener(slot);


        for (int i = 0; i < length; i++)
        {
            itemCode2slotData[slot.itemCode].slotNeedMore.Remove(indexs[i]);
            slot = slots[indexs[i]];
            slot.SetEmpty();
            CheckUseAll(indexs[i]);
        }
    }
    void CheckUseAll(int codeIndex, bool lessEqualZero = true)
    {
        if (!lessEqualZero)
            return;

        int newIndex = emptySlotList.BinarySearch(codeIndex);
        emptySlotList.Insert(~newIndex, codeIndex);
        itemCode2slotData[codeIndex].slotNeedMore.Remove(newIndex);
    }
    public void CheckCodetoSlot(int slotIndex)
    {
        int code = slots[slotIndex].itemCode;
        if (slots[slotIndex].brunchIndex.Contains(itemCode2slotData[code].itemCode))
        {
            itemCode2slotData[code].itemCode = slots[slotIndex].beforeSlotIndex;
        }
    }
    public void ItemCountChangeByIndex(int slotIndex, int addNum, out float usedNum)
    {
        Item item = InventoryManager.i.info.items[slots[slotIndex].itemCode];
        int originNum = addNum;

        SlotOperateWithCodeIndex(slotIndex, ref addNum, item);
        usedNum = -originNum + addNum;

        CheckNeedMore(item, slotIndex);
    }


    public void AffectItem(in CUnit itemUser, in Item item, float percent)
    {
        GameManager.manager.onItemUse.eventAction?.Invoke(item.itemCode, itemUser.transform.position);
        itemAffectAction[(int)item.itemSkillEffect](itemUser, item, percent);
    }
    void AffectItemHealing(in CUnit cUnit, in Item item, float percent)
    {
        int addHp = Mathf.CeilToInt(item.HP * percent);
        cUnit.Recovery(addHp);
        InputEffect.e.PrintEffect3(6, cUnit.transform);
    }
    void EquiptNewArm(in CUnit cUnit, in Item item, float percent)
    {
        int itemType = item.type - ItemType.EquipsHead;
        int itemLevel = Convert.ToInt32(item.itemCode > 16) + 2;
        cUnit.EquipUpgrade(itemType, itemLevel);
        cUnit.EquipOne(itemType);
        InputEffect.e.PrintEffect3(7, cUnit.transform);

    }
    public void AddListener(Action<int, int> callWhenCountChanged)
    {
        onCountChanged += callWhenCountChanged;
    }
    #endregion
}
