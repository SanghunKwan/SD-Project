using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewWindow : InitObject, IWindowSet
{
    [SerializeField] Toggle[] toggles;
    [SerializeField] SettingWindow settingWindow;

    Dictionary<ToggleType, Action> changedToggles;

    public enum ToggleType
    {
        IsViewLineRenderer,
        IsHeroCanvasLayerOff,
        IsEnemyCanvasLayerOff,

        Max
    }
    public override void Init()
    {
        settingWindow.LoadSaveDataActions += LoadView;

        changedToggles = new Dictionary<ToggleType, Action>((int)ToggleType.Max);
    }
    void LoadView(SaveData.PlaySetting playSetting)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = playSetting.viewWindowSet[i];
        }

    }



    public void OnViewLineRendererToggleChanged(bool isOn)
    {
        GameManager.manager.SetViewRendererType(isOn);

        OnToggleChanged(ToggleType.IsViewLineRenderer, isOn);
    }
    public void OnToggleChanged(ToggleType type, bool isOn)
    {
        settingWindow.isChanged = true;

        CanRevert(type, () => toggles[(int)type].isOn = !isOn);
    }
    void CanRevert(ToggleType type, in Action revertAction)
    {
        if (changedToggles.ContainsKey(type)) return;

        changedToggles.Add(type, revertAction);
    }
    public void OnHeroCanvasLayerToggleChanged(bool isOn)
    {
        GameManager.manager.SetCanvasMask(isOn, 6);

        OnToggleChanged(ToggleType.IsHeroCanvasLayerOff, isOn);
    }
    public void OnEnemyCanvasLayerToggleChanged(bool isOn)
    {
        GameManager.manager.SetCanvasMask(isOn, 11);

        OnToggleChanged(ToggleType.IsEnemyCanvasLayerOff, isOn);
    }


    public void RevertValue()
    {
        foreach (var item in changedToggles.Values)
        {
            item();
        }
        changedToggles.Clear();
    }
    public void SaveValue()
    {
        changedToggles.Clear();
        OverrideSetting();
    }
    void OverrideSetting()
    {
        GameManager.manager.settingLoader.PlaySetting.viewWindowSet = Toggles2Bools();
    }
    bool[] Toggles2Bools()
    {
        bool[] settings = new bool[(int)ToggleType.Max];

        for (int i = 0; i < settings.Length; i++)
        {
            settings[i] = toggles[i].isOn;
        }
        return settings;
    }

}
