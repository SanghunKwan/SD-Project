using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUpgradeButton : MonoBehaviour
{
    Button button;
    BuildingSetWindow buildingWindow;
    [SerializeField] HeroUpgradeWindow heroUpgradePopupWindow;
    private void Awake()
    {
        button = GetComponent<Button>();
        buildingWindow = transform.parent.parent.GetComponent<BuildingSetWindow>();
        buildingWindow.UpgradeButtonEvent += OnHeroAllocated;
    }
    private void OnEnable()
    {
        OnHeroAllocated(false);
    }
    public void OnHeroAllocated(bool onoff)
    {
        button.interactable = onoff;

        heroUpgradePopupWindow.OnHeroDeallocated(onoff);
    }
}
