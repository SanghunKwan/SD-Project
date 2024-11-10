using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingDescManager : JsonLoad
{
    [Serializable]
    public class DescContent
    {
        public string text;

        public AddressableManager.Video video;
    }
    [Serializable]
    public class Contents
    {
        public DescContent[] contents;
    }
    public Contents readContents { get; private set; }

    public override void Init()
    {
        readContents = LoadData<Contents>("SettingDescription");
    }

    [ContextMenu("json¸¸µé±â")]
    public void SDF()
    {
        SDF<Contents>();
    }


}
