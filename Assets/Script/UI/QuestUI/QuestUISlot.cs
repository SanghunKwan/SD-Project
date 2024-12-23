using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class QuestUISlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI detail;
    [SerializeField] Animator animator;
    int triggerOffNum;
    int triggerHighLightNum;

    public Action hideAction { get; set; }

    private void Awake()
    {
        triggerOffNum = Animator.StringToHash("fadeOut");
        triggerHighLightNum = Animator.StringToHash("highLight");
    }
    public void SetQuestText(in string dataTitle, in string dataDetail)
    {
        title.text = dataTitle;
        detail.text = dataDetail;
    }
    public void HideQuest()
    {
        animator.SetTrigger(triggerOffNum);
        hideAction();
        hideAction = null;
    }
    public void OnFadeOutEnd()
    {
        gameObject.SetActive(false);
        transform.SetAsLastSibling();
    }
    public void TimeStopHighLight()
    {
        animator.SetTrigger(triggerHighLightNum);
    }
}
