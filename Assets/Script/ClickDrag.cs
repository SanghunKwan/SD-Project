using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unit;
using System;

public class ClickDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerMoveHandler, IPointerDownHandler
{
    [SerializeField] Image rectangle;
    [SerializeField] Image copyRec;
    [SerializeField] float speed;
    [SerializeField] int suburb;
    Vector3 dragStart;
    PointerEventData pointerEventData;
    bool suburbUpdate = false;
    public bool MiniMapClick { private get; set; } = false;

    Vector3 move;
    Vector3 movey;
    [SerializeField] GameObject inventoryDescription;
    Action pointerDown = () => { };

    Camera camMain;
    void Start()
    {
        copyRec = Instantiate(rectangle, transform.parent);
        camMain = Camera.main;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            copyRec.gameObject.SetActive(true);

            dragStart = Data.Instance.UItoCanvas(eventData.position);
            copyRec.rectTransform.localPosition = dragStart;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !suburbUpdate)
        {
            Dragging(eventData);
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick(eventData);
        }
    }
    void RightClick(PointerEventData eventData)
    {
        Ray ray = camMain.ScreenPointToRay(eventData.position);
        int layerMask = 1 << LayerMask.NameToLayer("Character");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ~layerMask))
        {
            GameManager.manager.OrderUnit(hit);
        }
    }
    public void GetMousePoint(string Layer, out RaycastHit _hit)
    {
        Ray ray = camMain.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer(Layer);
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, ~layerMask);
        _hit = hit;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Select(1);
        }
    }
    void Select(int count)
    {
        copyRec.gameObject.SetActive(false);

        GameManager.manager.DragSelectingEnd(count);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            dragStart = Data.Instance.UItoCanvas(eventData.position);
            Dragging(eventData);
            Select(eventData.clickCount);
            InputEffect.e.PrintEffect(eventData.pointerCurrentRaycast.worldPosition, 0);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick(eventData);
            InputEffect.e.PrintEffect(eventData.pointerCurrentRaycast.worldPosition, 0);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        pointerEventData = eventData;
        GameManager.manager.pointerEventData = pointerEventData;
    }
    void Update()
    {
        if (pointerEventData == null)
            return;

        if (MiniMapClick)
        {
            suburbUpdate = false;
            return;
        }
        if (pointerEventData.position.x > Screen.width - suburb)
        {
            move = Vector3.right * Time.unscaledDeltaTime * speed;
            camMain.transform.position += move;
            suburbUpdate = true;
        }
        else if (pointerEventData.position.x < suburb)
        {
            move = Vector3.left * Time.unscaledDeltaTime * speed;
            camMain.transform.position += move;
            suburbUpdate = true;

        }
        else
        {
            suburbUpdate = false;
        }


        if (pointerEventData.position.y > Screen.height - suburb)
        {
            movey = -Vector3.back * Time.unscaledDeltaTime * speed;
            camMain.transform.position += movey;
            suburbUpdate = true;

        }
        else if (pointerEventData.position.y < suburb)
        {
            movey = Vector3.back * Time.unscaledDeltaTime * speed;
            camMain.transform.position += movey;
            suburbUpdate = true;
        }

        if (suburbUpdate)
        {
            MoveScreenRatioCal(move);
            MoveScreenRatioCaly(movey);
            Dragging(pointerEventData);
        }
        move = Vector3.zero;
        movey = Vector3.zero;

    }
    void MoveScreenRatioCal(Vector3 move)
    {
        Vector3 moveScreenRatio = move * Screen.height / (camMain.orthographicSize * 2);

        dragStart -= moveScreenRatio;
        copyRec.rectTransform.localPosition -= moveScreenRatio;
        GameManager.manager.PointMove(moveScreenRatio);
    }
    void MoveScreenRatioCaly(Vector3 move)
    {
        Vector3 vector3 = new Vector3(move.x, move.z, move.y);

        Vector3 moveScreenRatio = vector3 * Screen.height / (camMain.orthographicSize * 2) * Mathf.Sin(Mathf.Deg2Rad * 40);


        dragStart -= moveScreenRatio;
        copyRec.rectTransform.localPosition -= moveScreenRatio;
        GameManager.manager.PointMove(moveScreenRatio);
    }

    void Dragging(PointerEventData eventData)
    {
        Vector3 mouse = Data.Instance.UItoCanvas(eventData.position);
        Vector3 size = mouse - dragStart;
        if (size.x < 0)
        {
            size.x = Mathf.Abs(size.x);
            copyRec.rectTransform.localPosition = new Vector2(mouse.x, copyRec.rectTransform.localPosition.y);
        }
        if (size.y < 0)
        {
            size.y = Mathf.Abs(size.y);
            copyRec.rectTransform.localPosition = new Vector2(copyRec.rectTransform.localPosition.x, mouse.y);
        }
        copyRec.rectTransform.sizeDelta = new Vector2(size.x, size.y);

        GameManager.manager.DragUnitPosition(Data.Instance.CanvastoUI(copyRec.rectTransform.localPosition), size, pointerEventData.dragging);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown();
    }
    public void SetDown(Action action)
    {
        pointerDown = action;
    }
}
