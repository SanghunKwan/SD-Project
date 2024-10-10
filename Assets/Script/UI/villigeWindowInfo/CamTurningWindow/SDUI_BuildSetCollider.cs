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

        private void Start()
        {
            image = GetComponent<Image>();
            teamUI = transform.parent.parent.GetComponent<TeamUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = effectColor;
            if (isDrag)
            {
                isEnter = true;
                teamUI.EnterCollider(transform.parent.GetSiblingIndex(), villigeInteract.now_villigeInteract);
                villigeInteract.now_villigeInteract.NoMove();

                villigeInteract.now_villigeInteract.dragEndEvent += DragEnd;
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isDrag)
            {
                teamUI.ExitCollider(transform.parent.GetSiblingIndex());
                villigeInteract.now_villigeInteract.dragEndEvent -= DragEnd;
            }
            image.color = Color.clear;
            ColliderExit();
        }
        void DragEnd()
        {
            if (isEnter)
            {
                if (villigeInteract.now_villigeInteract.teamUIData.CanLoadData(out TeamUI team, out int sibling))
                    team.DeleteSave(sibling, sibling == transform.parent.GetSiblingIndex());

                teamUI.CharacterSave(transform.parent.GetSiblingIndex(), villigeInteract.now_villigeInteract);

                villigeInteract.now_villigeInteract.teamUIData.SaveData(teamUI, transform.parent.GetSiblingIndex());
                villigeInteract.now_villigeInteract.ChangeImage(AddressableManager.BuildingImage.Tower);
            }
            ColliderExit();
        }
        void ColliderExit()
        {
            isEnter = false;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            isVilligeInteractExist = teamUI.villigeInteracts[transform.parent.GetSiblingIndex()] != null
                && eventData.button != PointerEventData.InputButton.Left;

            if (!isVilligeInteractExist)
                return;

            teamUI.villigeInteracts[transform.parent.GetSiblingIndex()].BeginDragOffset(eventData, image);
            OnPointerEnter(eventData);
            teamUI.DeleteSave(transform.parent.GetSiblingIndex());
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
                || teamUI.villigeInteracts[transform.parent.GetSiblingIndex()] == null)
                return;

            teamUI.villigeInteracts[transform.parent.GetSiblingIndex()].OnPointerClick(eventData);
        }
    }
}