using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InventoryInput : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    int SlotNum;
    bool isOnItem;

    bool isDragActive;
    Action<Vector2>[] clicks;

    RectTransform dragImage;
    Vector2 pointerOffset;
    InventoryComponent inventoryComponent;

    static IEnumerator enumWaiting;
    bool IsSlotExist
    {
        get { return inventoryComponent.CheckItemSlotExist(SlotNum); }
    }
    private void Start()
    {
        SetClicks();
        GameObject.FindWithTag("CanvasWorld").transform.GetChild(0).GetComponent<ClickDrag>().pointerDown = () => inventoryComponent.ActiveDescription(false);
        inventoryComponent = transform.parent.parent.GetComponent<InventoryComponent>();
        SlotNum = transform.GetSiblingIndex();
    }
    void SetClicks()
    {
        clicks = new Action<Vector2>[3];

        clicks[(int)PointerEventData.InputButton.Left] = (pos) =>
        {
            StopWaiting();
            inventoryComponent.SetDescription(SlotNum, pos);
        };
        clicks[(int)PointerEventData.InputButton.Right] = (pos) =>
        {
            StopWaiting();
            inventoryComponent.Use(SlotNum);
        };
        clicks[(int)PointerEventData.InputButton.Middle] = (pos) => { };
    }
    #region input ¿Ã∫•∆Æ
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsSlotExist || !PlayerInputManager.manager.inventoryInputEnable[(int)eventData.button]) return;

        isDragActive = true;
        dragImage = inventoryComponent.GetDragImage(SlotNum);
        pointerOffset = (Vector2)dragImage.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragActive)
            dragImage.position = eventData.position + pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragActive)
            return;

        Destroy(dragImage.gameObject);
        isDragActive = false;

        inventoryComponent.DragEnd(SlotNum, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsSlotExist || isDragActive)
            return;

        clicks[(int)eventData.button](eventData.position);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isOnItem = true;
        inventoryComponent.OnPointerEnter(SlotNum);

        if (!IsSlotExist || eventData.dragging)
            return;

        StartWaiting(1.5f, CheckOnSlot);
    }
    void StartWaiting(float seconds, Action action)
    {
        StopWaiting();
        enumWaiting = WaitOnSlot(seconds, action);
        StartCoroutine(enumWaiting);
    }
    IEnumerator WaitOnSlot(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
    void StopWaiting()
    {
        if (enumWaiting != null)
            StopCoroutine(enumWaiting);
    }
    void CheckOnSlot()
    {
        if (isOnItem && !isDragActive)
            inventoryComponent.SetDescription(SlotNum, Input.mousePosition);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isOnItem = false;
        inventoryComponent.ActiveDescription(false);
        inventoryComponent.OnPointerEnter(eventData);
    }


    #endregion
}
