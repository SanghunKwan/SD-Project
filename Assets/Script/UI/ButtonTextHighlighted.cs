using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class ButtonTextHighlighted : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool isTextUI;
    TextMeshProUGUI textPro;
    Text textUI;
    Button button;

    Color colorChange;
    Color originColor;
    Color textColor
    {
        get
        {
            if (isTextUI)
                return textUI.color;
            return textPro.color;
        }
        set
        {
            if (isTextUI)
                textUI.color = value;
            else
                textPro.color = value;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            textColor *= colorChange;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        textColor = originColor;
    }
    private void OnDisable()
    {
        textColor = originColor;
    }
    private void Awake()
    {
        if (isTextUI)
        {
            textUI = transform.GetChild(0).GetComponent<Text>();
            originColor = textUI.color;
        }
        else
        {
            textPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            originColor = textPro.color;
        }

        button = GetComponent<Button>();
        colorChange = button.colors.pressedColor;
    }

}
