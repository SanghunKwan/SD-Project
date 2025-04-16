using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositionLoad : MonoBehaviour
{
    void Start()
    {
        WaitUntilManagerRegistered(DelayLoad);
    }
    void WaitUntilManagerRegistered(in System.Action action)
    {
        if (GameManager.manager.battleClearManager != null)
            action();
        else
            GameManager.manager.onBattleClearManagerRegistered += action;
    }
    void DelayLoad()
    {
        PlayInfo info = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;

        transform.position = info.camPosition;
        Debug.Log("Ä· À§Ä¡ ·Îµå");
    }
}
