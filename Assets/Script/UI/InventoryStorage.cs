using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unit;
using UnityEngine;
using UnityEngine.UIElements;

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

            foreach (var item in deepCopy.onChangeSlotIndex.GetInvocationList())
            {
                onChangeSlotIndex += (Action<int>)item;
            }
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
        public void AddListener(Action<int> action)
        {
            onChangeSlotIndex = action;
        }
        public void SetBrunchIndex(int slotIndex)
        {
            brunchIndex.Add(slotIndex);
        }
        public void SetEmpty()
        {
            onChangeSlotIndex(-1);
            itemCode = 0;
            itemCount = 0;
            brunchIndex = new List<int>(5);
            beforeSlotIndex = -1;
        }
    }
    public Slot[] slots;

    [SerializeField] int slotInALIne;
    public int[] itemcode2slotindex;
    int[] figure2step;
    public List<int> emptySlotList;

    delegate bool RefFunc(int num1, int num2, ref Slot slot);
    RefFunc[] checkEmptyFigure = new RefFunc[(int)Figure.Max];
    Action<CUnit, Item, float>[] itemAffectAction = new Action<CUnit, Item, float>[(int)SkillData.ItemSkillEffect.MAX];
    Action onAfterInit = () => { };

    CheckUICallChange uiCallChange;

    InventoryComponent.InventoryType m_type;

    public override void Init()
    {
        base.Init();

        uiCallChange = GetComponent<CheckUICallChange>();
        slots = Array.ConvertAll(new Slot[transform.GetChild(0).childCount], (i) => new Slot());

        itemcode2slotindex = new int[m_itemCounts.Length];
        Array.Fill(itemcode2slotindex, -1);

        figure2step = new int[(int)Figure.Max] { 0, 1, slotInALIne };

        emptySlotList = Enumerable.Range(0, slots.Length).ToList();

        SetInitFunc();
        SetInitAffectAction();
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

        int codeIndex = itemcode2slotindex[itemCode];
        if (codeIndex != -1)
            SlotOperateWithCodeIndex(codeIndex, ref addNum, item);

        if (addNum == 0 && m_type != InventoryComponent.InventoryType.Store)
            return;

        if (addNum >= 0)
        {
            IncreaseItemCount(item, codeIndex, addNum);
        }
        else
        {
            DecreaseItemCount(item.itemCode, addNum);
        }
    }

    void SlotOperateWithCodeIndex(int codeIndex, ref int addNum, in Item item)
    {
        Debug.Log(codeIndex.ToString() + addNum);
        Slot slot = slots[codeIndex];
        int count = addNum + slot.itemCount;
        if (m_type != InventoryComponent.InventoryType.Store)
        {
            count = Mathf.Clamp(count, 0, item.MaxCount);
        }
        addNum += slot.itemCount - count;
        //count는 최종 개수
        //addNum은 더하는 개수
        ChangeCountBySlot(slot, count);
        if (m_type == InventoryComponent.InventoryType.Store)
            return;

        if (count <= 0)
        {
            slot.SetEmpty();
            CheckCodetoSlot(codeIndex);
        }
        CheckUseAll(codeIndex, count <= 0);
    }
    void CheckUseAll(int codeIndex, bool lessEqualZero = true)
    {
        if (!lessEqualZero)
            return;


        int newIndex = emptySlotList.BinarySearch(codeIndex);
        emptySlotList.Insert(~newIndex, codeIndex);
    }

    void IncreaseItemCount(in Item item, int beforeIndex, int addNum)
    {
        if (AllocateSlot(item, out int emptyIndex))
        {
            int step = figure2step[(int)item.figure];
            itemcode2slotindex[item.itemCode] = emptyIndex;
            LinkIndex(beforeIndex, emptyIndex);


            if (m_type != InventoryComponent.InventoryType.Store && addNum > item.MaxCount)
            {
                addNum = item.MaxCount;
                IncreaseItemCount(item, emptyIndex, addNum - item.MaxCount);
            }
            ChangeCount(item.itemCode, addNum, item.needSlots, step);
        }
        else
        {
            uiCallChange.nowIndex = (int)CannotInputItemReply.NeedArrangement;
            if (emptySlotList.Count < item.needSlots)
                uiCallChange.nowIndex = Convert.ToInt32(emptySlotList.Count > 0);

            uiCallChange.SetUICall();
            uiCallChange.PopUp(true);
        }
    }
    public void LinkIndex(int beforeIndex, int emptyIndex)
    {
        if (beforeIndex >= 0)
        {
            slots[beforeIndex].AddListener((newIndex) => slots[emptyIndex].beforeSlotIndex = newIndex);
            slots[emptyIndex].beforeSlotIndex = beforeIndex;
        }
    }
    void DecreaseItemCount(int itemCode, int addNum)
    {
        int newSlotIndex = slots[itemcode2slotindex[itemCode]].beforeSlotIndex;
        if (newSlotIndex == -1)
        {
            //팝업 or 경고
            //모두 소모했습니다.
        }
        else
        {
            itemcode2slotindex[itemCode] = newSlotIndex;
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
        int step = figure2step[(int)item.figure];

        if (checkEmptyFigure[(int)item.figure](slotIndex, length, ref slot))
        {
            emptyIndex = slotIndex;
            emptySlotList.RemoveAt(index);
            for (int i = 1; i < length; i++)
            {
                emptySlotList.Remove(slotIndex + i * step);
            }
            return true;
        }
        else
            return false;
    }

    void ChangeCount(int itemCode, int itemCount, int needSlots, int step)
    {
        int codeIndex = itemcode2slotindex[itemCode];
        int temp;
        Slot tempslot = slots[codeIndex];
        List<int> tempList = tempslot.brunchIndex;

        tempList.Clear();
        m_itemCounts[itemCode] += itemCount - tempslot.itemCount;

        for (int i = 0; i < needSlots; i++)
        {
            temp = codeIndex + i * step;
            tempslot = slots[temp];
            tempslot.SetSlot(itemCode, itemCount);
            tempslot.brunchIndex = tempList;
            tempslot.SetBrunchIndex(temp);
            eventAlert(temp);
        }
    }
    public void DecreaseSelectedSlot(in Slot slot)
    {
        m_itemCounts[slot.itemCode] -= slot.itemCount;
    }
    #endregion
    #region slot 개수 추가
    public void ItemCountChangeBySlot(int slotIndex, int addNum, in Item item)
    {
        Slot slot = slots[slotIndex];

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
    public int GetEmptySlotCount()
    {
        return emptySlotList.Count;
    }
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

            if (originBrunch + brunchDifference != offsetBrunch)
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
    public void SetSlot(in int[]  brunchArray ,in Slot slot, int offset)
    {
        int length = brunchArray.Length;
        int tempIndex;
        ReturnCodetoSlot(slot, slot.beforeSlotIndex, offset);
        for (int i = 0; i < length; i++)
        {
            slot.brunchIndex[i] += offset;
            tempIndex = slot.brunchIndex[i];
            slots[tempIndex].SwapSlot(tempIndex, slot);
            LinkIndex(slot.beforeSlotIndex, tempIndex);
            emptySlotList.Remove(tempIndex);
        }
    }
    public void SetSlotEmpty(int slotIndex)
    {
        Slot slot = slots[slotIndex];
        int beforeNum = slot.beforeSlotIndex;
        if (beforeNum >= 0)
        {
            slots[beforeNum].AddListener((num) => { });
        }
        slot.SetEmpty();
        CheckUseAll(slotIndex);
    }
    public void CheckCodetoSlot(int slotIndex)
    {
        int code = slots[slotIndex].itemCode;
        if (slots[slotIndex].brunchIndex.Contains(itemcode2slotindex[code]))
        {
            itemcode2slotindex[code] = slots[slotIndex].beforeSlotIndex;
        }
    }
    public void ReturnCodetoSlot(in Slot slot, int beforeSlotIndex, int offset)
    {
        int code = slot.itemCode;
        if (itemcode2slotindex[code] == beforeSlotIndex)
        {
            itemcode2slotindex[code] = slot.brunchIndex[0] + offset;
        }
    }
    public void ItemCountChangeByIndex(int slotIndex, int addNum, out float usedNum)
    {
        Item item = InventoryManager.i.info.items[slots[slotIndex].itemCode];
        int originNum = addNum;

        SlotOperateWithCodeIndex(slotIndex, ref addNum, item);
        usedNum = -originNum + addNum;
    }


    void ChangeCountBySlot(in Slot slot, int count)
    {
        Slot tempslot;
        int temp;
        int itemCode = slot.itemCode;
        int itemCount = slot.itemCount - count;
        int needSlots;
        int[] brunchArray = slot.brunchIndex.ToArray();
        needSlots = brunchArray.Length;

        m_itemCounts[itemCode] -= itemCount;
        for (int i = 0; i < needSlots; i++)
        {
            temp = brunchArray[i];

            tempslot = slots[temp];
            tempslot.SetSlot(itemCode, count);
            eventAlert(temp);
        }
    }

    public void AffectItem(in CUnit itemUser, int itemCode, float percent)
    {
        Item item = InventoryManager.i.info.items[itemCode];
        itemAffectAction[(int)item.itemSkillEffect](itemUser, item, percent);
    }
    void AffectItemHealing(in CUnit cUnit, in Item item, float percent)
    {
        int addHp = Mathf.CeilToInt(item.HP * percent);
        cUnit.Recovery(addHp);
    }
    #endregion


}
