using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettleFloorSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI floorNumberText;
    [SerializeField] TextMeshProUGUI floorResultText;
    [SerializeField] Animator animator;


    public void SetContents(int floorNum, in Color floorTextColor, in string result)
    {
        floorNumberText.text = floorNum.ToString() + "<color=#E5E5E5>Ãþ";
        floorResultText.text = result;
        floorResultText.color = floorTextColor;
    }
    public void SlotAnimationActivate()
    {
        animator.enabled = true;
    }
}
