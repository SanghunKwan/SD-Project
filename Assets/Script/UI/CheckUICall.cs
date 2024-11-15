using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckUICall : MonoBehaviour
{
    [SerializeField] CheckUI UI;
    CheckUI m_Ui;
    [SerializeField] string toUserQuestion;
    [SerializeField] CheckUI.ToUserAnswers[] toUserAnswers;
    [SerializeField] ButtonTypeChange[] buttonTypeChanges;
    [SerializeField] Transform transformUI;
    [SerializeField] ButtonEvent[] UIEvents;
    [SerializeField] CheckUI.COMPONENTINDEX[] hiddenButton;


    [Serializable]
    public class ButtonTypeChange
    {
        public CheckUI.COMPONENTINDEX buttonIndex;
        public CheckUI.BUTTONTYPE buttonType;
    }
    [Serializable]
    public class ButtonEvent
    {
        public CheckUI.COMPONENTINDEX buttonIndex;
        public UnityEvent UiEvent;
    }
    public void CallUI()
    {
        m_Ui = Instantiate(UI, transformUI);
    }
    public void SetUIButton()
    {
        m_Ui.SetUI(toUserQuestion, toUserAnswers);

        foreach (var uievent in UIEvents)
        {
            m_Ui.AddButtonFunction(uievent.buttonIndex, uievent.UiEvent.Invoke);
        }
    }
    public void CallCheckUIRemove()
    {
        m_Ui.ObjectRemove();
    }
    public void ChangeButtonType()
    {
        foreach (var buttonChange in buttonTypeChanges)
        {
            m_Ui.ChangeButton(buttonChange.buttonIndex, buttonChange.buttonType);
        }
    }

    public void CallUIOnce(bool needMove)
    {
        CallUI();
        SetUIButton();
        ChangeButtonType();
        HideButton(needMove);
    }
    void HideButton(bool needMove)
    {
        if (needMove)
            HideButtonWithMove();
        else
            HideButtonWithNoMove();
    }
    public void HideButtonWithMove()
    {
        foreach (var item in hiddenButton)
        {
            m_Ui.SetButtonInactive(item);
        }
    }
    public void HideButtonWithNoMove()
    {
        foreach (var item in hiddenButton)
        {
            m_Ui.SetButtonInactiveNoPositionChange(item);
        }
    }

    #region CheckUiCallChange 관련 기능
    public void DataChange(CheckUICallChange.ChangeData data)
    {
        toUserQuestion = data.toUserQuestion;
        toUserAnswers = data.toUserAnswers;
        buttonTypeChanges = data.buttonTypes;
        hiddenButton = data.hiddenButton;
        UIEvents = data.buttonEvents;
    }
    #endregion
}
