using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelChange : MonoBehaviour
{
    Image levelCircle;
    TextMeshProUGUI numText;


    private void Awake()
    {
        numText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelCircle = GetComponent<Image>();
    }

    public void GetLevel(int str)
    {
        //str의 크기에 따라 levelCircle의 색깔 변화
        levelCircle.color = Color.gray;
        numText.text = str.ToString();
    }

}
