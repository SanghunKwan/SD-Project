using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unit;

public class WindowStatus
{
    public CObject targetObject { get; private set; }

    TextMeshProUGUI[] textMeshProUGUIs = new TextMeshProUGUI[11];


    public void GetStatus(CObject obj)
    {
        targetObject = obj;
    }

    public void Init(Transform tr, int firstLast, int midOffset)
    {
        for (int i = 0; i < firstLast; i++)
        {
            textMeshProUGUIs[i] = tr.GetChild(i).GetComponent<TextMeshProUGUI>();
        }

        for (int i = firstLast; i < textMeshProUGUIs.Length; i++)
        {
            textMeshProUGUIs[i] = tr.GetChild(i + midOffset).GetComponent<TextMeshProUGUI>();
        }

    }

    public void AlloStatus()
    {
        AlloStatus(targetObject.curstat, targetObject.stat);
    }
    public void AlloStatus(unit_status curStat, unit_status dataStat)
    {
        SetTextGUI(textMeshProUGUIs[0], curStat.HP, dataStat.HP);
        SetTextGUI(textMeshProUGUIs[1], curStat.ATK, dataStat.ATK);
        SetTextGUI(textMeshProUGUIs[2], curStat.Accuracy, dataStat.Accuracy);
        SetTextGUI(textMeshProUGUIs[3], curStat.Range, dataStat.Range);
        SetTextGUI(textMeshProUGUIs[4], curStat.AtkSpeed, dataStat.AtkSpeed);
        SetTextGUI(textMeshProUGUIs[5], curStat.Stress, dataStat.Stress);
        SetTextGUI(textMeshProUGUIs[6], curStat.DEF, dataStat.DEF);
        SetTextGUI(textMeshProUGUIs[7], curStat.DOG, dataStat.DOG);
        SetTextGUI(textMeshProUGUIs[8], curStat.SPEED, dataStat.SPEED);
        SetTextGUI(textMeshProUGUIs[9], curStat.ViewAngle, dataStat.ViewAngle);
        SetTextGUI(textMeshProUGUIs[10], curStat.ViewRange, dataStat.ViewRange);
    }
    public void SetTextGUI<T>(TextMeshProUGUI textGUI, T statNum, T dataNum) where T : System.IComparable
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        bool isColorChanged = false;

        if (statNum.CompareTo(dataNum) > 0)
        {
            isColorChanged = true;
            builder.Append("<color=#FFE99BFF>");
        }
        else if (statNum.CompareTo(dataNum) < 0)
        {
            isColorChanged = true;
            builder.Append("<color=#CA3433FF>");
        }
        builder.Append(statNum.ToString());

        if (isColorChanged)
            builder.Append("</color>");

        textGUI.text = builder.ToString();
    }
}
