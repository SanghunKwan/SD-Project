using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unit;
using System;

public class InfoBox : MonoBehaviour
{
    Image image;
    TextMeshProUGUI textMeshProUGUI;
    [SerializeField] ExplainData explainData;

    Dictionary<Type, Action<Hero, Enum>> actions = new Dictionary<Type, Action<Hero, Enum>>();

    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        textMeshProUGUI = image.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        actions.Add(typeof(AddressableManager.PreviewImage), (qwer, uiop) => { PreviewImage(qwer, (AddressableManager.PreviewImage)uiop); });
        actions.Add(typeof(AddressableManager.EquipsImage), (qwer, uiop) => { EquipsImage(qwer, (AddressableManager.EquipsImage)uiop); });
    }
    void PreviewImage(Hero hero, AddressableManager.PreviewImage skillImage)
    {
        int num = (int)skillImage + (((int)hero.Getnum) * 4);

        textMeshProUGUI.text = TypetoHex((ExplainData.TypeName)((int)skillImage + 5)) +
                               explainData.GetSkillExplain(num).name + "</color>" + "\n" +
                               TypetoHex(explainData.GetSkillExplain(num).type) +
                               explainData.GetSkillExplain(num).type.ToString() + "</color>" + "\n" +
                               GetColorHex(explainData.arrColor[(int)ExplainData.ColorArr.Pros]) +
                               explainData.GetSkillExplain(num).Pros + "</color>" +
                               GetColorHex(explainData.arrColor[(int)ExplainData.ColorArr.Cons]) +
                               explainData.GetSkillExplain(num).Cons + "</color>" +
                               explainData.GetSkillExplain(num).Descrip;

        SetExplanationLength(explainData.GetSkillExplain(num).ExplainLength);
    }
    void EquipsImage(Hero hero, AddressableManager.EquipsImage equipsImage)
    {
        int num = (int)equipsImage + (((int)hero.Getnum) * 3);
        Debug.Log(num);
        textMeshProUGUI.text =
                               explainData.GetItemExplain(num).name + "\n" +
                               GetarrInList((int)explainData.GetItemExplain(num).quality) + "<line-height=25>" +
                               (explainData.GetItemExplain(num).quality + 4).ToString() + "</color>" + "\n</line-height>" +
                               explainData.GetItemExplain(num).HP +
                               explainData.GetItemExplain(num).ATK +
                               explainData.GetItemExplain(num).DOG +
                               explainData.GetItemExplain(num).SPEED +
                               explainData.GetItemExplain(num).ViewAngle +
                               explainData.GetItemExplain(num).ViewRange +
                               explainData.GetItemExplain(num).Accuracy +
                               explainData.GetItemExplain(num).AtkSpeed +
                               explainData.GetItemExplain(num).Range +
                               explainData.GetItemExplain(num).Stress;

        SetExplanationLength(explainData.GetItemExplain(num).ExplainLength, 5);

    }
    string TypetoHex<T>(in T type) where T : struct, Enum
    {
        return GetColorHex(explainData.arrColor[Convert.ToInt32(type)]);
    }

    string GetColorHex(Color color)
    {
        byte r = (byte)(color.r * 255);
        byte g = (byte)(color.g * 255);
        byte b = (byte)(color.b * 255);

        string asdf = "<color=#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + ">";
        return asdf;
    }
    string GetarrInList(int num)
    {
        return GetColorHex(explainData.arrColor[explainData.quality2Color[num]]);
    }

    void SetExplanationLength(int nLineCount, float add = 0)
    {
        image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, nLineCount * 24 + 16 + add);
    }
    public void SetTextMessage<T>(Hero hero, T equipsImage) where T : struct, Enum
    {
        actions[typeof(T)](hero, equipsImage);
        //   textMeshProUGUI.text = TypetoHex((ExplainData.TypeName)(Convert.ToInt32(equipsImage) + 5));

    }
}
