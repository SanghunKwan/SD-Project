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
        AlloStatus(targetObject.curstat);
    }
    public void AlloStatus(unit_status stat)
    {
        textMeshProUGUIs[0].text = stat.HP.ToString();
        textMeshProUGUIs[1].text = stat.ATK.ToString();
        textMeshProUGUIs[2].text = stat.Accuracy.ToString();
        textMeshProUGUIs[3].text = stat.Range.ToString();
        textMeshProUGUIs[4].text = stat.AtkSpeed.ToString();
        textMeshProUGUIs[5].text = stat.Stress.ToString();
        textMeshProUGUIs[6].text = stat.DEF.ToString();
        textMeshProUGUIs[7].text = stat.DOG.ToString();
        textMeshProUGUIs[8].text = stat.SPEED.ToString();
        textMeshProUGUIs[9].text = stat.ViewAngle.ToString();
        textMeshProUGUIs[10].text = stat.ViewRange.ToString();
    }
}
