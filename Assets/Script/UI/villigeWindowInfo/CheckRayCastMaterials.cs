using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRayCastMaterials : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] SetBuildingMat setBuildingMat;
    OpenWithMousePosition openWithMousePosition;

    int siblingIndex;
    private void Awake()
    {
        siblingIndex = transform.GetSiblingIndex();
        openWithMousePosition = setBuildingMat.GetComponent<OpenWithMousePosition>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        setBuildingMat.GetData(siblingIndex, SetBuildingMat.MaterialsType.MedecineNeed);
        openWithMousePosition.OpenOnMouse(eventData);
        GameManager.manager.onGetMaterials.eventAction?.Invoke((int)GameManager.GetMaterialsNum.Healing, Vector3.zero);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        openWithMousePosition.Close();
    }
}
