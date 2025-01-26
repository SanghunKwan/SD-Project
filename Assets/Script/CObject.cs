using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.AI;

namespace Unit
{
    public class CObject : MonoBehaviour
    {
        public int[] dots { get; protected set; } = new int[(int)SkillData.EFFECTINDEX.MAX];
        public Vector3 dotsDirection { get; protected set; }
        public int id { get { return ID; } }
        protected int initCount;
        protected int initMaxCount = 2;
        public unit_status stat { get; protected set; }
        public unit_status curstat { get; protected set; }
        public bool selected { get; protected set; } = false;
        public bool drag { get; private set; } = false;
        public CapsuleCollider ObjectCollider { get; protected set; }
        public float uiradius { get; protected set; }
        public float uiheight { get; protected set; }

        public DropInfo DropInfo { get; protected set; }

        Vector2 nowPosition;
        protected Image copyBar;
        protected UICircle copyUICircle;
        private NavMeshObstacle obstacle;

        [SerializeField] protected Material[] materials = new Material[3];

        [SerializeField] bool mouseOnCollider = false;
        [SerializeField] protected int CirclePad = 0;
        [SerializeField] protected int ID = 100;
        [SerializeField] protected int BarOffset = 30;

        protected float charactertoUImultiply = 100;
        private float maxX, maxY, minX, minY;
        protected Transform HPCanvas;
        Transform CircleCanvas;
        protected loadingbar hpbarScript;

        protected IEnumerator LateRepeat;
        IEnumerator CorEndKnockBack;

        protected List<ILayoutElement> texts = new List<ILayoutElement>();

        public delegate void refAction(ref unit_status stat);
        public refAction OnUICompleteAction { get; set; }
        public System.Action OnInitEnd { get; set; }


        protected virtual void Awake()
        {
            HPCanvas = GameObject.FindGameObjectWithTag("CanvasWorld").transform;
            CircleCanvas = GameObject.FindGameObjectWithTag("Canvas").transform;
            ObjectCollider = GetComponent<CapsuleCollider>();
            GetColliderSize(ObjectCollider);
        }

        void OnEnable()
        {
            StartCoroutine(DelayGetUI());
            StartCoroutine(DelayRegist());

            LateRepeat = CorLateUpdate();
            StartCoroutine(LateRepeat);
        }
        protected virtual IEnumerator DelayGetUI()
        {

            while (ObjectUIPool.isReady.Equals(false))
            {
                yield return null;
            }
            GameObject barObjct = ObjectUIPool.pool.Call(ObjectUIPool.Folder.HPBar);
            barObjct.transform.SetParent(HPCanvas, false);

            GameObject circleObjct = ObjectUIPool.pool.Call(ObjectUIPool.Folder.UICircle);
            circleObjct.transform.SetParent(CircleCanvas, false);

            copyBar = barObjct.GetComponent<Image>();
            hpbarScript = copyBar.transform.GetChild(0).GetComponent<loadingbar>();

            if (stat is not null)
                hpbarScript.GetStatus(curstat, BarOffset);

            copyUICircle = circleObjct.GetComponent<UICircle>();

            copyUICircle.Padding = CirclePad;
            CheckInitCount();
        }
        IEnumerator DelayRegist()
        {
            while (GameManager.isReady.Equals(false))
            {
                yield return null;
            }
            GetSelecting();
        }

        protected virtual void Start()
        {
            //start가 delayUI보다 늦게 실행되서 버그가 생김.
            if (stat == null)
            {
                stat = Data.Instance.GetInfo(ID);
            }
            if (curstat == null)
            {
                curstat = new unit_status();
                curstat.Clone(stat);
                curstat.curHP = curstat.HP;
                curstat.curMORALE = curstat.MORALE;
            }
            unit_status tempStat = curstat;

            OnUICompleteAction?.Invoke(ref tempStat);
            OnUICompleteAction = null;

            DropInfo = DropManager.instance.GetDropInfo(ID);
            if (hpbarScript)
            {
                hpbarScript.GetStatus(curstat, BarOffset);
            }
            CheckInitCount();
        }
        protected void CheckInitCount()
        {
            initCount++;

            if (initCount == initMaxCount)
            {
                GameManager.manager.CheckObjectLoadComplete();
                Debug.Log("name : " + name + "     count : " + initCount);
            }
        }
        protected IEnumerator CorLateUpdate()
        {
            yield return new WaitUntil(() => copyUICircle != null);
            while (true)
            {
                CharUI();
                SetNowPosition();
                SelectingUICircleTurning();

                yield return null;
            }
        }
        void OnDisable()
        {
            ObjectCollider.enabled = true;
            selected = false;
            drag = false;
            mouseOnCollider = false;
        }
        protected virtual void DisableVirtual()
        {
            obstacle.enabled = true;
        }
        protected void GetColliderSize(CapsuleCollider collider)
        {
            uiradius = collider.radius * charactertoUImultiply;
            uiheight = collider.height * charactertoUImultiply * 0.4f;
        }

