using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Unit
{
    public class Monster : CUnit
    {
        public Image mentalBar;
        loadingbar mentalBarScript;
        public MonsterMove monsterMove { get; private set; }

        protected override IEnumerator DelayGetUI()
        {
            initMaxCount = 5;
            yield return StartCoroutine(base.DelayGetUI());

            GameObject MentalityBar = ObjectUIPool.pool.Call(ObjectUIPool.Folder.MentalBar, copyBar.transform);
            mentalBar = MentalityBar.GetComponent<Image>();
            mentalBarScript = mentalBar.transform.GetChild(0).GetComponent<loadingbar>();
            if (stat is not null) mentalBarScript.GetStatus(curstat, BarOffset);

            CheckInitCount();
        }
        protected override void CharUI()
        {
            if (detected)
            {
                copyUICircle.material = materials[0];
                copyUICircle.gameObject.SetActive(true);
                copyUICircle.rectTransform.position = transform.position;
                copyUICircle.Arc = 1f;
                copyUICircle.SetAllDirty();

                copyBar.gameObject.SetActive(true);
                copyBar.rectTransform.localPosition = Data.Instance.CameratoCanvas(transform.position) + Vector3.down * BarOffset;

                mentalBar.gameObject.SetActive(true);
            }
        }

        public override void Selected(bool asdf)
        {
            if (asdf)
            {
                monsterCheck = true;
                GameManager.manager.onSelected.eventAction?.Invoke(gameObject.layer, transform.position);
            }
        }
        protected override void SelectingUICircleTurning()
        {
            if (!detected && copyUICircle != null)
            {
                if (monsterCheck || selecting())
                {
                    copyUICircle.gameObject.SetActive(true);
                    copyUICircle.rectTransform.position = transform.position;
                    MaterialChange();
                    copyUICircle.Arc = 0.3f;
                    copyUICircle.ArcRotation += 3;
                    copyUICircle.SetAllDirty();
                    if (copyUICircle.ArcRotation == 360)
                    {
                        copyUICircle.ArcRotation = 0;
                    }
                }
                else
                {
                    copyUICircle.gameObject.SetActive(false);

                }
            }
        }
        protected override void GetSelecting()
        {
            GameManager.manager.objectManager.NewObject(ObjectManager.CObjectType.Monster, this);
            GameManager.manager.HereComesNewEnermy(this);
            monsterMove = unitMove as MonsterMove;
        }

        protected override void LoadDead(bool isLoaded, in Vector3 vec)
        {
            GameManager.manager.MonsterOut(this, detected);
            GameManager.manager.objectManager.NewCorpse(this);
            base.LoadDead(isLoaded, vec);

            mentalBar.transform.SetParent(ObjectUIPool.pool.transform.GetChild((int)ObjectUIPool.Folder.MentalBar).transform, false);
            mentalBar.gameObject.SetActive(false);
            mentalBar = null;
        }
        public override void Hit(int skill)
        {
            base.Hit(skill);
            MentalBarRenew();
        }
        public void MentalBarRenew()
        {
            if (curstat.curMORALE > curstat.MORALE)
            {
                curstat.curMORALE = curstat.MORALE;
            }
            else if (curstat.curMORALE <= 0)
            {
                curstat.curMORALE = 0;
                GetStatusEffect(SkillData.manager.SkillInfo.skill[3].effect_IndexEnum, Vector3.zero);
                StartCoroutine(RecoverMental());
            }

            if (mentalBar != null)
                BarSetRenew();
        }
        void BarSetRenew()
        {
            mentalBarScript.BarUpdate();
        }
        IEnumerator RecoverMental()
        {
            yield return new WaitUntil(() => unitMove.isFear);
            float time = 7 / (float)curstat.MORALE;
            while (unitMove.isFear && curstat.MORALE > curstat.curMORALE)
            {
                curstat.curMORALE++;
                mentalBarScript.BarUpdate();
                yield return new WaitForSeconds(time);
            }
        }
        public override void SetMentality(bool isAdd, CUnit Attacker)
        {
            if (unitMove.isFear)
                return;

            int add = Attacker.curstat.Stress;

            if (!isAdd)
                add *= -1;

            curstat.curMORALE += add;
            MentalBarRenew();
        }
        protected override void DeathEvent()
        {
            GameManager.manager.MonsterDeathEvent();
        }

        public override void DetectbyHit(CUnit Attacker)
        {
            GameManager.manager.Search(ObjectManager.CObjectType.Hero, Attacker, unitMove);
        }



        public override void EquipOne(int equipNum)
        {
            //엘리트 몬스터. 장비 드랍.
            throw new System.NotImplementedException();
        }

        public override void EquipUpgrade(int equipNum, int level)
        {
            throw new System.NotImplementedException();
        }

        public override void FieldEquipUpgrade(int equipNum, int level)
        {
            throw new System.NotImplementedException();
        }
    }
}