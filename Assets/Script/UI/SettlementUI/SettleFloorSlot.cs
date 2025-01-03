using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SettleFloorSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI floorNumberText;
    [SerializeField] TextMeshProUGUI floorResultText;


    public void SetContents(int floorNum, in Color floorTextColor, in string result)
    {
        floorNumberText.text = floorNum.ToString() + "<color=#E5E5E5>Ãþ";

    }
}
