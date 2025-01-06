using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettleItemSlots : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Image image;
    [SerializeField] Text text;

    public void SetData(in Sprite itemImage, int itemCount)
    {
        image.sprite = itemImage;
        text.text = "x" + itemCount.ToString();
    }
    public void SetSlotAnimationActivate()
    {
        animator.enabled = true;
    }
}
