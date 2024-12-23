using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBackGround : MonoBehaviour
{
    RectTransform rectTransform;
    public float size = 100;
    Vector2 sizeDelta;
    Camera mainCam;
    Image maskImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        maskImage = GetComponent<Image>();
        mainCam = Camera.main;
    }

    #region ±â´É
    public void SetHighLight(in Vector3 position, float circleSize)
    {
        Vector3 vec = mainCam.WorldToScreenPoint(position + (circleSize * 0.5f * Vector3.up));
        SetHighLightUI(vec, circleSize);
    }
    public void SetSize(float circleSize)
    {
        sizeDelta.x = size * circleSize;
        sizeDelta.y = size * circleSize;

        rectTransform.sizeDelta = sizeDelta;
    }
    public void SetOff()
    {
        gameObject.SetActive(false);
    }
    public void SetActiveHole(bool onoff)
    {
        maskImage.enabled = onoff;
    }
    public void SetHighLightUI(in Vector3 position, float circleSize)
    {
        gameObject.SetActive(true);
        SetSize(circleSize);
        SetActiveHole(true);

        rectTransform.position = position;
    }
    #endregion
}
