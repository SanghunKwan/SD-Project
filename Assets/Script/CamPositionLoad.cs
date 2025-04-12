using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositionLoad : MonoBehaviour
{
    void Start()
    {
        GameManager.manager.onBattleClearManagerRegistered += DelayLoad;
    }
    void DelayLoad()
    {
        PlayInfo info = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;
        
        transform.position = info.camPosition;
        Debug.Log("Ä· À§Ä¡ ·Îµå");
    }
}
