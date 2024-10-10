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
        buildingWork
    }
    [SerializeField] Color[] colors;

    List<string> villigeActions = new List<string>();
    List<string> buildingActions = new List<string>();


    private void Awake()
    {
        text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        villigeActions.Add("<color=#ECE7E4>��å��...</color>");
        villigeActions.Add("<color=#DEBA7F>�Ʒ���...</color>");
        villigeActions.Add("<color=#F3776B>ġ����...</color>");

        buildingActions.Add("<color=#99D191>��������</color>");
        buildingActions.Add("<color=#99D191>���尣 ����</color>");
        buildingActions.Add("<color=#99D191>������ ����</color>");
        buildingActions.Add("<color=#99D191>������ ����</color>");
        buildingActions.Add("<color=#99D191>���� ����</color>");
        buildingActions.Add("<color=#99D191>������ ����</color>");
        buildingActions.Add("<color=#99D191>�Ʒü� ����</color>");
        buildingActions.Add("<color=#99D191>������ ����</color>");
        buildingActions.Add("<color=#99D191>������ ����</color>");
    }

    public void ChangeAction(ActionType type, AddressableManager.BuildingImage buildingAction)
    {
        if (type == ActionType.buildingWork)
        {
            text.text = buildingActions[(int)buildingAction];
        }
        else
        {
            text.text = villigeActions[(int)type];
            text.color = colors[(int)type];
        }
    }



}
