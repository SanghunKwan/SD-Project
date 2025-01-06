using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SettleItems : SettleController
{
    [SerializeField] ItemLines[] itemLines;
    public override void PlaySettle(Action onPlayEnd)
    {
    }
    async void ForRepeat(Action action)
    {
        await Task.Delay(500);
        action();
    }


    [Serializable]
    public class ItemLines
    {
        public SettleItemSlots[] itemSlots;

        public void SelectSlots(int count)
        {
            for (int i = 0; i < count; i++)
            {
                itemSlots[i].gameObject.SetActive(true);
            }
        }
        public async void ActivateSlots(int count)
        {
            for (int i = 0; i < count; i++)
            {
                itemSlots[i].SetSlotAnimationActivate();
                await Task.Delay(250);
            }
        }
    }
}
