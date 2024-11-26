using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CheckUICall))]
public class CheckUICallChange : MonoBehaviour
{
    [SerializeField] CheckUICall uiCall;
    [SerializeField] List<ChangeData> m_list;
    public CheckUICall CheckUICall { get { return uiCall; } }
    public List<ChangeData> DataList { get { return m_list; } }
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
        uiCall.DataChange(m_list[nowIndex]);
    }
    public void PopUp(bool needMove)
    {
        uiCall.CallUIOnce(needMove);
    }
    public void SetandCallOnce(int index, bool needMove = false)
    {
        nowIndex = index;
        SetUICall();
        PopUp(needMove);
    }
}
