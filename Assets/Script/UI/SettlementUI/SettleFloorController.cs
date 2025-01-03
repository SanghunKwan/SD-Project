using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SettleFloorController : MonoBehaviour
{
    [SerializeField] SettleFloorSlot[] settleFloorSlots;
    public void PlaySettleFloors(Action onPlayEnd)
    {
        int length = settleFloorSlots.Length;
        ForRepeat(length);
    }
    async void ForRepeat(int length)
    {
        for (int i = 0; i < length; i++)
        {
            settleFloorSlots[i].SetContents(1, new Color(0,0,0), "asdf");
            await Task.Delay(250);
        }
    }
}
