using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MaterialsData : JsonLoad
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
        data = LoadData<NeedList>("Materials_Data");
    }

    [ContextMenu("json¸¸µé±â")]
    public void SDF()
    {
        SDF<NeedList>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
