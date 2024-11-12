using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryStorage : StorageComponent
{
    public enum Figure
    {
        One,
        Horizon,
        Vertic,
        Max
    }
    public class Slot
    {
        public int itemCode { get; private set; }
        public int itemCount { get; private set; }
        List<int> brunchIndex = new List<int>();
        public int beforeSlotIndex { get; set; } = -1;
        Action<int> onChangeSlotIndex = (num) => { };

        public Slot()
        {
            itemCode = 0;
            itemCount = 0;
            brunchIndex.Capacity = 5;

        }
        public Slot(in Slot deepCopy)
        {
            itemCode = deepCopy.itemCode;
            itemCount = deepCopy.itemCount;
            brunchIndex = new List<int>(deepCopy.brunchIndex);
            beforeSlotIndex = deepCopy.beforeSlotIndex;
            onChangeSlotIndex = (Action<int>)deepCopy.onChangeSlotIndex.Clone();
        }
        public void SetSlot(int code, int count)
        {
            itemCode = code;
            itemCount = count;
        }
        public void SwapSlot(int slotIndex, in Slot slot)
        {
            onChangeSlotIndex(slotIndex);
            itemCode = slot.itemCode;
            itemCount = slot.itemCount;
            brunchIndex = slot.brunchIndex;
            beforeSlotIndex = slot.beforeSlotIndex;

        }
        public void AddListener(Action<int> action)
        {
            onChangeSlotIndex = action;
        }
        public void SetBrunchIndex(params int[] slotIndex)
        {
            foreach (var item in slotIndex)
            {
                brunchIndex.Add(item);
            }
        }
    }
    public Slot[] slots { get; private set; }

    [SerializeField] int slotInALIne;
    int[] itemcode2slotindex;
    List<int> emptySlotList;

    delegate bool RefFunc(int num1, int num2, ref Slot slot);
    RefFunc[] checkEmptyFigure = new RefFunc[(int)Figure.Max];
    public override void Init()
    {
        base.Init();

        slots = Array.ConvertAll(new Slot[transform.GetChild(0).childCount], (i) => new Slot());

        itemcode2slotindex = new int[m_itemCounts.Length];
        Array.Fill(itemcode2slotindex, -1);

        emptySlotList = Enumerable.Range(0, m_itemCounts.Length - 1).ToList();

        checkEmptyFigure[(int)Figure.One] = (int slotIndex, int needSlotCount, ref Slot slot)
                                => IsInventoryEmpty(slots[slotIndex]);

        checkEmptyFigure[(int)Figure.Horizon] = (int slotIndex, int needSlotCount, ref Slot slot)
                                => IsSlotEmpty(slotIndex, needSlotCount, IsInOneLine, 1, ref slot);

        checkEmptyFigure[(int)Figure.Vertic] = (int slotIndex, int needSlotCount, ref Slot slot)
                                => IsSlotEmpty(slotIndex, needSlotCount, IsInOneVerticalLine, slotInALIne, ref slot);
    }
    #region 기본 개수 증가
    public override void ItemCountChange(int itemCode, int addNum)
    {
        Item item = InventoryManager.i.info.items[itemCode];

        int codeIndex = itemcode2slotindex[itemCode];

        if (codeIndex != -1)
        {
            Slot slot = slots[codeIndex];
            int count = Mathf.Clamp(addNum + slot.itemCount, 0, item.MaxCount);
            addNum += slot.itemCount - count;
            ChangeCount(itemCode, addNum);
            UseAll(codeIndex, count <= 0);
        }

        if (addNum == 0)
            return;

        //addNum이 양수이면 새로운 index를 받고 slots[새로운 Index]의 beforeSlotIndex = codeIndex;
        //addNum이 음수이면 slots[slots[codeIndex].beforeSlotIndex]를 가져옴.

        if (addNum > 0)
        {
            IncreaseItemCount(item, codeIndex, addNum);
        }
        else
        {
            DecreaseItemCount(itemCode, addNum);
        }
        eventAlert(itemcode2slotindex[itemCode]);
        m_itemCounts[itemCode] += addNum;
    }
    void UseAll(int codeIndex, bool lessEqualZero)
    {
        if (!lessEqualZero)
            return;

        int newIndex = emptySlotList.BinarySearch(codeIndex);
        emptySlotList.Insert(newIndex, codeIndex);
    }
    void IncreaseItemCount(in Item item, int beforeIndex, int addNum)
    {
        if (item.needSlots > emptySlotList.Count)
        {
            //팝업.
            //인벤토리가 가득 찼습니다.
            Debug.Log("인벤토리 꽉 참");
        }
        else
        {
            AllocateSlot2(item, out int emptyIndex);
            slots[emptyIndex].SetSlot(item.itemCode, addNum);
            slots[emptyIndex].beforeSlotIndex = beforeIndex;
            itemcode2slotindex[item.itemCode] = emptyIndex;

            if (beforeIndex >= 0)
                slots[beforeIndex].AddListener((newIndex) => slots[emptyIndex].beforeSlotIndex = newIndex);
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
    bool AllocateSlot2(in Item item, out int emptyIndex)
    {
        int length = emptySlotList.Count;
        Slot slot = new Slot();

        for (int i = 0; i < length; i++)
        {
            if (GetEmptySlot(item, ref slot, out emptyIndex))
            {
                return true;
            }
        }
        emptyIndex = -1;
        return false;
    }
    bool GetEmptySlot(in Item item, ref Slot slot, out int emptyIndex)
    {
        emptyIndex = -1;

        int length = emptySlotList.Count;
        for (int i = 0; i < length; i++)
        {
            if (checkEmptyFigure[(int)item.figure](emptySlotList[i], item.needSlots, ref slot))
            {
                emptyIndex = emptySlotList[i];
                emptySlotList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    void ChangeCount(int itemCode, int itemCount)
    {
        int codeIndex = itemcode2slotindex[itemCode];
        slots[codeIndex].SetSlot(itemCode, itemCount);
        eventAlert(codeIndex);
    }
    void ChangeCount(int itemCount, in Item item)
    {
        slots[itemcode2slotindex[item.itemCode]].SetSlot(item.itemCode, itemCount);

    }
    void RefindSlot(int itemCode)
    {
        emptySlotList.Add(itemcode2slotindex[itemCode]);
        emptySlotList = emptySlotList.OrderBy((i) => i).ToList();
        int newSlotIndex = GetBeforeSlot(itemCode);
        itemcode2slotindex[itemCode] = newSlotIndex;
    }
    int GetBeforeSlot(int itemCode)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemCode == itemCode)
                return i;
        }
        throw new NotImplementedException("아이템 못 찾음");
    }
    #endregion
    #region Slot empty 확인
    bool IsInventoryEmpty(in Slot slot)
    {
        return slot.itemCode == 0;
    }
    bool IsInOneLine(int slotIndex, int needSlotCount)
    {
        return (slotIndex % slotInALIne) + needSlotCount > slotInALIne;
    }
    bool IsSlotEmpty(int slotIndex, int needSlotCount, Func<int, int, bool> lineCheck, int step, ref Slot slot)
    {
        if (!lineCheck(slotIndex, needSlotCount))
            return false;

        int forCount = needSlotCount * step;
        for (int i = step; i < needSlotCount; i += step)
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
    #endregion

    #region 외부 이벤트

    public void SwapSlot(int slot1Index, int slot2Index)
    {
        Slot tempSlot = new Slot(slots[slot1Index]);
        slots[slot1Index].SwapSlot(slot2Index, slots[slot2Index]);
        slots[slot2Index].SwapSlot(slot1Index, tempSlot);

        eventAlert(slot1Index);
        eventAlert(slot2Index);
    }
    #endregion


}
