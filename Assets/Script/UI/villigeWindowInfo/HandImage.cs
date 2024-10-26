using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandImage : MonoBehaviour
{
    [SerializeField] Image comp_image;
    public Image image { get { return comp_image; } }
    [SerializeField] TextMeshProUGUI comp_text;

    public void SetText(in string HeroInfo)
    {
        comp_text.text = HeroInfo;
    }
}