using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unit;

public class SkillData : JsonLoad
{
    public static SkillData manager;
    public SKILLS SkillInfo { get; private set; }
    public enum SkillType
    {
        BasicWeapon,
        SpecialWeapon,
        Level,
        Characteristic,
        Ambush,
        MAX
    }
    public enum Range
    {
        SemiCircle,
        Circle,
        Targetting,
        MAX
    }
    public enum EFFECTINDEX
    {
        KNOCKBACK,
        FROZEN,
        STUN,
        FEAR,

        MAX
    }
    public enum ItemSkillEffect
    {
        HEALING,
        BURN,
        Reinforcement,
        MAX
    }
    [System.Serializable]
    public class Skill
    {
        public string name;
        public SkillType type;
        public Range range;
        public int atkMultiply;
        public int outRange;
        public bool frendlyFire;
        public int[] effect_IndexEnum = new int[(int)EFFECTINDEX.MAX];
        public float durTime;
        public float rangeMultiply;
        public float firstDelay;
        public float lastDelay;
        public float coolTime;
        public int hitWeigh;
    }
    [System.Serializable]
    public class SKILLS
    {
        public Skill[] skill;
    }

    private void Start()
    {
        manager = this;

        SkillInfo = LoadData<SKILLS>("Skill_Data");
    }

    public IEnumerator MakeSkillStruct(int nIndex, WeaponComponent user, CObject target, Vector3 position)
    {
        yield return new WaitForSeconds(SkillInfo.skill[nIndex].firstDelay);
        MakeAOE(SkillInfo.skill[nIndex], user, (coll, vec) => user.Hit(coll, vec, SkillInfo.skill[nIndex], target), position);

        yield return new WaitForSeconds(SkillInfo.skill[nIndex].lastDelay);
    }
    public IEnumerator MakeSkillStructExceptTarget(int nIndex, WeaponComponent user, CObject target, Vector3 position)
    {
        yield return new WaitForSeconds(SkillInfo.skill[nIndex].firstDelay);
        MakeAOE(SkillInfo.skill[nIndex], user, (coll, vec) => user.HitExceptTarget(coll, vec, SkillInfo.skill[nIndex], target), position);

        yield return new WaitForSeconds(SkillInfo.skill[nIndex].lastDelay);
    }
    void MakeAOE(Skill skill, WeaponComponent user, System.Action<Collider, Vector3> action, in Vector3 position)
    {

        Collider col = AOEManager.manager.Call(skill.range);
        col.gameObject.SetActive(true);
        col.transform.position = position;
        col.transform.localScale = Vector3.one * skill.rangeMultiply;

        AOEComponent colComponent = col.GetComponent<AOEComponent>();
        colComponent.Init(skill, user.transform.eulerAngles.y, action);

        StartCoroutine(AOEExpire(colComponent, skill));
    }

    IEnumerator AOEExpire(AOEComponent AOE, Skill skill)
    {
        yield return new WaitForSeconds(skill.durTime);
        AOE.Return(skill.range);
    }

    [ContextMenu("asdf")]
    public void SDF()
    {
        SDF<SKILLS>();
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }
}
