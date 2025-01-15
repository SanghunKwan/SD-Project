using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SettlementManager;

public class SettleTitle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Image image;
    [SerializeField] Color[] titleColor = new Color[2];


    public void SetTitle()
    {
        SaveData.StageData stageData = GameManager.manager.battleClearManager.SaveDataInfo.stageData;

        bool isSucess = stageData.isClear && (stageData.floors.Length - stageData.nowFloorIndex) == 1;

        SetBattleResult((BattleResult)Convert.ToInt32(isSucess));

    }

    void SetBattleResult(BattleResult result)
    {
        VertexGradient vertexGradient = titleText.colorGradient;
        gameObject.SetActive(true);

        if (result == BattleResult.Success)
        {
            titleText.text = "½Â¸®!";
            titleText.color = titleColor[0];
            vertexGradient.topLeft = titleColor[0];
            vertexGradient.topRight = titleColor[0];
            titleText.colorGradient = vertexGradient;
        }
        else
        {
            titleText.text = "ÆÐ¹è";
            titleText.color = titleColor[1];
            vertexGradient.bottomLeft = titleColor[1];
            vertexGradient.bottomRight = titleColor[1];
            titleText.colorGradient = vertexGradient;
            image.color = titleColor[1];
        }

    }
}
