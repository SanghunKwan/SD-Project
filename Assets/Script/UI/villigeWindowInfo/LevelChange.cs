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
        //str�� ũ�⿡ ���� levelCircle�� ���� ��ȭ
        levelCircle.color = Color.gray;
        numText.text = str.ToString();
    }

}
