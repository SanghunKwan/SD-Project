using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SettleFloorController : SettleController
{
    [SerializeField] SettleFloorSlot[] settleFloorSlots;

    [SerializeField] Color[] resultColors;

    [SerializeField] string[] resultString = new string[2] { "클리어!", "패배" };
   

    public override void PlaySettle(Action onPlayEnd, int interval)
    {
        m_interval = interval;

        ForRepeat(onPlayEnd);
    }
    async void ForRepeat(Action action)
    {
        int[] floorsData = stageData.floors;
        int length = floorsData.Length;

        int clearIndex = stageData.nowFloorIndex - Convert.ToInt32(!stageData.isClear);

        int index;

        for (int i = 0; i < length; i++)
            settleFloorSlots[i].gameObject.SetActive(true);

        for (int i = 0; i < length; i++)
        {
            //int가 0일때 클리어
            //따라서 bool 값은 false가 클리어.
            index = Convert.ToInt32(i > clearIndex);
            settleFloorSlots[i].SetContents(floorsData[i], resultColors[index], resultString[index]);
            settleFloorSlots[i].SlotAnimationActivate();
            await Task.Delay(m_interval);
        }
        action();
    }
}
