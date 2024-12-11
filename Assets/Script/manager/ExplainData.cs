using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class ExplainData : JsonLoad
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
    #region 데이터 클래스
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



    #endregion
    public List<int> quality2Color = new List<int>();

    private void Start()
    {
        explain = LoadData<SkillExplans>("Explan_Data");
        explainItem = LoadData<ItemExplains>("ExplanItem_Data");

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
   

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
