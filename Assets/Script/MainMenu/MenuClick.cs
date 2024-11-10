using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuClick : PlayBase
{
    Animator animTitle;
    Animator animButton;

    [SerializeField] TriggerName triggerName;
    int[] animHashNum;
    public Action buttonEvent { get; set; }
    [SerializeField] float beforeButtonTime;
    [SerializeField] NeedAddressable needAddressable;

    enum TriggerName
    {
        buttonSizeTwinkle,
        textSizeTwinkle
    }
    public override void Init()
    {

        animTitle = transform.GetChild(0).GetComponent<Animator>();
        animButton = transform.GetChild(1).GetComponent<Animator>();

        animHashNum = new int[2]
        {
            Animator.StringToHash(triggerName.ToString()),
            Animator.StringToHash("fadeOut")
        };

    }
    public void ShowTitle()
    {
        gameObject.SetActive(true);
        StartCoroutine(WaitingforSeconds(beforeButtonTime, ShowButton));
        needAddressable.Init();
    }
    void ShowButton()
    {
        animButton.gameObject.SetActive(true);
        animButton.SetTrigger(animHashNum[0]);
    }

    public void OnStartButtonClick()
    {
        animButton.SetTrigger(animHashNum[1]);
        animTitle.SetTrigger(animHashNum[1]);

        StartCoroutine(WaitingforSeconds(2, buttonEvent));
    }
}
