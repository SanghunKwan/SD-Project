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
    public class Slot
    {
        public int itemCode { get; private set; }
        public int itemCount { get; private set; }
        public List<int> brunchIndex { get; set; }
        public int beforeSlotIndex { get; set; } = -1;
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
            onChangeSlotIndex = (Action<int>)deepCopy.onChangeSlotIndex.Clone();
        }
        public void SetSlot(int code, int count)
        {
            itemCode = code;
            itemCount = count;
        }
        public void SwapSlot(int targetSlotIndex, in Slot targetSlot)
        {
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
    public Slot[] slots { get; private set; }

    [SerializeField] int slotInALIne;
    int[] itemcode2slotindex;
    int[] figure2step;
    List<int> emptySlotList;

    delegate bool RefFunc(int num1, int num2, ref Slot slot);
    RefFunc[] checkEmptyFigure = new RefFunc[(int)Figure.Max];
    Action<CUnit, Item, float>[] itemAffectAction = new Action<CUnit, Item, float>[(int)SkillData.ItemSkillEffect.MAX];

    CheckUICallChange uiCallChange;



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

    #region 기본 개수 증가
    public override void ItemCountChange(int itemCode, int addNum)
    {
        Item item = InventoryManager.i.info.items[itemCode];

        int codeIndex = itemcode2slotindex[itemCode];

        if (codeIndex != -1)
            SlotOperateWithCodeIndex(codeIndex, ref addNum, item);

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
            DecreaseItemCount(item.itemCode, addNum);
        }
    }

    void SlotOperateWithCodeIndex(int codeIndex, ref int addNum, in Item item)
    {
        Slot slot = slots[codeIndex];
        int count = Mathf.Clamp(addNum + slot.itemCount, 0, item.MaxCount);
        addNum += slot.itemCount - count;
        //count는 최종 개수
        //addNum은 더하는 개수
        ChangeCountBySlot(slot, count, codeIndex);
        CheckUseAll(codeIndex, count <= 0);
    }
    void CheckUseAll(int codeIndex, bool lessEqualZero = true)
    {
        if (!lessEqualZero)
            return;

        slots[codeIndex].SetSlot(0, 0);
        int newIndex = emptySlotList.BinarySearch(codeIndex);
        emptySlotList.Insert(~newIndex, codeIndex);
    }

    void IncreaseItemCount(in Item item, int beforeIndex, int addNum)
    {
        if (AllocateSlot(item, out int emptyIndex))
        {
            int step = figure2step[(int)item.figure];
            itemcode2slotindex[item.itemCode] = emptyIndex;
            ChangeCount(item.itemCode, addNum, item.needSlots, step);

            if (beforeIndex >= 0)
                slots[beforeIndex].AddListener((newIndex) => slots[emptyIndex].beforeSlotIndex = newIndex);
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
        Debug.Log("itemCode : " + itemCode.ToString() + "codeIndex : " + codeIndex);
        Slot tempslot = slots[codeIndex];
        List<int> tempList = tempslot.brunchIndex;

        tempList.Clear();
        m_itemCounts[itemCode] += itemCount - tempslot.itemCount;

        for (int i = 0; i < needSlots; i++)
        {
            temp = codeIndex + i * step;
            Debug.Log("temp : " + temp);
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
        int length = tempArray.Length;
        int originBrunch;
        int offsetBrunch;
        int brunchDifference = (tempArray[0] + offset) / slotInALIne - tempArray[0] / slotInALIne;

        for (int i = 0; i < length; i++)
        {
            originBrunch = tempArray[i] / slotInALIne;
            offsetBrunch = (tempArray[i] + offset) / slotInALIne;

            if (originBrunch + brunchDifference != offsetBrunch)
                return false;
        }
        return true;
    }
    #endregion

    #region 외부 이벤트
    public void SetSlot(in Slot slot, int offset)
    {
        int[] brunchArray = slot.brunchIndex.ToArray();
        int length = brunchArray.Length;
        int tempIndex;
        ReturnCodetoSlot(slot, slot.beforeSlotIndex, offset);
        for (int i = 0; i < length; i++)
        {
            slot.brunchIndex[i] += offset;
            tempIndex = slot.brunchIndex[i];
            slots[tempIndex].SwapSlot(tempIndex, slot);
            emptySlotList.Remove(tempIndex);
        }
    }
    public void SetSlotEmpty(int slotIndex)
    {
        Slot slot = slots[slotIndex];
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

    void ChangeCountBySlot(in Slot slot, int count, int codeIndex)
    {
        Slot tempslot;
        int temp;
        int itemCode = slot.itemCode;
        int itemCount = slot.itemCount - count;
        int needSlots;
        int[] brunchArray = slot.brunchIndex.ToArray();
        needSlots = brunchArray.Length;

        m_itemCounts[itemCode] += itemCount;

        for (int i = 0; i < needSlots; i++)
        {
            temp = brunchArray[i];
            Debug.Log("temp : " + temp.ToString() + "    number : " + count);

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
