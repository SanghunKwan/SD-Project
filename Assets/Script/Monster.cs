using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Unit
{
    public class Monster : CUnit
    {
        Image mentalBar;
        loadingbar mentalBarScript;

        protected override IEnumerator DelayGetUI()
        {
            yield return StartCoroutine(base.DelayGetUI());

            GameObject MentalityBar = ObjectUIPool.pool.Call(ObjectUIPool.Folder.MentalBar);
            MentalityBar.transform.SetParent(copyBar.transform, false);

            mentalBar = MentalityBar.GetComponent<Image>();
            mentalBarScript = mentalBar.transform.GetChild(0).GetComponent<loadingbar>();
            if (stat is not null) mentalBarScript.GetStatus(stat, curstat, BarOffset);
        }
        protected override void CharUI()
        {
            //lv
            //stat.LV;

            //hp
            //stat.HP
            //curstat.HP

            //morale
            //stat.morale
            //curstat.morale

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
            if (asdf == true)
            {
                monsterCheck = true;
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
            GameManager.manager.HereComesNewEnermy(this);
        }

        public override void Death(Vector3 vec)
        {
            GameManager.manager.MonsterOut(this, detected);
            base.Death(vec);
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
            if (curstat.Mentality > stat.Mentality)
            {
                curstat.Mentality = stat.Mentality;
            }
            else if (curstat.Mentality <= 0)
            {
                curstat.Mentality = 0;
                GetStatusEffect(SkillData.manager.SkillInfo.skill[3].effect_IndexEnum, Vector3.zero);
                StartCoroutine(RecoverMental());
            }

            mentalBarScript.BarUpdate();
        }
        IEnumerator RecoverMental()
        {
            yield return new WaitUntil(() => unitMove.isFear);
            float time = 7 / (float)stat.Mentality;
            while (unitMove.isFear && stat.Mentality > curstat.Mentality)
            {
                curstat.Mentality++;
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

            curstat.Mentality += add;
            MentalBarRenew();
        }
    }
}