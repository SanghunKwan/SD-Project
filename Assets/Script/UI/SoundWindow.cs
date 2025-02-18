using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWindow : InitObject
{
    [SerializeField] SoundManager soundManager;
    [SerializeField] SoundSlider[] soundSliders;
    Action<SoundType>[] soundAction = new Action<SoundType>[2];
    SettingWindow settingWindow;
    public Action noSave;
    public bool isResistered { get; private set; } = false;
    public Action Sliderinit { get; set; }


    public enum SoundType
    {
        Main,
        BGM,
        FX,
        VOICE,
        INPUTFX,
        MAX
    }

    private void Awake()
    {
        soundAction[0] = SoundCal;
        soundAction[1] = (sType) => CalAndAllo(sType, soundManager.saved[0] / 10000);

        settingWindow = transform.parent.GetComponent<SettingWindow>();
        noSave += () => isResistered = false;
    }
    public void ValueChanged(SoundType type, int nNum)
    {
        int nType = (int)type;
        soundManager.saved[nType] = nNum;
        soundAction[nType >> (nType / 2)](type);
        settingWindow.isChanged = true;

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
    private void OnEnable()
    {
        if (isResistered)
            return;

        settingWindow.setBack += noSave;
        settingWindow.setApply += () => isResistered = false;
        Sliderinit();
        isResistered = true;
    }

    public override void Init()
    {
        foreach (var item in soundSliders)
        {
            item.Init();
        }
    }
}
