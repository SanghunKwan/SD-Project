using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBackGround : MonoBehaviour
{
    RectTransform rectTransform;
    public float size = 100;
    Vector2 sizeDelta;
    Camera mainCam;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    #region ±â´É
    public void SetHighLight(in Vector3 position, float circleSize)
    {
        gameObject.SetActive(true);
        SetSize(circleSize);
        SetTarget(position);
    }
    public void SetSize(float circleSize)
    {
        sizeDelta.x = size * circleSize;
        sizeDelta.y = size * circleSize;

        rectTransform.sizeDelta = sizeDelta;
    }
    public void SetTarget(in Vector3 position)
    {
        Vector3 vec = mainCam.WorldToScreenPoint(position + Vector3.up);
        rectTransform.position = vec;
    }
    public void SetOff()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
