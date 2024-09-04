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
        actions.Add("<color=#ECE7E4>��å��...</color>");
        actions.Add("<color=#DEBA7F>�Ʒ���...</color>");
        actions.Add("<color=#F3776B>ġ����...</color>");
        actions.Add("<color=#9AD292>�����...</color>");
    }

    public void ChangeAction(ActionType type)
    {
        text.text = actions[(int)type];
        text.color = colors[(int)type];
    }


}
