using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildSetCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    BuildingSetWindow buildingSetWindow;

    Image image;

    public bool isDrag { get => buildingSetWindow.isDrag; }
    bool isEnter;
    bool isVilligeInteractExist;

    [SerializeField] Color effectColor;

    private void Start()
    {
        image = GetComponent<Image>();
        buildingSetWindow = transform.parent.parent.GetComponent<BuildingSetWindow>();
        buildingSetWindow.AddDragEnd(DragEnd);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDrag)
        {
            isEnter = true;
            buildingSetWindow.SetHeroInDic(transform.parent.gameObject);
        }
        image.color = effectColor;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDrag)
        {
            buildingSetWindow.SetBackHeroText(transform.parent.gameObject);
        }
        isEnter = false;
        image.color = Color.clear;
    }
    void DragEnd()
    {
        image.color = Color.clear;

        if (isEnter)
            buildingSetWindow.SaveHeroData(transform.parent.gameObject);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isVilligeInteractExist = buildingSetWindow.buildingComponent.IsDataNull(transform.GetSiblingIndex(), out _)
                                 && eventData.button != PointerEventData.InputButton.Left;

        if (!isVilligeInteractExist)
            return;

        buildingSetWindow.buildingComponent.saveVilligeInteract[transform.parent.GetSiblingIndex()].BeginDragOffset(eventData, image);
        OnPointerEnter(eventData);
        buildingSetWindow.buildingComponent.ResetData(transform.parent.GetSiblingIndex());
        buildingSetWindow.UpgradeButtonEvent?.Invoke(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isVilligeInteractExist)
            return;

        villigeInteract.now_villigeInteract.OnDrag(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isVilligeInteractExist)
            return;

        villigeInteract.now_villigeInteract.OnEndDrag(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left
                || buildingSetWindow.buildingComponent.saveVilligeInteract[transform.parent.GetSiblingIndex()] == null)
            return;
        buildingSetWindow.buildingComponent.saveVilligeInteract[transform.parent.GetSiblingIndex()].OnPointerClick(eventData);
    }
}