        protected virtual void GetSelecting()
        {
            GameManager.manager.HereComesNewObject(this);
        }
        protected void SetNowPosition()
        {
            nowPosition = Camera.main.WorldToScreenPoint(transform.position);

        }
        public void DragBoxCollide(float[] rec, bool dragging)
        {
            drag = dragging;
            maxX = rec[1] + uiradius;
            maxY = rec[3];
            minX = rec[0] - uiradius;
            minY = rec[2] - uiheight;
        }
        public bool selecting()
        {
            return mouseOnCollider ||
                   drag
                && nowPosition.x <= maxX && nowPosition.x >= minX
                && nowPosition.y <= maxY && nowPosition.y >= minY;

        }
        public void DragEnd(bool ctrl = true, bool shift = false)
        {
            Selected((selecting() || shift || ctrl) && !(selecting() && ctrl));
            drag = false;
        }
        public virtual void Selected(bool asdf)
        {
            selected = asdf;
            copyBar.gameObject.SetActive(asdf);
            copyUICircle.gameObject.SetActive(asdf);

            if (asdf)
                GameManager.manager.onSelected.eventAction?.Invoke(gameObject.layer, transform.position);
        }
        protected void MaterialChange()
        {
            copyUICircle.material = materials[2];
        }
        protected virtual void CharUI()
        {
            if (selected)
            {
                copyUICircle.material = materials[1];
                copyUICircle.rectTransform.position = transform.position;
                copyUICircle.Arc = 1f;
                copyUICircle.SetAllDirty();

                copyBar.rectTransform.localPosition = Data.Instance.CameratoCanvas(transform.position) + Vector3.down * BarOffset;
            }
            else if (copyBar.gameObject.activeSelf)
            {
                copyBar.rectTransform.localPosition = Data.Instance.CameratoCanvas(transform.position) + Vector3.down * BarOffset;
            }
        }
        protected virtual void SelectingUICircleTurning()
        {
            if (!selected)
            {
                if (selecting())
                {
                    copyUICircle.gameObject.SetActive(true);
                    copyUICircle.rectTransform.position = transform.position;
                    MaterialChange();
                    copyUICircle.Arc = 0.3f;
                    copyUICircle.ArcRotation += 3;
                    copyUICircle.SetAllDirty();
                }
                else
                {
                    copyUICircle.gameObject.SetActive(false);
                }

                if (copyUICircle.ArcRotation == 360)
                {
                    copyUICircle.ArcRotation = 0;
                }
            }
        }


