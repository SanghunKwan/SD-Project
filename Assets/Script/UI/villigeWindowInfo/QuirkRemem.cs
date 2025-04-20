using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System;

public class QuirkRemem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI textMeshProUGUI;
    QuirkData.Quirk quirk;
    [SerializeField] QuirkInfo quirkInfo;
    public QuirkInfo QuirkInfo => quirkInfo;
    QuirkChange.quirkType quirkType;
    Action<int>[] callQuirkData = new Action<int>[2];

    private void Init()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        callQuirkData[0] = (num) => { quirk = QuirkData.manager.quirkInfo.quirks[num]; };
        callQuirkData[1] = (num) => { quirk = QuirkData.manager.diseaseInfo.quirks[num]; };
    }
    public void RegistQuirkType(QuirkChange.quirkType type)
    {
        Init();
        quirkType = type;
    }
    public void QuirkRemember(int quirkIndex)
    {
        callQuirkData[(int)quirkType](quirkIndex);

        textMeshProUGUI.text = quirk.name;
        textMeshProUGUI.raycastTarget = quirk.index != 0;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        quirkInfo.gameObject.SetActive(true);
        quirkInfo.transform.parent.GetChild(1).gameObject.SetActive(true);
        quirkInfo.Print(quirk.index, quirkType);
        quirkInfo.transform.parent.position = eventData.position;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        quirkInfo.gameObject.SetActive(false);
        quirkInfo.transform.parent.GetChild(1).gameObject.SetActive(false);
    }

}
