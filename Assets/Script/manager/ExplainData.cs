using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExplainData : MonoBehaviour
{
    public enum TypeName
    {
        Melee,
        Range,
        Area,

        Armed,
        lightArmed,
        MAX
    }
    public Color[] arrColor;
    public enum ColorArr
    {
        Melee,
        Range,
        Area,
        Armed,
        lightArmed,
        
        NormalAttack,
        NormalDefence,
        Skill,
        SpecialMove,
        Damage,
        Pros,
        Cons
    }
    [Serializable]
    public class SkillExplain
    {
        public string name;
        public TypeName type;
        public string Pros;
        public string Cons;
        public string Descrip;
        public int ExplainLength;
    }
    [Serializable]
    class SkillExplans
    {
        public SkillExplain[] skillExplains = null;
    }
    SkillExplans explain;

    [Serializable]
    public class ItemExplain
    {
        public string name;
        public AddressableManager.ItemQuality quality;
        public string HP;
        public string ATK;
        public string DOG;
        public string SPEED;
        public string ViewAngle;
        public string ViewRange;
        public string Accuracy;
        public string AtkSpeed;
        public string Range;
        public string Stress;

        public int ExplainLength;
    }
    [Serializable]
    class ItemExplains
    {
        public ItemExplain[] itemExplains = null;
    }
    ItemExplains explainItem;
    public List<int> quality2Color = new List<int>();

    private void Start()
    {
        string readJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/Explan_Data.json"));
        explain = JsonUtility.FromJson(readJson, typeof(SkillExplans)) as SkillExplans;

        string readJson2 = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/ExplanItem_Data.json"));
        explainItem = JsonUtility.FromJson(readJson2, typeof(ItemExplains)) as ItemExplains;

        quality2Color.Capacity = 4;
        quality2Color.Add(0);
        quality2Color.Add(0);
        quality2Color.Add(10);
        quality2Color.Add(8);
    }
    public SkillExplain GetSkillExplain(int index)
    {
        return explain.skillExplains[index];
    }
    public ItemExplain GetItemExplain(int index)
    {
        return explainItem.itemExplains[index];
    }

}
