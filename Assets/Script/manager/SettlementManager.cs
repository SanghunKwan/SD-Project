using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SettlementManager : SettleCanSkip
{
    [SerializeField] Animator hideAnimator;
    [SerializeField] SettleController[] settleControllers;
    [SerializeField] SettleTitle settleTitle;
    [SerializeField] SettlementCharacterParent settleCharacterParent;
    [SerializeField] GameObject buttonCallNext;

    [SerializeField] GameObject[] firstSettles;
    [SerializeField] GameObject[] secondSettles;


    [Header("slotPlaySpeed")]
    [SerializeField] int startDelay = 2000;


    public enum BattleResult
    {
        Failed,
        Success,
        Retreat,
    }
    private void Start()
    {
        SetBlackFade(true);

        SaveStageClearData(out bool stageSucess);

        DelayAction(startDelay / 2, () => StartSettleTitleAnimation(stageSucess));
        DelayAction(startDelay, StartSettlementAnimation);

        settleCharacterParent.Init();
    }
    public void SetBlackFade(bool onoff)
    {
        hideAnimator.gameObject.SetActive(true);
        if (onoff)
            hideAnimator.SetTrigger("fadeOutNoDelay");

        else
            hideAnimator.SetTrigger("StopFadeOut");
    }
    void SaveStageClearData(out bool stageSucess)
    {
        SaveData.StageData stageData = GameManager.manager.battleClearManager.SaveDataInfo.stageData;

        stageSucess = stageData.isClear && (stageData.floors.Length - stageData.nowFloorIndex) == 1;
        SaveData.StageData tempData = new();
        tempData.CloneValue(stageData);
        foreach (var item in settleControllers)
        {
            item.Init(tempData);
        }

    }

    async void DelayAction(int miliSec, Action action)
    {
        await Task.Delay(miliSec);
        action();
    }
    void StartSettleTitleAnimation(bool isSucess)
    {

        settleTitle.SetTitle(isSucess);
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
        int secondLength = secondSettles.Length;
        int firstLength = firstSettles.Length;
        for (int i = 0; i < secondLength; i++)
        {
            secondSettles[i].SetActive(true);
        }
        for (int i = 0; i < firstLength; i++)
        {
            firstSettles[i].SetActive(false);
        }
    }
    #endregion
}
