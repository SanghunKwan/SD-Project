using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingDescManager : MonoBehaviour
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

    private void Start()
    {
        string jSonIO = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/SettingDescription.json"));

        readContents = JsonUtility.FromJson<Contents>(jSonIO);
    }




    [ContextMenu("json¸¸µé±â")]
    public void SDF()
    {
        Contents asdf = new Contents();

        string wnth = Path.Combine(Application.dataPath, "DataTable/asdf.json");

        File.WriteAllText(wnth, JsonUtility.ToJson(asdf, true));

    }
}
