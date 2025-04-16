using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerFloorButtonEvent : MonoBehaviour
{
    [SerializeField] HighLightImage highLightImage;
    [SerializeField] TextMeshProUGUI floorText;
    [SerializeField] RectTransform rectTransform;

    public void CallImage()
    {
        int buttonFloor = int.Parse(floorText.text);
        highLightImage.CallPosition(rectTransform);
        GameManager.manager.onVilligeExpeditionFloorSelect.eventAction?.Invoke(buttonFloor, Vector3.zero);
    }

}
