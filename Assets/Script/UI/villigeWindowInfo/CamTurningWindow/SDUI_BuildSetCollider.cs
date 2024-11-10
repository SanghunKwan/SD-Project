using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDUI
{
    public class SDUI_BuildSetCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        TeamUI teamUI;
        Image image;

        public bool isDrag { get => villigeInteract.isDrag; }
        bool isEnter;
        bool isVilligeInteractExist;
        [SerializeField] Color effectColor;

        Action<PointerEventData>[] beginDrag;
        Action<PointerEventData>[] onDrag;
        Action<PointerEventData>[] endDrag;
        [SerializeField] DragPage page;
        enum DragType
        {
            Interct,
            TeamCollider
        }
        private void Start()
        {
            image = GetComponent<Image>();
            teamUI = transform.parent.parent.GetComponent<TeamUI>();
            SetDragActions();
        }
        void SetDragActions()
        {
            beginDrag = new Action<PointerEventData>[2];
            beginDrag[0] = BeginDragFalse;
            beginDrag[1] = BeginDragTrue;

            onDrag = new Action<PointerEventData>[2];
            onDrag[0] = OnDragFalse;
            onDrag[1] = OnDragTrue;

            endDrag = new Action<PointerEventData>[2];
            endDrag[0] = EndDragFalse;
            endDrag[1] = EndDragTrue;
        }
        #region 드래그 입출
        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = effectColor;
            if (isDrag)
            {
                isEnter = true;
                ChangeText(villigeInteract.now_villigeInteract);
                villigeInteract.now_villigeInteract.dragEndEvent += DragEnd;
            }

        }
        public void ChangeText(villigeInteract interact)
        {
            teamUI.EnterCollider(transform.parent.GetSiblingIndex(), interact);
            interact.NoMove();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDrag)
            {
                ResetText();
                villigeInteract.now_villigeInteract.dragEndEvent -= DragEnd;
            }
            if (isVilligeInteractExist)
            {

                teamUI.DeleteSave(transform.parent.GetSiblingIndex());
            }
            image.color = Color.clear;
            ColliderExit();
        }
        public void ResetText()
        {
            teamUI.ExitCollider(transform.parent.GetSiblingIndex());
        }
        public void NullText()
        {
            teamUI.ResetTeam(transform.parent.GetSiblingIndex());
        }


        #endregion
        void DragEnd()
        {
            if (isEnter)
            {
                DataChange(villigeInteract.now_villigeInteract);
            }
            ColliderExit();
        }
        public void DataChange(villigeInteract interact)
        {
            if (interact.teamUIData.CanLoadData(out TeamUI team, out int sibling))
                team.DeleteSave(sibling, team == teamUI && sibling == transform.parent.GetSiblingIndex());

            teamUI.CharacterSave(transform.parent.GetSiblingIndex(), interact);

            interact.teamUIData.SaveData(teamUI, transform.parent.GetSiblingIndex());
            interact.ChangeImage(AddressableManager.BuildingImage.Tower);
        }
        public void DataErase()
        {
            teamUI.CharacterSave(transform.parent.GetSiblingIndex(), null);
        }
        void ColliderExit()
        {
            isEnter = false;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            isVilligeInteractExist = teamUI.villigeInteracts[transform.parent.GetSiblingIndex()] is not null
                && eventData.button != PointerEventData.InputButton.Left;

            beginDrag[Convert.ToInt32(isVilligeInteractExist)](eventData);
        }
        void BeginDragTrue(PointerEventData eventData)
        {
            teamUI.villigeInteracts[transform.parent.GetSiblingIndex()].BeginDragOffset(eventData, image);
            OnPointerEnter(eventData);
            teamUI.villigeInteracts[transform.parent.GetSiblingIndex()].ChangeImage(AddressableManager.BuildingImage.Tower, false);
        }
        void BeginDragFalse(PointerEventData eventData)
        {
            page.OnBeginDrag(eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            onDrag[Convert.ToInt32(isVilligeInteractExist)](eventData);
        }
        void OnDragTrue(PointerEventData eventData)
        {
            villigeInteract.now_villigeInteract.OnDrag(eventData);
        }
        void OnDragFalse(PointerEventData eventData)
        {
            page.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            endDrag[Convert.ToInt32(isVilligeInteractExist)](eventData);
        }
        void EndDragTrue(PointerEventData eventData)
        {
            villigeInteract.now_villigeInteract.OnEndDrag(eventData);
            isVilligeInteractExist = false;
        }
        void EndDragFalse(PointerEventData eventData)
        {
            page.OnEndDrag(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left
                || teamUI.villigeInteracts[transform.parent.GetSiblingIndex()] == null)
                return;

            teamUI.villigeInteracts[transform.parent.GetSiblingIndex()].OnPointerClick(eventData);
        }
    }
}