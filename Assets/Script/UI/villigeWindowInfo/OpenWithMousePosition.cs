using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenWithMousePosition : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void OpenOnMouse(PointerEventData pointer)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        InWindow(pointer.position.x, pointer.position.y);
    }

    void InWindow(float x, float y)
    {
        float fX, fY;

        fX = x;
        fY = y;


        if (x + rect.sizeDelta.x > Screen.width)
        {
            fX = Screen.width - rect.sizeDelta.x;
        }

        if (y - rect.sizeDelta.y < 0)
        {
            fY = rect.sizeDelta.y;
        }

        rect.position = new Vector2(fX, fY);

    }
    public void OpenOnBoxes(UnityEngine.UI.Image image)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        float fX = image.rectTransform.position.x - rect.sizeDelta.x + image.rectTransform.sizeDelta.x;
        float fY = image.rectTransform.position.y - image.rectTransform.sizeDelta.y;
        rect.position = new Vector2(fX, fY);
    }


    public void Close()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
