using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckUICall))]
public class CheckUICallChange : MonoBehaviour
{
    [SerializeField] CheckUICall uiCall;
    [SerializeField] List<ChangeData> list;
    public int nowIndex { get; set; }



    [Serializable]
    public class ChangeData
    {
        public string toUserQuestion;
        public CheckUI.ToUserAnswers[] toUserAnswers;
        public CheckUICall.ButtonEvent[] buttonEvents;
        public CheckUICall.ButtonTypeChange[] buttonTypes;
        public CheckUI.COMPONENTINDEX[] hiddenButton;

    }

    public void SetUICall()
    {
        uiCall.DataChange(list[nowIndex]);
    }
    public void PopUp(bool needMove)
    {
        uiCall.CallUIOnce(needMove);
    }
}
