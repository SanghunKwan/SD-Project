using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unit;
using UnityEngine.EventSystems;


public class NameChange : MonoBehaviour
{
    TextMeshProUGUI unitTypeText;
    TMP_InputField nameInputField;
    Hero hero;

    private void Awake()
    {
        nameInputField = transform.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        unitTypeText = nameInputField.transform.Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>();
    }

    public void NameChangeClick()
    {
        nameInputField.interactable = !nameInputField.interactable;
        nameInputField.OnSelect(new BaseEventData(EventSystem.current));
    }

    public void GetName(Hero getHero)
    {
        hero = getHero;
        nameInputField.text = hero.stat.NAME;
        unitTypeText.text = hero.stat.type;
    }

    #region on¿Ã∫•∆Æ
    public void EndEdit()
    {
        hero.stat.NAME = nameInputField.text;
        hero.Villige_CheckText();
    }

    #endregion

}
