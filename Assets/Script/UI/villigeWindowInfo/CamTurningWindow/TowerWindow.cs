using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerWindow : CamTuringWindow
{
    Image buildingIcon;
    TextMeshProUGUI buildingName;

    [SerializeField] AddressableManager addressableManager;
    public override void Init()
    {
        buildingIcon = transform.Find("BuildingName").GetChild(0).GetComponent<Image>();
        buildingName = buildingIcon.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetOpen(bool onoff, AddressableManager.BuildingImage type)
    {
        gameObject.SetActive(onoff);

        if (onoff)
        {
            addressableManager.GetData("Building", type, out Sprite sprite);
            buildingIcon.sprite = sprite;
        }
    }
}
