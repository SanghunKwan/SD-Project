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
        public int stageIndex { get; set; }
        public unit_status stat { get; protected set; }
        public unit_status curstat { get; protected set; }
        public bool selected { get; protected set; } = false;
        public bool drag { get; private set; } = false;
        public CapsuleCollider ObjectCollider { get; protected set; }
        public float uiradius { get; protected set; }
        public float uiheight { get; protected set; }

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
        protected loadingbar hpbarScript;

        protected IEnumerator LateRepeat;
        IEnumerator CorEndKnockBack;

        protected List<ILayoutElement> texts = new List<ILayoutElement>();

        public delegate void refAction(ref unit_status stat);
        public refAction OnUICompleteAction { get; set; }
        public System.Action OnInitEnd { get; set; }


        protected virtual void Awake()
        {
            ObjectCollider = GetComponent<CapsuleCollider>();
            GetColliderSize(ObjectCollider);
        }

        void OnEnable()
        {
            StartCoroutine(DelayGetUI());
            StartCoroutine(DelayRegist());

            LateRepeat = CorLateUpdate();
            StartCoroutine(LateRepeat);
            stageIndex = transform.parent.GetSiblingIndex();
        }
        protected virtual IEnumerator DelayGetUI()
        {

            while (ObjectUIPool.isReady.Equals(false))
            {
                yield return null;
            }
            copyBar = ObjectUIPool.pool.Call(ObjectUIPool.Folder.HPBar, ObjectUIPool.UICanvasType.UpperCanvas)
                      .GetComponent<Image>();
            hpbarScript = copyBar.transform.GetChild(0).GetComponent<loadingbar>();

            if (stat is not null)
                hpbarScript.GetStatus(curstat, BarOffset);

            copyUICircle = ObjectUIPool.pool.Call(ObjectUIPool.Folder.UICircle, ObjectUIPool.UICanvasType.GroundCanvas)
                           .GetComponent<UICircle>();

            copyUICircle.Padding = CirclePad;
            CheckInitCount();
            //copyBar, hpBarScript, copyUICircle 리턴.
        }
        IEnumerator DelayRegist()
        {
            while (GameManager.isReady.Equals(false) || GameManager.manager.battleClearManager == null)
            {
                yield return null;
            }
            GetSelecting();
        }

        protected virtual void Start()
        {
            //start가 delayUI보다 늦게 실행되서 버그가 생김.
            if (stat == null)
                stat = Data.Instance.GetInfo(ID);

            if (curstat == null)
            {
                curstat = new unit_status();
                curstat.Clone(stat);
                curstat.NAME = name;
                curstat.curHP = curstat.HP;
                curstat.curMORALE = curstat.MORALE;
            }
            unit_status tempStat = curstat;

            OnUICompleteAction?.Invoke(ref tempStat);
            OnUICompleteAction = null;

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

            //copyBar, hpBarScript, copyUICircle 리턴.
            ReturnUIAfterDeath();
        }
        protected void GetColliderSize(CapsuleCollider collider)
        {
            uiradius = collider.radius * charactertoUImultiply;
            uiheight = collider.height * charactertoUImultiply * 0.4f;
        }

        protected virtual void GetSelecting()
        {
            GameManager.manager.objectManager.NewObject(ObjectManager.CObjectType.FieldObject, this);
        }
        protected void SetNowPosition()
        {
            nowPosition = Camera.main.WorldToScreenPoint(transform.position);

        }
        public void DragBoxCollide(in float[] rec, bool dragging)
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
                   (drag
                && nowPosition.x <= maxX && nowPosition.x >= minX
                && nowPosition.y <= maxY && nowPosition.y >= minY);
        }
        public bool DragEnd(bool ctrl, bool shift)
        {
            Selected((selecting() || shift || ctrl) && !(selecting() && ctrl));
            drag = false;
            return selected;
        }
        public void DragFalse()
        {
            Selected(false);
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
            DeathEvent();
        }

        protected virtual void LoadDead(bool isLoaded = false, in Vector3 vec = new Vector3())
        {
            GameManager.manager.objectManager.OutObject(ObjectManager.CObjectType.FieldObject, gameObject);
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
            if (!gameObject.scene.isLoaded ||
                copyBar == null || copyUICircle == null) return;

            ObjectUIPool.pool.BackPooling(copyBar.gameObject, ObjectUIPool.Folder.HPBar);
            ObjectUIPool.pool.BackPooling(copyUICircle.gameObject, ObjectUIPool.Folder.UICircle);
            copyBar = null;
            copyUICircle = null;
        }
        protected virtual void DeathEvent()
        {

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
            Destroy(gameObject);
        }
        protected void ItemDrop()
        {
            ObjectManager manager = GameManager.manager.objectManager;
            //아이템 데이터 생성
            SaveData.YetDroppedItem items =
            manager.CreateItemList(DropManager.instance.GetDropInfo(ID), transform.position, ObjectCollider.radius, stageIndex);
            manager.AddYetDroppedItem(items);
            //아이템 생성 후 아이템 데이터 삭제
            DropManager.instance.pool.CallItems(items);
        }

        public void GetStatusEffect(int[] ints, Vector3 vec)
        {
            if (!enabled) return;

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