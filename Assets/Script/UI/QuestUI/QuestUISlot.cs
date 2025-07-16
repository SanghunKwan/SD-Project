using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class QuestUISlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI grade;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI detail;
    [SerializeField] Animator animator;
    [SerializeField] VerticalLayoutGroup layoutGroup;
    [SerializeField] Image image;

    Queue<QuestUISlot> viewerQueue;

    static int triggerOffNum = Animator.StringToHash("fadeOut");
    static int triggerHighLightNum = Animator.StringToHash("highLight");
    int animStateNameHash;

    public Action hideAction { get; set; }

    IEnumerator enumerator;

    public void Call(in Color gradeColor, in Color gradientColorUp, in Color gradientColorDown)
    {
        gameObject.SetActive(true);
        animator.Rebind();
        grade.color = gradeColor;
        grade.colorGradient = new VertexGradient(gradientColorUp, Color.clear,
                                                 gradientColorDown, Color.clear);
    }

    public void InitQuestText(QuestManager.QuestGrade gradeData, in string dataTitle, in string dataDetail)
    {
        InitQuestText(gradeData.ToString(), dataTitle, dataDetail);
    }
    public void InitQuestText(in string gradeData, in string dataTitle, in string dataDetail)
    {
        grade.text = gradeData;
        SetQuestText(dataTitle, dataDetail);
    }
    public void SetQuestText(in string dataTitle, in string dataDetail)
    {
        title.text = dataTitle;
        detail.text = dataDetail;
    }
    public void GetQuestText(out string dataTitle, out string dataDetail)
    {
        dataTitle = title.text;
        dataDetail = detail.text;
    }
    public void AddQuestText(in string dataTitle, in string dataDetail)
    {
        title.text += dataTitle;
        detail.text += dataDetail;
    }
    public void HideQuest(Queue<QuestUISlot> queue)
    {
        animator.SetTrigger(triggerOffNum);
        hideAction();
        hideAction = null;

        viewerQueue = queue;
    }
    public void OnFadeOutStart()
    {
        PreserveSize(false);
    }
    public void OnFadeOutEnd()
    {
        gameObject.SetActive(false);
        transform.SetAsLastSibling();
        image.enabled = true;

        viewerQueue.Enqueue(this);

        viewerQueue = null;
    }
    public void TimeStopHighLight()
    {
        animator.SetTrigger(triggerHighLightNum);
    }
    private void OnEnable()
    {
        PreserveSize(true);
    }

    public void PreserveSize(bool fadeinIsTrue)
    {
        enumerator = SizeSetCoroutine(fadeinIsTrue);
        StartCoroutine(enumerator);
    }
    IEnumerator SizeSetCoroutine(bool fadeinIsTrue)
    {
        layoutGroup.childControlHeight = false;
        if (!fadeinIsTrue) layoutGroup.childForceExpandHeight = false;
        IsStatePlaying(out AnimatorStateInfo info);
        animStateNameHash = info.shortNameHash;
        float calculatedTime;

        while (IsStatePlaying(out info) && info.normalizedTime <= 1)
        {
            calculatedTime = info.normalizedTime;
            if (!fadeinIsTrue)
                calculatedTime = 1 - info.normalizedTime;
            SlotSetSize(calculatedTime);
            animStateNameHash = info.shortNameHash;
            yield return null;
        }
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandHeight = true;
        enumerator = null;
    }
    bool IsStatePlaying(out AnimatorStateInfo info)
    {
        info = animator.GetCurrentAnimatorStateInfo(0);
        return info.shortNameHash == animStateNameHash;
    }
    void SlotSetSize(float normalizedTime)
    {
        float spacing = normalizedTime * 20f;
        layoutGroup.spacing = spacing;

        layoutGroup.padding.top = Mathf.RoundToInt(2f * spacing);
        layoutGroup.padding.bottom = Mathf.RoundToInt(1.5f * spacing);

        grade.rectTransform.sizeDelta = new Vector2(340, normalizedTime * grade.preferredHeight);
        title.rectTransform.sizeDelta = new Vector2(340, normalizedTime * title.preferredHeight);
        detail.rectTransform.sizeDelta = new Vector2(340, normalizedTime * detail.preferredHeight);
    }
    //onenable로 vertical padding spacing * 1.5f로 유지.
    //onFacdOutEnd로 동일하게 유지.

}
