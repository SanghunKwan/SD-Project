using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveStageViewNode : InitObject, IPointerExitHandler
{
    Text text;
    Animator animator;
    Button button;
    int[] hashNum = new int[2];
    public int saveStage { get; set; }
    public bool IsInteractive { get { return button.interactable; } }
    public Action exitAction { get; set; } = () => { };

    public override void Init()
    {
        animator = GetComponent<Animator>();
        text = transform.GetChild(2).GetComponent<Text>();
        button = transform.GetChild(1).GetChild(0).GetComponent<Button>();
        hashNum[0] = Animator.StringToHash("twinkle");
        hashNum[1] = Animator.StringToHash("repeat");
    }

    public void SelectStage(int stageNum)
    {
        LoadStage(stageNum);
        animator.SetTrigger(hashNum[1]);
    }
    public void LoadStage(int stageNum)
    {
        button.interactable = true;
        text.text = stageNum.ToString();
        saveStage = stageNum;
    }
    public void StageReset()
    {
        NodeReset();
        MakeLine(false);
    }
    public void NodeReset()
    {
        button.interactable = false;
        text.text = "";
        saveStage = 0;
        AnimRepeatStop();
    }
    public void MakeLine(bool onoff)
    {
        transform.GetChild(0).gameObject.SetActive(onoff);
    }
    public void AnimRepeatStop()
    {
        animator.ResetTrigger(hashNum[1]);
        animator.SetTrigger(hashNum[0]);
    }
    public void NewLastNode()
    {
        MakeLine(false);
        animator.SetTrigger(hashNum[1]);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exitAction();
        exitAction = () => { };
    }
}
