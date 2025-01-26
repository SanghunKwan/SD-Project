using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            base.Start();
            unitMove.GetStatus(curstat.Range, curstat.SPEED);
            view = GetComponent<View>();

            CheckInitCount();
        }

        public async void EyeOpen()
        {
            while (view == null)
                await Task.Yield();

            view.enabled = true;
            view.ViewAngle = curstat.ViewAngle;
            view.ViewRange = curstat.ViewRange;

            CheckInitCount();
        }

        public void SetDetected(bool asdf)
        {
            detected = asdf;
        }
        protected override void LoadDead(bool isLoaded, in Vector3 vec)
        {
            enabled = false;
            SetDetected(false);
            unitMove.Death(vec, isLoaded);
            ObjectCollider.isTrigger = true;
            gameObject.layer = 16;
            ReturnUIAfterDeath();
            StopCoroutine(LateRepeat);
            view.enabled = false;
            gameObject.AddComponent<CorpseComponent>();
            if (overlappedCorpse >= 2)
            {
                GameManager.manager.battleClearManager.CallObject(BattleClearManager.OBJECTNUM.BONEWALL, transform).gameObject.SetActive(true);
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
            GameManager.manager.onLowHPRelease.eventAction?.Invoke(gameObject.layer, transform.position);
            curstat.curHP += add;
            if (curstat.curHP > curstat.HP)
            {
                curstat.curHP = curstat.HP;
            }
            hpbarScript.BarUpdate();
        }

        public abstract void EquipOne(int equipNum);
        public abstract void EquipUpgrade(int equipNum, int level);
    }
}