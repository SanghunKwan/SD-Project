using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SettleFloorController : SettleController
{
    [SerializeField] SettleFloorSlot[] settleFloorSlots;

    [SerializeField] Color[] resultColors;

    [SerializeField] string[] resultString = new string[2] { "Ŭ����!", "�й�" };

    public override void PlaySettle(Action onPlayEnd)
    {


        ForRepeat(onPlayEnd);
    }
    async void ForRepeat(Action action)
    {
        SaveData.StageData stageData = GameManager.manager.battleClearManager.SaveDataInfo.stageData;
        int[] floorsData = stageData.floors;
        int length = floorsData.Length;

        int clearIndex = stageData.nowFloorIndex - Convert.ToInt32(!stageData.isClear);

        int index;

        for (int i = 0; i < length; i++)
            settleFloorSlots[i].gameObject.SetActive(true);

        for (int i = 0; i < length; i++)
        {
            //int�� 0�϶� Ŭ����
            //���� bool ���� false�� Ŭ����.
            index = Convert.ToInt32(i > clearIndex);
            settleFloorSlots[i].SetContents(floorsData[i], resultColors[index], resultString[index]);
            settleFloorSlots[i].SlotAnimationActivate();
            await Task.Delay(250);
        }
        action();
    }
}
