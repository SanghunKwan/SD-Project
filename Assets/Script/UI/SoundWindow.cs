using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWindow : InitObject, IWindowSet
{
    [SerializeField] SoundManager soundManager;
    [SerializeField] SoundSlider[] soundSliders;
    SettingWindow settingWindow;

    Action<SoundType>[] soundAction = new Action<SoundType>[2];

    Dictionary<SoundType, Action> changedValues;

    public enum SoundType
    {
        Main,
        BGM,
        FX,
        VOICE,
        INPUTFX,
        MAX
    }

    public override void Init()
    {
        changedValues = new Dictionary<SoundType, Action>((int)SoundType.MAX);

        soundAction[0] = SoundCal;
        soundAction[1] = (sType) => CalAndAllo(sType, soundManager.saved[0] / 10000);

        foreach (var item in soundSliders)
        {
            item.Init();
        }

        settingWindow = transform.parent.GetComponent<SettingWindow>();
        settingWindow.LoadSaveDataActions += LoadSaveDataSound;
    }
    void LoadSaveDataSound(SaveData.PlaySetting playSetting)
    {
        for (int i = 0; i < soundSliders.Length; i++)
        {
            soundSliders[i].InputValue(playSetting.soundWindowSet[i]);
        }

        changedValues.Clear();
    }
    public void ValueChanged(SoundType type, int nNum)
    {
        int nType = (int)type;
        soundManager.saved[nType] = nNum;
        soundAction[nType >> (nType / 2)](type);
        settingWindow.isChanged = true;

        CanRevert(type, () => soundSliders[nType].NoSaveRevertValue());

    }
    void SoundCal(SoundType type)
    {
        float cal = soundManager.saved[(int)SoundType.Main] / 10000;

        int max = (int)SoundType.MAX;
        for (int i = 1; i < max; i++)
        {
            CalAndAllo((SoundType)i, cal);
        }
    }
    void CalAndAllo(SoundType type, float cal)
    {
        float value = soundManager.saved[(int)type] * cal;
        soundManager.VolumeChange(type, value);
    }



    public void SaveValue()
    {
        SaveData.PlaySetting settings = GameManager.manager.settingLoader.PlaySetting;
        foreach (var item in changedValues.Keys)
        {
            soundSliders[(int)item].SaveValue(settings);
        }

        changedValues.Clear();
    }

    public void RevertValue()
    {
        foreach (var item in changedValues.Values)
        {
            item();
        }

        changedValues.Clear();
    }

    void CanRevert(SoundType type, in Action revertAction)
    {
        if (changedValues.ContainsKey(type)) return;

        changedValues.Add(type, revertAction);
    }
}
