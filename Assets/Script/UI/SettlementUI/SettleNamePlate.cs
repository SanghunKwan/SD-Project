using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettleNamePlate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI teamText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI nameText;


    public void SetTexts(in string teamString, int level, in string nameString)
    {
        teamText.text = "<mark=#00000055>" + teamString + "</mark>";
        levelText.text = level.ToString();
        nameText.text = nameString;
    }

}
