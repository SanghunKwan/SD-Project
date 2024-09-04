using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionAlert : MonoBehaviour
{
    TextMeshProUGUI text;

    public enum ActionType
    {
        walking,
        training,
        healing,
        farming
    }
    [SerializeField] Color[] colors;

    List<string> actions = new List<string>();


    private void Awake()
    {
        text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        actions.Add("<color=#ECE7E4>산책중...</color>");
        actions.Add("<color=#DEBA7F>훈련중...</color>");
        actions.Add("<color=#F3776B>치료중...</color>");
        actions.Add("<color=#9AD292>농사중...</color>");
    }

    public void ChangeAction(ActionType type)
    {
        text.text = actions[(int)type];
        text.color = colors[(int)type];
    }


}
