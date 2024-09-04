using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;
using TMPro;

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
        textMeshProUGUIs[0].text = targetObject.stat.HP.ToString();
        textMeshProUGUIs[1].text = targetObject.stat.ATK.ToString();
        textMeshProUGUIs[2].text = targetObject.stat.Accuracy.ToString();
        textMeshProUGUIs[3].text = targetObject.stat.Range.ToString();
        textMeshProUGUIs[4].text = targetObject.stat.AtkSpeed.ToString();
        textMeshProUGUIs[5].text = targetObject.stat.Stress.ToString();
        textMeshProUGUIs[6].text = targetObject.stat.DEF.ToString();
        textMeshProUGUIs[7].text = targetObject.stat.DOG.ToString();
        textMeshProUGUIs[8].text = targetObject.stat.SPEED.ToString();
        textMeshProUGUIs[9].text = targetObject.stat.ViewAngle.ToString();
        textMeshProUGUIs[10].text = targetObject.stat.ViewRange.ToString();
    }
}
