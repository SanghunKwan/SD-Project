using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPage : DragFromChild
{
    [SerializeField] ExpeditionPage page;
    float startPosition;
    [SerializeField] float decelerationRate;
    Vector2 velocity;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        page.StopMoveAnim();
        startPosition = page.rectTransform.anchoredPosition.x;
        MoveUI(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        MoveUI(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        MoveUI(eventData);
        CalculateInertia();
    }

    void MoveUI(PointerEventData eventData)
    {
        float move = eventData.position.x - eventData.pressPosition.x;
        page.rectTransform.anchoredPosition = (move + startPosition) * Vector2.right;
        velocity = Vector2.right * (eventData.delta.x + velocity.x) / 2;
    }
    void CalculateInertia()
    {
        float distance = decelerationRate * 2 / (1 - decelerationRate);
        int pageNum = Convert.ToInt32(page.rectTransform.anchoredPosition.x + (distance * velocity.x) <= -500);
        page.GotoPage(pageNum);
        page.LightAnimation(pageNum);
    }
}
