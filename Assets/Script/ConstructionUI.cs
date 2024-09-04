using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ConstructionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Image image;
    Text text;
    [SerializeField] GameObject scroll;

    Dictionary<bool, Action> dicSetColor = new Dictionary<bool, Action>();


    public void OnPointerClick(PointerEventData eventData)
    {

        ToggleScroll();
    }
    void ToggleScroll()
    {
        scroll.SetActive(!scroll.gameObject.activeSelf);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scroll.gameObject.activeSelf)
            return;

        ColorSet();
    }
    void ColorSet()
    {
        image.color -= new Color(0.15f, 0.15f, 0, 0);
        text.color = 0.85f * Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scroll.gameObject.activeSelf)
            return;

        ButtonColorReset();
    }

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        text = transform.GetChild(0).GetComponent<Text>();

        dicSetColor.Add(true, ColorSet);
        dicSetColor.Add(false, ButtonColorReset);

        GameManager.manager.callConstructionUI = GameManagerInputSpace;
    }

    public void ButtonColorReset()
    {
        image.color = Color.white;
        text.color = Color.white;
    }

    void GameManagerInputSpace()
    {
        ToggleScroll();
        dicSetColor[scroll.gameObject.activeSelf]();

    }
}
