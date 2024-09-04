using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MaterialsData : MonoBehaviour
{
    [Serializable]
    public class NeedMaterials
    {
        public int index;
        public string name;
        public int money;
        public int turn;
        public int grayNum;
        public int blackNum;    
        public int whiteNum;
        public int timberNum;
        public string desc;
    }
    [Serializable]
    public class NeedList
    {
        public NeedMaterials[] Needs;
    }

    public NeedList data;

    private void Start()
    {
        string jSonIO = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/Materials_Data.json"));

        data = JsonUtility.FromJson<NeedList>(jSonIO);
    }


    [ContextMenu("json¸¸µé±â")]
    public void SDF()
    {
        NeedList asdf = new NeedList();

        string wnth = Path.Combine(Application.dataPath, "DataTable/asdf.json");

        File.WriteAllText(wnth, JsonUtility.ToJson(asdf, true));

    }
}
