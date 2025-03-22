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
    SummonHeroNameTag heroNameTag;

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
        heroNameTag = null;
        hero = getHero;
        nameInputField.text = hero.name;
        unitTypeText.text = hero.stat.type.ToString();
    }
    public void GetName(SummonHeroNameTag getHeroNameTag)
    {
        hero = null;
        heroNameTag = getHeroNameTag;
        nameInputField.text = getHeroNameTag.heroData.name;
        unitTypeText.text = getHeroNameTag.heroData.unitData.objectData.cur_status.type.ToString();
    }

    #region on¿Ã∫•∆Æ
    public void EndEdit()
    {
        if (hero != null)
        {
            hero.name = nameInputField.text;
            hero.Villige_CheckText();
        }
        else
        {
            heroNameTag.heroData.name = nameInputField.text;
            heroNameTag.CheckNameTag();
        }
    }

    #endregion
    private void OnDisable()
    {
        hero = null;
        heroNameTag = null;
    }
}
