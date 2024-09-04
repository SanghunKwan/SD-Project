using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandImage : MonoBehaviour
{
    public Image image { get; private set; }
    TextMeshProUGUI text;
    public void Init()
    {
        image = GetComponent<Image>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void SetText(in string HeroInfo)
    {
        text.text = HeroInfo;
    }

}
