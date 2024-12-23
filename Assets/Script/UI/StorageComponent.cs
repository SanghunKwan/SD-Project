using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageComponent : InitObject
{
    protected Action<int> eventAlert = (num) => { };

    [Serializable]
    public struct Item
    {
        public string name;
        public int itemCode;
        public int MaxCount;
        public ItemType type;
        public string description;
        public int HP;
        public int needSlots;
        public InventoryStorage.Figure figure;
        public SkillData.ItemSkillEffect itemSkillEffect;
    }

    protected int[] m_itemCounts;
    public int[] ItemCounts { get { return m_itemCounts; } }

    public override void Init()
    {
        m_itemCounts = new int[InventoryManager.i.info.items.Length];
    }
    public virtual void ItemCountChange(int itemCode, int addNum)
    {
        m_itemCounts[itemCode] += addNum;
        eventAlert(itemCode);
    }
    public void AddListener(Action<int> callWhenCountChanged)
    {
        eventAlert += callWhenCountChanged;
    }
    public void SubtractListener(Action<int> callWhenCountChanged)
    {
        eventAlert -= callWhenCountChanged;
    }
    #region temp
    public void SetItems()
    {
        for (int i = 0; i <= 13; i++)
        {
            if (i == 8)
            {
                ItemCountChange(i, 0);
                continue;
            }
            else if (i == 13)
            {
                ItemCountChange(i, 20000);
                continue;
            }
            //m_itemCounts[i] = i;
            ItemCountChange(i, i);
        }
    }
    #endregion
}
