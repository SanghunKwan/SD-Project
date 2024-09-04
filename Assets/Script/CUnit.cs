using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Unit
{
    public enum MovingState
    {

        Run,
        Standing
    }
    public enum ActingState
    {
        None,
        Concentrate,
        Superarmor,
        Dodge
    }
    public enum Turning
    {
        Front,
        Left,
        Right
    }


    public abstract class CUnit : CObject
    {
        View view;
        protected bool monsterCheck;
        public bool detected { get; protected set; } = false;

        public int overlappedCorpse;
        public UnitMove unitMove { get; protected set; }

        [SerializeField] TypeNum num;
        public TypeNum Getnum { get { return num; } }
        [SerializeField] Species species;
        public Species GetSpecies { get { return species; } }

        protected override void Awake()
        {
            unitMove = GetComponent<UnitMove>();

            base.Awake();
        }
        protected override void Start()
        {
            stat = Data.Instance.GetInfo(species, num);
            curstat = stat.Clone(stat);
            DropInfo = DropManager.instance.GetDropInfo(stat.ID);
            if (hpbarScript)
            {
                hpbarScript.GetStatus(stat, curstat, BarOffset);
            }
            unitMove.GetStatus(stat.Range, stat.SPEED);
            view = GetComponent<View>();
        }
        public void EyeOpen()
        {
            view.enabled = true;
            view.ViewAngle = stat.ViewAngle;
            view.ViewRange = stat.ViewRange;
        }

        public void SetDetected(bool asdf)
        {
            detected = asdf;
        }
        public override void Death(Vector3 vec)
        {
            enabled = false;
            SetDetected(false);
            unitMove.Death(vec);
            ObjectCollider.isTrigger = true;
            gameObject.layer = 16;
            ReturnUIAfterDeath();
            StopCoroutine(LateRepeat);
            ItemDrop();
            view.enabled = false;
            gameObject.AddComponent<CorpseComponent>();
            if (overlappedCorpse >= 2)
            {
                StageManager.instance.CallObject(StageManager.OBJECTNUM.BONEWALL, transform).gameObject.SetActive(true);

            }
        }
        public void FindAll()
        {
            view.enabled = false;
        }

        protected override void DisableVirtual()
        {
        }
        public override void DetectbyHit(CUnit Attacker)
        {

            GameManager.manager.Search(Attacker, unitMove);
        }
        public override void Counter(CUnit unit)
        {
            unitMove.CounterAttack(unit);
        }
        public override void Hit(int skill)
        {
            base.Hit(skill);
            unitMove.Hit(skill);
        }
        public override void Dodge()
        {
            unitMove.Dodge();
        }
        protected override IEnumerator EndKnockBack(Vector3 vec, float speed)
        {
            speed *= 2;
            unitMove.TempNavStop();
            yield return base.EndKnockBack(vec, speed);
            unitMove.TempNavReStart();
        }

        protected override void Fear()
        {
            unitMove.Fear(dots[(int)SkillData.EFFECTINDEX.FEAR]);
        }

        public virtual void Recovery(int add)
        {
            curstat.HP += add;
            if (curstat.HP > stat.HP)
            {
                curstat.HP = stat.HP;
            }
            hpbarScript.BarUpdate();
        }
    }
}