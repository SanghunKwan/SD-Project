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
    [SerializeField] SettleController[] settleControllers;
    [SerializeField] GameObject buttonCallNext;


    [Header("slotPlaySpeed")]
    [SerializeField] int startDelay = 2000;
    public int interval = 250;


    public enum BattleResult
    {
        Success,
        Retreat,
        Failed
    }
    private void Start()
    {
        SetBlackFade(true);
        DelayAction(startDelay, StartSettlementAnimation);

    }

    public void SetBattleResult(BattleResult result)
    {
        VertexGradient vertexGradient = titleText.colorGradient;

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
        PlaySettlementAnimation(0);
    }
    void PlaySettlementAnimation(int settleIndex)
    {
        Action onEndAction = () => PlaySettlementAnimation(settleIndex + 1);
        int length = settleControllers.Length;


        if (settleIndex + 1 >= length)
            onEndAction = CallButton;

        settleControllers[settleIndex].PlaySettle(onEndAction, interval);
    }
    void CallButton()
    {
        buttonCallNext.gameObject.SetActive(true);
    }

    #region SecondSettle
    public void CallNextSettlement()
    {

    }
    #endregion
}
