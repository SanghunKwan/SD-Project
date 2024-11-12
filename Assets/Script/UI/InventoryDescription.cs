using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnimationFadeOutDisable))]
public class InventoryDescription : InitObject
{
    Image image;
    TextMeshProUGUI text;
    Animator animator;
    int[] trigger;

    public override void Init()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        trigger = new int[2];
        trigger[0] = Animator.StringToHash("fadeOut");
        trigger[1] = Animator.StringToHash("StopFadeOut");


    }
    public void SetActive(bool onoff)
    {
        if (onoff)
        {
            gameObject.SetActive(true);
            animator.SetTrigger(trigger[1]);
            animator.ResetTrigger(trigger[0]);
        }
        else
        {
            if (!gameObject.activeSelf)
                return;

            animator.ResetTrigger(trigger[1]);
            animator.SetTrigger(trigger[0]);
        }
    }
    public void SetText(in string str)
    {
        text.text = str;
    }
    public void SetPosition(Vector2 vector)
    {
        image.rectTransform.position = vector;
    }
}
