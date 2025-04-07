using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionPage : InitObject
{
    public RectTransform rectTransform { get; private set; }
    RectTransform parentTransform;
    [SerializeField] float animTime;
    HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] Animator lightAnimator;
    IEnumerator animEnum;
    Action[] action = new Action[2];
    int[] triggers = new int[3];

    public override void Init()
    {
        horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        parentTransform = transform.parent.parent.GetComponent<RectTransform>();

        triggers[0] = Animator.StringToHash("Normal");
        triggers[1] = Animator.StringToHash("Highlighted");
        triggers[2] = Animator.StringToHash("Selected");

        action[0] = Page0;
        action[1] = Page1;
    }

    public void GotoPage(int page)
    {
        GameManager.manager.onVilligeExpeditionWindow.eventAction?.Invoke(page, Vector3.zero);
        AllocateCourutine(MoveAnimation(GetXAxisValue(page)));
    }


    int GetXAxisValue(int page)
    {
        float padding = -page * (parentTransform.sizeDelta.x + horizontalLayoutGroup.spacing);
        return Mathf.RoundToInt(padding);
    }
    void AllocateCourutine(in IEnumerator enumerator)
    {
        StopMoveAnim();

        animEnum = enumerator;
        StartCoroutine(animEnum);
    }
    public void StopMoveAnim()
    {
        if (animEnum is not null)
            StopCoroutine(animEnum);
    }
    IEnumerator MoveAnimation(int lastPaddingValue)
    {
        float startPadding = rectTransform.anchoredPosition.x;
        float deltaPadding = lastPaddingValue - startPadding;
        float playTime = 0;

        while (playTime <= animTime)
        {
            playTime += Time.deltaTime;
            rectTransform.anchoredPosition
                = Vector2.right * Mathf.RoundToInt(Mathf.Pow(playTime, 0.2f) * deltaPadding + startPadding);
            yield return null;
        }

    }
    public void LightAnimation(int page)
    {
        action[page]();
    }
    void Page0()
    {
        lightAnimator.SetTrigger(triggers[0]);
        lightAnimator.SetTrigger(triggers[2]);
        lightAnimator.ResetTrigger(triggers[1]);
    }
    void Page1()
    {
        lightAnimator.SetTrigger(triggers[1]);
        lightAnimator.ResetTrigger(triggers[0]);
        lightAnimator.ResetTrigger(triggers[2]);
    }
}
