using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class QuirkData : JsonLoad
{
    public static QuirkData manager;
    public QuirkS quirkInfo { get; private set; }
    public QuirkS diseaseInfo { get; private set; }


    [System.Serializable]
    public class Quirk
    {
        public int index;
        public string name;
        public int HP;
        public int ATK;
        public int DEF;
        public int DOG;
        public int SPEED;
        public int ViewAngle;
        public int ViewRange;
        public int Accuracy;
        public int AtkSpeed;
        public int Range;
        public int Mentality;
        public int Stress;

    }
    [System.Serializable]
    public class QuirkS
    {
        public Quirk[] quirks;
    }
    private void Awake()
    {
        manager = this;

        quirkInfo = LoadData<QuirkS>("Quirk_Data");
        diseaseInfo = LoadData<QuirkS>("Disease_Data");
    }
    [ContextMenu("JsonPrototype")]
    public void SDF()
    {
        SDF<QuirkS>();
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }
}
