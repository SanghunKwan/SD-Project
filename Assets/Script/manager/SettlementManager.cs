using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SettlementManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Color[] titleColor = new Color[2];
    [SerializeField] Animator hideAnimator;
    [SerializeField] SettleFloorController settleFloorController;

    public enum BattleResult
    {
        Success,
        Retreat,
        Failed
    }
    private void Start()
    {
        SetBlackFade(true);
        DelayAction(2000, StartSettlementAnimation);

    }

    public void SetBattleResult(BattleResult result)
    {
        VertexGradient vertexGradient = titleText.colorGradient;

        if (result == BattleResult.Success)
        {
            titleText.text = "�¸�!";
            titleText.color = titleColor[0];
            vertexGradient.topLeft = titleColor[0];
            vertexGradient.topRight = titleColor[0];
            titleText.colorGradient = vertexGradient;
        }
        else
        {
            titleText.text = "�й�";
            titleText.color = titleColor[1];
            vertexGradient.bottomLeft = titleColor[1];
            vertexGradient.bottomRight = titleColor[1];
            titleText.colorGradient = vertexGradient;
        }

    }
    public void SetBlackFade(bool onoff)
    {
        hideAnimator.gameObject.SetActive(true);
        if (onoff)
            hideAnimator.SetTrigger("fadeOutNoDelay");

        else
            hideAnimator.SetTrigger("StopFadeOut");
    }

    async void DelayAction(int miliSec, Action action)
    {
        await Task.Delay(miliSec);
        action();
    }
    void StartSettlementAnimation()
    {
        hideAnimator.gameObject.SetActive(false);
        settleFloorController.PlaySettleFloors(() => { });
    }
}
