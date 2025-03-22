using System;
using System.Collections;
using System.Collections.Generic;
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
        public NeedMaterials[] NeedsInUpgrade;
        public NeedMaterials[] NeedsInSkillUpgrade;

        public NeedMaterials[][] needsArray;

        public void Init()
        {
            needsArray = new NeedMaterials[][] { Needs, NeedsInUpgrade, NeedsInSkillUpgrade };
        }
    }

    public NeedList data { get; private set; }

    private void Start()
    {
        data = LoadData<NeedList>("Materials_Data");
        data.Init();
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
