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
        villigeActions.Add("<color=#ECE7E4>산책중...</color>");
        villigeActions.Add("<color=#DEBA7F>훈련중...</color>");
        villigeActions.Add("<color=#F3776B>치료중...</color>");

        buildingActions.Add("<color=#99D191>무덤관리</color>");
        buildingActions.Add("<color=#99D191>대장간 업무</color>");
        buildingActions.Add("<color=#99D191>거주지 관리</color>");
        buildingActions.Add("<color=#99D191>거주지 관리</color>");
        buildingActions.Add("<color=#99D191>공구 관리</color>");
        buildingActions.Add("<color=#99D191>거주지 관리</color>");
        buildingActions.Add("<color=#99D191>훈련소 교관</color>");
        buildingActions.Add("<color=#99D191>거주지 관리</color>");
        buildingActions.Add("<color=#99D191>거주지 관리</color>");
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