        private void OnMouseEnter()
        {
            mouseOnCollider = true;
        }
        private void OnMouseExit()
        {
            mouseOnCollider = false;
        }
        public virtual void PrintMiss()
        {
            Text text = InputEffect.e.PrintEffect2(transform.position + Vector3.up * 0.5f * texts.Count);
            CorNesting(text);
        }
        public virtual void PrintDodge()
        {
            Text text = InputEffect.e.PrintEffect2(transform.position + Vector3.up * 0.5f * texts.Count, "회피");
            CorNesting(text);
        }
        public virtual void PrintDamage(int damage)
        {
            Text text = InputEffect.e.PrintEffect2(transform.position + Vector3.up * 0.5f * texts.Count, damage.ToString());
            CorNesting(text);
        }
        protected void CorNesting(ILayoutElement text)
        {
            StartCoroutine(TextNesting(text));
        }
        protected IEnumerator TextNesting(ILayoutElement text)
        {
            texts.Add(text);
            yield return new WaitForSeconds(0.5f);
            texts.RemoveAt(0);
        }
        public void Death(Vector3 vec)
        {
            ItemDrop();
            LoadDead();
        }
        protected virtual void LoadDead(bool isLoaded = false, in Vector3 vec = new Vector3())
        {
            GameManager.manager.ObjectOut(this);
            GetComponent<Animator>().SetTrigger("Death");
            StartCoroutine(Delete());
            ReturnUIAfterDeath();
            StopCoroutine(LateRepeat);
        }
        public void DelayAfterResigter()
        {
            OnInitEnd += () => LoadDead(true);
        }
        protected void ReturnUIAfterDeath()
        {
            copyBar.transform.SetParent(ObjectUIPool.pool.transform.GetChild(0).transform, false);
            copyUICircle.transform.SetParent(ObjectUIPool.pool.transform.GetChild(1).transform, false);
            copyBar.gameObject.SetActive(false);
            copyUICircle.gameObject.SetActive(false);
            copyBar = null;
            copyUICircle = null;
        }
        public virtual void DetectbyHit(CUnit Attacker)
        {
        }
        public virtual void Counter(CUnit unit)
        {
            //피격 시 대응
            //가시덤불 등 특별한 오브젝트만 작동.
        }
        public virtual void Hit(int skill)
        {
            hpbarScript.BarUpdate();
            if (curstat.curHP < curstat.HP * 0.3f)
                GameManager.manager.onLowHp.eventAction?.Invoke(gameObject.layer, transform.position);
            //피격 시 리액션
        }
        public virtual void Dodge()
        {
            //회피
        }
        IEnumerator Delete()
        {
            ObjectCollider.enabled = false;
            yield return new WaitForSeconds(1.5f);
            obstacle = GetComponent<NavMeshObstacle>();
            obstacle.enabled = false;
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);

        }
        protected void ItemDrop()
        {
            StartCoroutine(DropRepeatAndDelay());


        }
        IEnumerator DropRepeatAndDelay()
        {
            for (int i = 0; i < DropInfo.MaxNum; i++)
            {
                if (Random.Range(0, 100) > 100 - DropInfo.Percentage)
                {
                    GameObject item = DropManager.instance.pool.CallItem((int)DropInfo.material);
                    item.SetActive(true);

                    item.transform.position = transform.position
                        + Quaternion.Euler(0, 360 * i / DropInfo.MaxNum, 0) * Vector3.right * (ObjectCollider.radius + 0.5f);
                    item.transform.Rotate(0, 360 * i / DropInfo.MaxNum, 0);

                    yield return new WaitForSeconds(0.1f);
                }

            }
            if (Random.Range(0, 100) > 100 - DropInfo.PercentageEquips)
            {
                //무기 드랍 코드 수정 필요
                GameObject item = DropManager.instance.pool.CallItem((int)Materials.Weapon);
                item.SetActive(true);

                item.transform.position = transform.position;

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                //소모품 드랍 코드 수정 필요
                GameObject item = DropManager.instance.pool.CallItem(Random.Range((int)Materials.Berry, (int)Materials.GreenFruit + 1));
                item.SetActive(true);

                item.transform.position = transform.position;

                yield return new WaitForSeconds(0.1f);
            }
        }
        public void GetStatusEffect(int[] ints, Vector3 vec)
        {
            for (int i = 0; i < (int)SkillData.EFFECTINDEX.MAX; i++)
            {
                dots[i] += ints[i];
            }

            if (ints[(int)SkillData.EFFECTINDEX.KNOCKBACK] > 0)
            {

                float StartVel = dots[(int)SkillData.EFFECTINDEX.KNOCKBACK] * 2 / 0.7f;

                dotsDirection = (transform.position - vec).normalized;

                if (CorEndKnockBack != null)
                {
                    StopCoroutine(CorEndKnockBack);
                }

                CorEndKnockBack = EndKnockBack(dotsDirection, StartVel);
                StartCoroutine(CorEndKnockBack);
            }
            if (ints[(int)SkillData.EFFECTINDEX.FEAR] > 0)
            {
                Fear();
            }
        }
        protected virtual void Fear()
        {
            return;
        }
        protected virtual IEnumerator EndKnockBack(Vector3 vec, float speed)
        {
            speed /= 2;
            while (speed > 0)
            {
                transform.position += vec * speed / 30;
                speed += -4.1f / 30;
                yield return new WaitForFixedUpdate();
            }

            dots[(int)SkillData.EFFECTINDEX.KNOCKBACK] = 0;
            CorEndKnockBack = null;
        }

        public virtual void SetMentality(bool isAdd, CUnit Attacker)
        {

        }
    }
}