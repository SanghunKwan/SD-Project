using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VilligeTeamCollider : villigeBase, IPointerEnterHandler, IPointerExitHandler
{
    Action<PointerEventData>[] beginDrag = new Action<PointerEventData>[3];
    Action<PointerEventData>[] dragging = new Action<PointerEventData>[3];
    Action<PointerEventData>[] endDrag = new Action<PointerEventData>[3];
    public Action dragEndEvent { get; set; } = () => { };

    Dictionary<bool, Action<PointerEventData>> dragMoveAction = new Dictionary<bool, Action<PointerEventData>>();
    public static bool isDrag { get; private set; }

    [SerializeField] HandTeam handTeam;
    CharacterList characterList;
    public villigeViewPort viewPort { get; private set; }
    public static VilligeTeamCollider now_Collider { get; private set; }

    Image tempImage;
    Vector2 MouseImageOffset;
    [SerializeField] Button button;
    #region 기본설정
    protected override void Start()
    {
        base.Start();

        ActNone();
        ButtonRightAct();
        viewPort = transform.parent.GetComponent<villigeViewPort>();
        characterList = viewPort.transform.parent.parent.parent.parent.GetComponent<CharacterList>();
        ChangeButtonAct(viewPort.teamName);
        DictionaryAddAction();
    }
    void ActNone()
    {
        beginDrag[0] = (evenData) => { };
        dragging[0] = (evenData) => { };
        endDrag[0] = (evenData) => { };

        beginDrag[2] = (evenData) => { };
        dragging[2] = (evenData) => { };
        endDrag[2] = (evenData) => { };
    }
    void ButtonRightAct()
    {
        beginDrag[1] = (evenData) => { tempImage = CreateHandImage(); SetVisible(false); };
        dragging[1] = (evenData) => { ImageMove(evenData); CheckRaycastTarget(evenData); };
        endDrag[1] = (evenData) => { CleanDrag(); SetVisible(true); PositionClosing(); };
        //villigeInteract로 드래그 시 병합
        //villigeViewPort로 드래그 시 위치 이동.
        //towerCollider로 드래그 시 0~5까지 등록.
    }
    void ChangeButtonAct(string str)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Click(str));
    }
    void DictionaryAddAction()
    {
        dragMoveAction.Add(true, DragMove);
        dragMoveAction.Add(false, (eventdata) => DragNoMove());
    }
    #endregion
    #region 드래그 시작
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        now_Collider = this;
        isDrag = true;
        DragNoMove();
        beginDrag[(int)eventData.button](eventData);
    }
    Image CreateHandImage()
    {
        HandTeam tempTeam = Instantiate(handTeam, transform.parent.position, Quaternion.identity, characterList.transform.parent);
        tempTeam.Init(viewPort);

        MouseImageOffset = Input.mousePosition - tempTeam.image.rectTransform.position;

        return tempTeam.image;
    }
    void SetVisible(bool onoff)
    {
        viewPort.DragInvisible(onoff);
        image.enabled = onoff;
    }
    #endregion
    #region 드래그중
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (eventData.pointerCurrentRaycast.gameObject is not null)
            dragging[(int)eventData.button](eventData);
    }
    void ImageMove(PointerEventData eventData)
    {
        tempImage.rectTransform.position = eventData.position - MouseImageOffset;
    }
    void CheckRaycastTarget(PointerEventData eventData)
    {
        GameObject dragObject = eventData.pointerCurrentRaycast.gameObject;

        bool isDragMove = dragObject.CompareTag("NameTag") && transform.parent != dragObject.transform;

        dragMoveAction[isDragMove](eventData);
        //viewport != raycast.gameObject
    }
    void DragMove(PointerEventData eventData)
    {
        characterList.MoveBoard(eventData.pointerCurrentRaycast.gameObject, eventData.position.y);
    }
    void DragNoMove()
    {
        //viewPort gameobject
        characterList.NoMove(viewPort.gameObject, false);
    }
    #endregion
    #region 드래그종료
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        isDrag = false;
        dragEndEvent();
        dragEndEvent = () => { };
        endDrag[(int)eventData.button](eventData);
    }
    void CleanDrag()
    {
        Destroy(tempImage.gameObject);
    }
    void PositionClosing()
    {
        characterList.EndDrag(viewPort);
    }
    #endregion
    #region 마우스 올려서 강조
    public void OnPointerEnter(PointerEventData eventData)
    {
        viewPort.TeamCodeHighlight(FontStyle.Bold);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        viewPort.TeamCodeHighlight(FontStyle.Normal);
    }
    #endregion
    #region 버튼 기능
    public void Click(string str)
    {
        GameManager.manager.ArmySelect(str);
        GameManager.manager.ScreenToPoint(PlayerNavi.nav.getCenter);
    }
    public void ButtonSetInteractive(bool onoff)
    {
        button.interactable = onoff;
    }

    #endregion
}
