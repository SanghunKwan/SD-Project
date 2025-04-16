using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unit;
using System;
using System.Text;

public class InfoBox : MonoBehaviour
{
    Image[] images;
    TextMeshProUGUI[] textMeshProUGUIs;
    [SerializeField] ExplainData explainData;

    Dictionary<Type, Action<Hero, Enum>> actions = new Dictionary<Type, Action<Hero, Enum>>();
    Dictionary<Type, Action<SaveData.HeroData, Enum>> actionsHeroData = new();
    Dictionary<Type, Action<Hero, Enum, int>> buttonActions = new();
    StringBuilder builder;

    [SerializeField] SetBuildingMat materialPriceBox;
    OpenWithMousePosition materialPosition;

    enum TextBoxType
    {
        DefaultRight,
        Left
    }

    private void Awake()
    {
        images = new Image[2];
        textMeshProUGUIs = new TextMeshProUGUI[2];
        for (int i = 0; i < 2; i++)
        {
            images[i] = transform.GetChild(i).GetComponent<Image>();
            textMeshProUGUIs[i] = images[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        materialPosition = materialPriceBox.GetComponent<OpenWithMousePosition>();

        actions.Add(typeof(AddressableManager.PreviewImage), (qwer, uiop) =>
        { PreviewImage(qwer, (AddressableManager.PreviewImage)uiop); });
        actions.Add(typeof(AddressableManager.EquipsImage), (qwer, uiop) =>
        { EquipsImage(qwer, (AddressableManager.EquipsImage)uiop); });

        actionsHeroData.Add(typeof(AddressableManager.PreviewImage), (qwer, uiop) =>
        { PreviewImage(qwer, (AddressableManager.PreviewImage)uiop); });
        actionsHeroData.Add(typeof(AddressableManager.EquipsImage), (qwer, uiop) =>
        { EquipsImage(qwer, (AddressableManager.EquipsImage)uiop); });

        buttonActions.Add(typeof(AddressableManager.PreviewImage), (hero, enumValue, heroSkillArrayNum) =>
        PreviewImageCompare(hero, (AddressableManager.PreviewImage)enumValue, heroSkillArrayNum));
        buttonActions.Add(typeof(AddressableManager.EquipsImage), (hero, enumValue, heroEquipArrayNum) =>
        EquipsImageCompare(hero, (AddressableManager.EquipsImage)enumValue, heroEquipArrayNum));

        builder = new StringBuilder();
    }


    void PreviewImage(Hero hero, AddressableManager.PreviewImage skillImage)
    {
        PreviewImage(hero.Getnum, skillImage, hero.SkillsNum[(int)skillImage]);
    }
    void PreviewImage(TypeNum heroType, AddressableManager.PreviewImage skillImage, int skillNum, TextBoxType textBoxType = TextBoxType.DefaultRight)
    {
        builder.Clear();
        int num = (int)skillImage + ((int)heroType * 4) + ((int)TypeNum.PlayerTypeLength * (skillNum - 1) * 4);

        builder.Append(TypetoHex((ExplainData.TypeName)((int)skillImage + 5)));
        builder.Append(explainData.GetSkillExplain(num).name);
        builder.AppendLine("</color>");
        builder.Append(TypetoHex(explainData.GetSkillExplain(num).type));
        builder.Append(explainData.GetSkillExplain(num).type.ToString());
        builder.AppendLine("</color>");
        builder.Append(GetColorHex(explainData.arrColor[(int)ExplainData.ColorArr.Pros]));
        builder.Append(explainData.GetSkillExplain(num).Pros);
        builder.Append("</color>");
        builder.Append(explainData.GetSkillExplain(num).Cons);
        builder.Append("</color>");
        builder.Append(explainData.GetSkillExplain(num).Descrip);

        textMeshProUGUIs[(int)textBoxType].text = builder.ToString();

        SetExplanationLength(explainData.GetSkillExplain(num).ExplainLength, 0, textBoxType);
    }
    void PreviewImageCompare(Hero hero, AddressableManager.PreviewImage skillImage, int skillNum)
    {
        PreviewImage(hero.Getnum, skillImage, hero.SkillsNum[(int)skillImage], TextBoxType.Left);
        PreviewImage(hero.Getnum, skillImage, skillNum, TextBoxType.DefaultRight);
        materialPosition.OpenOnBoxes(images[0]);
        materialPriceBox.GetData((int)skillImage + ((skillNum - 1) * 4) + 1, SetBuildingMat.MaterialsType.SkillUpgradeNeed);
    }
    void PreviewImage(SaveData.HeroData hero, AddressableManager.PreviewImage skillImage)
    {
        PreviewImage(hero.unitData.objectData.cur_status.type, skillImage, hero.skillNum[(int)skillImage]);
    }


    void EquipsImage(Hero hero, AddressableManager.EquipsImage equipsImage)
    {
        EquipsImage(hero.Getnum, equipsImage, hero.EquipsNum[(int)equipsImage]);
    }
    void EquipsImage(TypeNum heroType, AddressableManager.EquipsImage equipsImage, int equipNum, TextBoxType textBoxType = TextBoxType.DefaultRight)
    {
        builder.Clear();
        int num = (int)equipsImage + (((int)heroType) * 3)
                                   + ((int)TypeNum.PlayerTypeLength * (equipNum - 1) * 3);

        builder.AppendLine(explainData.GetItemExplain(num).name);
        builder.Append(GetarrInList((int)explainData.GetItemExplain(num).quality));
        builder.Append("<line-height=25>");
        builder.Append(explainData.GetItemExplain(num).quality + 4);
        builder.AppendLine("</color></line-height>");
        builder.Append("</line-height>");
        builder.Append(explainData.GetItemExplain(num).HP);
        builder.Append(explainData.GetItemExplain(num).ATK);
        builder.Append(explainData.GetItemExplain(num).DOG);
        builder.Append(explainData.GetItemExplain(num).SPEED);
        builder.Append(explainData.GetItemExplain(num).ViewAngle);
        builder.Append(explainData.GetItemExplain(num).ViewRange);
        builder.Append(explainData.GetItemExplain(num).Accuracy);
        builder.Append(explainData.GetItemExplain(num).AtkSpeed);
        builder.Append(explainData.GetItemExplain(num).Range);
        builder.Append(explainData.GetItemExplain(num).Stress);

        textMeshProUGUIs[(int)textBoxType].text = builder.ToString();

        SetExplanationLength(explainData.GetItemExplain(num).ExplainLength, 5, textBoxType);
    }
    void EquipsImageCompare(Hero hero, AddressableManager.EquipsImage equipsImage, int equipNum)
    {
        EquipsImage(hero.Getnum, equipsImage, hero.EquipsNum[(int)equipsImage], TextBoxType.Left);
        EquipsImage(hero.Getnum, equipsImage, equipNum, TextBoxType.DefaultRight);
        materialPosition.OpenOnBoxes(images[0]);
        materialPriceBox.GetData((int)equipsImage + ((equipNum - 1) * 3) + 1, SetBuildingMat.MaterialsType.UpgradeNeed);
    }
    void EquipsImage(SaveData.HeroData hero, AddressableManager.EquipsImage equipsImage)
    {
        EquipsImage(hero.unitData.objectData.cur_status.type, equipsImage, hero.skillNum[(int)equipsImage]);
    }


    string TypetoHex<T>(in T type) where T : struct, Enum
    {
        return GetColorHex(explainData.arrColor[Convert.ToInt32(type)]);
    }

    string GetColorHex(Color color)
    {
        StringBuilder tempBuilder = new StringBuilder();

        byte r = (byte)(color.r * 255);
        byte g = (byte)(color.g * 255);
        byte b = (byte)(color.b * 255);

        tempBuilder.Append("<color=#");
        tempBuilder.Append(r.ToString("X2"));
        tempBuilder.Append(g.ToString("X2"));
        tempBuilder.Append(b.ToString("X2"));
        tempBuilder.Append(">");

        return tempBuilder.ToString();
    }
    string GetarrInList(int num)
    {
        return GetColorHex(explainData.arrColor[explainData.quality2Color[num]]);
    }

    void SetExplanationLength(int nLineCount, float add = 0, TextBoxType textBoxType = TextBoxType.DefaultRight)
    {
        images[(int)textBoxType].rectTransform.sizeDelta = new Vector2(images[(int)textBoxType].rectTransform.sizeDelta.x, nLineCount * 24 + 16 + add);
    }
    public void SetTextMessage<T>(Hero hero, T equipsImage) where T : struct, Enum
    {
        actions[typeof(T)](hero, equipsImage);
        //   textMeshProUGUI.text = TypetoHex((ExplainData.TypeName)(Convert.ToInt32(equipsImage) + 5));
    }
    public void SetTextMessage<T>(SaveData.HeroData hero, T equipsImage) where T : struct, Enum
    {
        actionsHeroData[typeof(T)](hero, equipsImage);
        //   textMeshProUGUI.text = TypetoHex((ExplainData.TypeName)(Convert.ToInt32(equipsImage) + 5));
    }
    public void SetTextMessage<T>(Hero hero, T equipsImage, int heroNumArrayValue) where T : struct, Enum
    {
        buttonActions[typeof(T)](hero, equipsImage, heroNumArrayValue);
    }
    public void CompareBoxTurnOff()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        materialPriceBox.transform.GetChild(0).gameObject.SetActive(false);
    }
}
