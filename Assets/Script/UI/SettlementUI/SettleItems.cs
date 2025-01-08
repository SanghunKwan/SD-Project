using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SettleItems : SettleController
{
    [SerializeField] ItemLines[] itemLines;
    [SerializeField] InventoryStorage inventory;

    int[] type2index;
    int[] linesIndexArray;

    private void Awake()
    {
        type2index = new int[(int)ItemType.Max];

        int length = itemLines.Length;
        for (int i = 0; i < length; i++)
        {
            type2index[(int)itemLines[i].needType] = i;
        }
    }

    public override void PlaySettle(Action onPlayEnd, int interval)
    {
        m_interval = interval;
        GetInventoryItems();
        ForRepeat(onPlayEnd);
    }

    void GetInventoryItems()
    {
        int itemLineLength = itemLines.Length;
        linesIndexArray = new int[itemLineLength];
        Array.Fill(linesIndexArray, 0);

        int[] itemCounts = inventory.ItemCounts;

        int length = itemCounts.Length;

        int needSlots;
        int linesIndex;
        ItemType type;
        StorageComponent.Item[] items = InventoryManager.i.info.items;

        for (int i = 0; i < length; i++)
        {
            if (itemCounts[i] <= 0)
                continue;

            type = items[i].type;
            needSlots = items[i].needSlots;
            linesIndex = type2index[(int)type];
            for (int j = 0; j < needSlots; j++)
            {
                itemLines[linesIndex].SetSlots(linesIndexArray[linesIndex], i, j, itemCounts[i]);

            }
            linesIndexArray[linesIndex]++;

        }

        for (int i = 0; i < itemLineLength; i++)
        {
            itemLines[i].SelectSlots(linesIndexArray[i]);
        }

    }

    async void ForRepeat(Action action)
    {
        int itemLineLength = itemLines.Length;

        for (int i = 0; i < itemLineLength; i++)
        {
            await itemLines[i].ActivateSlots(linesIndexArray[i], m_interval);
        }
        action();
    }


    [Serializable]
    public class ItemLines
    {
        public SettleItemSlots[] itemSlots;
        public ItemType needType;

        public void SetSlots(int index, int itemIndex, int spriteIndex, int itemCount)
        {
            itemSlots[index].SetData(InventoryManager.i.resourceSprite[InventoryManager.i.info.items[itemIndex].name][spriteIndex], itemCount);

        }
        public void SelectSlots(int count)
        {
            for (int i = 0; i < count; i++)
            {
                itemSlots[i].gameObject.SetActive(true);
            }
        }
        public async Task ActivateSlots(int count, int interval)
        {
            for (int i = 0; i < count; i++)
            {
                itemSlots[i].SetSlotAnimationActivate();
                await Task.Delay(interval);
            }
        }
    }
}
