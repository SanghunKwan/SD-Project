using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;

public class SettingLoader : InitObject
{
    [SerializeField] SettingWindow settingWindow;
    [SerializeField] LoadSaveManager loadSaveManager;
    PlaySetting playSetting;
    public PlaySetting PlaySetting => playSetting;


    public override void Init()
    {
        if (loadSaveManager.IsValidSettingData())
            loadSaveManager.LoadData(out playSetting);
        else
        {
            playSetting = new PlaySetting();
            loadSaveManager.OverrideSettingFile(playSetting);
        }

        settingWindow.LoadSettingData(playSetting);
        GameManager.manager.settingLoader = this;
    }
    public void SaveSetting()
    {
        loadSaveManager.OverrideSettingFile(playSetting);
    }
}
