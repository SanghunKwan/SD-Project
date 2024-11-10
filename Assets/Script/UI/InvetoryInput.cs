using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using Unit;
using System.Linq;

public class InventoryInput : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerMoveHandler, IPointerDownHandler
{
    int SlotNum;
    int prevSlotNum;
    bool isOnItem;
    bool isPlus;
    bool isNotLine;

    bool isDragActive;
    Action[] clicks;

    Image dragImage;
    Vector2 pointerOffset;

    InventoryComponent inventoryComponent;
    private void Start()
    {
        Clicks();
        GameObject.FindWithTag("CanvasWorld").transform.GetChild(0).GetComponent<ClickDrag>().SetDown(() => inventoryComponent.ActiveDescription(false));
        inventoryComponent = transform.parent.parent.GetComponent<InventoryComponent>();
    }
    void Clicks()
    {
        clicks = new Action[2];

        clicks[0] = () => { InventoryManager.i.CallItemInfo(SlotNum); };//left
        clicks[1] = () => { InventoryManager.i.Use(SlotNum); };//right
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isOnItem && InventoryManager.i.CheckSlot(SlotNum))
        {
            dragImage = InventoryManager.i.HideSlot(SlotNum, eventData.pressPosition, out pointerOffset, out prevSlotNum);
            isDragActive = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragActive)
            dragImage.rectTransform.position = eventData.position - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragActive)
            return;

        Destroy(dragImage.gameObject);
        isDragActive = false;
        InventoryManager.i.ReturnItem(prevSlotNum);

        if (isOnItem && eventData.button == PointerEventData.InputButton.Left)
            InventoryManager.i.ItemSwap(prevSlotNum, SlotNum);

        else if (eventData.position.x < 1390 || eventData.position.y > 235)
        {
            var units = GameManager.manager.playerCharacter;
            if (PlayerNavi.nav.lists.Count > 0)
                units = PlayerNavi.nav.lists.Select(character => character.cUnit).ToList();

            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            int layerMask = 1 << LayerMask.NameToLayer("Floor");
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                CUnit unit = GameManager.manager.GetNearest(units, hit.point,
                                                (cunit) => true, 1000) as CUnit;

                if (eventData.button == PointerEventData.InputButton.Left)
                    InventoryManager.i.ThrowAway(unit, prevSlotNum, hit.point);

                else if (eventData.button == PointerEventData.InputButton.Right)
                    InventoryManager.i.UseOne(unit, prevSlotNum);

            }
            else
                Debug.Log("Raycast  ½ÇÆÐ");
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOnItem && InventoryManager.i.CheckSlot(SlotNum) && !eventData.dragging)
            clicks[(int)eventData.button]();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        inventoryComponent.ActiveDescription(true);
        inventoryComponent.SetDescription(SlotNum, eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inventoryComponent.ActiveDescription(false);
    }
}
