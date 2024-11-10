using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckUI : tempMenuWindow
{
    [SerializeField] Button[] buttons;
    [SerializeField] TextMeshProUGUI[] buttonTexts;

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Sprite[] buttonImages;
    [SerializeField] Vector2[] sizes;


    [Serializable]
    public class ToUserAnswers
    {
        public COMPONENTINDEX buttonIndex;
        public string buttonAnswer;
    }

    public enum BUTTONTYPE
    {
        DefaultSquare,
        Circle
    }
    public enum COMPONENTINDEX
    {
        Left,
        Right
    }

    #region GUI
    public void SetUI(in string message, params ToUserAnswers[] answers)
    {
        text.text = message;

        foreach (var item in answers)
        {
            buttonTexts[(int)item.buttonIndex].text = item.buttonAnswer;
        }
    }
    public void ChangeButton(COMPONENTINDEX index, BUTTONTYPE type)
    {
        int buttonIndex = (int)index;
        int buttonType = (int)type;

        buttons[buttonIndex].image.sprite = buttonImages[buttonType];
        SetButtonType(buttonIndex, buttonType);
    }

    void SetButtonSize(int index, float width, float height)
    {
        buttons[index].image.rectTransform.sizeDelta = new Vector2(width, height);
    }
    void SetButtonType(int index, int type)
    {
        SetButtonSize(index, sizes[type].x, sizes[type].y);
    }
    #endregion
    #region ±â´É
    public void ObjectRemove()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public void AddButtonFunction(COMPONENTINDEX index, Action action)
    {
        buttons[(int)index].onClick.AddListener(() => action());

    }
    public void SetButtonInactiveNoPositionChange(COMPONENTINDEX index)
    {
        buttons[(int)index].gameObject.SetActive(false);
    }
    public void SetButtonInactive(COMPONENTINDEX index)
    {
        buttons[(int)index].transform.parent.gameObject.SetActive(false);
    }

    #endregion
    public override void OnOffWindow()
    {
        ObjectRemove();
    }
}
