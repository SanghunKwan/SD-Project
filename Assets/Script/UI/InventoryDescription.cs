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
    int trigger;

    public override void Init()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        trigger = Animator.StringToHash("fadeOut");
    }
    public void SetActive(bool onoff)
    {
        if (onoff)
        {
            gameObject.SetActive(true);
        }
        else
        {
            animator.SetTrigger(trigger);
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
