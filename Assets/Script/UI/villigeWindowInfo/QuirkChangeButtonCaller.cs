using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuirkChangeButtonCaller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    QuirkButtonController.QuirkButtonType buttonType;
    QuirkButtonController quirkButtonController;
    int siblingIndex;
    SetBuildingMat setBuildingMat;
    OpenWithMousePosition openWithMousePosition;
    QuirkRemem quirkRemem;
    public RectTransform rectTransform { get; private set; }

    public void Init(QuirkButtonController.QuirkButtonType type, QuirkButtonController controller, SetBuildingMat setMat)
    {
        rectTransform = GetComponent<RectTransform>();
        quirkRemem = GetComponent<QuirkRemem>();
        setBuildingMat = setMat;
        openWithMousePosition = setBuildingMat.GetComponent<OpenWithMousePosition>();

        buttonType = type;
        quirkButtonController = controller;
        siblingIndex = transform.GetSiblingIndex();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //QuirkButtonController 호출
        quirkButtonController.ButtonCall(buttonType, siblingIndex);
        //업그레이드 비용 UI 호출.
        setBuildingMat.GetData((int)buttonType, SetBuildingMat.MaterialsType.MedecineNeed);
        openWithMousePosition.OpenOnBoxes(quirkRemem.QuirkInfo.rectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //업그레이드 비용 UI 종료.
        openWithMousePosition.Close();
    }
}
