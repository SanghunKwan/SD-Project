using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUISlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI detail;
    [SerializeField] Animator animator;
    int triggerNum;

    private void Awake()
    {
        triggerNum = Animator.StringToHash("fadeOut");
    }
    public void SetQuestText(in string dataTitle, in string dataDetail)
    {
        title.text = dataTitle;
        detail.text = dataDetail;
    }
    public void HideQuest()
    {
        animator.SetTrigger(triggerNum);
    }
    public void OnFadeOutEnd()
    {
        gameObject.SetActive(false);
        transform.SetAsLastSibling();
    }
}
