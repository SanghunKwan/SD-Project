using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unit;

//길찾기, 기본 스탠딩 애니메이션 등

public enum MoveType
{
    Move,
    Attack
}
public enum OrderType
{
    NowAct,
    InQueue,
}
public enum BitRole
{
    InAct,
    InNormalAttack,
    NormalAttackDelay,
    InSkill,
    SkillDelay,
    Stumbling
}
[RequireComponent(typeof(WeaponComponent))]
public abstract class UnitMove : MonoBehaviour
{
    public UnitState unit_State { get; protected set; }
    public Vector3 depart { get; protected set; }
    public Vector3 destination { get; set; }
    protected int movestate;
    protected int actstate;
    protected float speed;
    float length;
    float nTempSpeed;

    public delegate void Action();
    protected Action[] action;

    public LinkedListNode<CObject> targetNode;
    public CObject TargetEnemy => targetNode.Value;
    public UnitMove targetMove;
    public float targetSpeed
    {
        get
        {
            if (targetMove == default)
            {
                return 0;
            }
            return targetMove.speed;
        }
    }

    public CObject targetSave { get; protected set; }
    public CUnit cUnit { get; protected set; }

    protected int MaxSpeed;
    protected int MaxAngularSpeed = 600;

    int range;
    public BitArray bitArray = new BitArray(6, false);
    public bool isCounter = false;
    public bool isHold = false;
    public bool isLoaded = false;
    public bool EnermySearch { get; private set; }
    public bool arrive;
    public bool attackMove;
    public bool isFear { get; protected set; }
    bool canOrder = true;
    Dictionary<bool, Action> Chasing = new Dictionary<bool, Action>();

    [SerializeField] protected NavMeshAgent nav;
    protected WeaponComponent weapon;

    IEnumerator FindEnermy;
    IEnumerator React;
    IEnumerator FearCor;
    public enum Skill
    {
        NormalAttack = 1,
        Skill = 3
    }
    protected enum ORDERQUEUE
    {
        NONE = 0,
        QUEUE = 1
    }
    protected ORDERQUEUE orderCount;
    delegate void Action<in T>(T obj);
    Action<Vector3>[] MovetypetoAct;
    Action<Action>[] stopHoldOrder;
    protected delegate void Action<in T1, in T2>(T1 t1, T2 t2);
    protected Action<MoveType, Vector3>[] OrdertoReserve;
    protected Queue<Action> orderQueue = new Queue<Action>();
    protected Action nextBehaviour;



    protected virtual void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        unit_State = GetComponent<UnitState>();
        cUnit = GetComponent<CUnit>();
        weapon = GetComponent<WeaponComponent>();
    }
    public void GetStatus(int _range, int _speed)
    {
        range = _range;
        MaxSpeed = _speed;

        Debug.Log("생성 정보 : range" + range);
    }
    protected virtual void Start()
    {
        MovetypetoAct = new Action<Vector3>[]
        {
            (vec)=>{Navi_Destination(vec); },
            (vec)=>{AttackPosition(vec); }
        };
        action = new Action[]
        {
            () =>
            {
                if(nav.desiredVelocity.sqrMagnitude < 0.001f||
                (speed<0.05f && Vector3.Dot(transform.forward, nav.desiredVelocity.normalized) > 0.9f))
                    StopCheck();
                arrive = false;
                //if(gameObject.layer == 7)
                //    Debug.Log(nav.desiredVelocity);
                //stop 명령.
                //stop 시 OrderQueue에 따른 다음 명령 수행.
                //OrderQueue가 비어있으면 1초마다 주변 체크.
                //patrol 기능은 따로 구현.
            },
            () =>
            {
                //moveCheck 삭제.
                //moveCheck의 기능은 모두 명령 시 실행하도록 수정.
                if(nav.desiredVelocity.sqrMagnitude > 0.05f &&
                  (speed>0.05f || Vector3.Dot(transform.forward, nav.desiredVelocity.normalized) <= 0.9f))
                 MoveCheck();
                else
                    Arrive();
            },
        };

        OrdertoReserve = new Action<MoveType, Vector3>[]
        {
            (movetype, vec) =>
            {
                 QueueClear();
                 MoveOrAttack(movetype, vec);
            },
            (movetype, vec) =>
            {
                orderQueue.Enqueue(() =>
                {
                    MoveOrAttack(movetype, vec);
                });
            }
        };

        stopHoldOrder = new Action<Action>[2]
        {
            (method)=>{ QueueClear(); LineClear(); method();},
            (method)=>
                orderQueue.Enqueue(()=>{method();  })
        };

        Chasing.Add(false, Chase);
        Chasing.Add(true, Hold);
    }
    #region statecheck
    protected void Update()
    {
        speed = nav.velocity.sqrMagnitude;
        unit_State.TurningTimeCheck();

        action[movestate]();
        Chasing[isHold]();
    }
    //이동 시작 시 조건 변경
    //정지 시 정지 함수 호출해서 navMesh도 정지.
    //정지 조건
    //1. desired 속도가 0에 수렴.
    //3-1. 회전 중일 때 예외처리.

    //중앙에서 Update를 호출하게 할 것.
    protected bool IsTargetActive => IsTargetactive(targetNode);
    bool IsTargetactive(LinkedListNode<CObject> node)
    {
        if (targetNode == default)
            return false;

        if (targetNode.List == null)
        {
            targetNode = default;
            return false;
        }

        return true;
    }
    void MoveCheck()
    {
        movestate = (int)MovingState.Run;
        unit_State.SetState(MovingState.Run, EnermySearch);
    }
    void StopCheck()
    {
        movestate = (int)MovingState.Standing;
        unit_State.SetState(MovingState.Standing, EnermySearch);
    }
    #endregion
    #region Move
    public virtual void Navi_Destination(Vector3 vec)
    {

        BattleOff();
        depart = transform.position;
        nav.SetDestination(vec);

        if (FindEnermy != null)
        {
            StopCoroutine(FindEnermy);
        }
    }
    public void LoadDepart(Vector3 vec)
    {
        isLoaded = true;
        depart = vec;
    }
    void BattleOff()
    {
        attackMove = false;
        TargetOff();
        unit_State.FocusTarget(false);
        isCounter = true;
        SpeedCheck();
        NewOrder();
    }
    void TargetOff()
    {
        targetMove = default;
        targetNode = default;
    }
    void NewOrder()
    {
        ChangeHold(false);
    }
    public void SpeedCheck()
    {
        if (bitArray[(int)BitRole.InNormalAttack] || bitArray[(int)BitRole.InSkill])
        {
            if (InRange())
            {
                SetActionSpeed();
            }
            else
            {
                SetNaviSpeed(nTempSpeed);
            }
        }
        else
            SpeedReturn();
    }
    void SpeedReturn()
    {
        if (InRange())
        {
            SetNaviSpeed(targetSpeed);
            StartCoroutine(ReSpeedCheck());
        }
        else
        {
            SetNaviSpeed(MaxSpeed);
        }

    }

    IEnumerator ReSpeedCheck()
    {
        yield return new WaitForSeconds(0.2f);
        SpeedCheck();
    }
    #endregion
    #region Attack
    protected void Chase()
    {
        if (!IsTargetActive) return;

        if (InRange())
        {
            Targetting(out float targetangle);

            if (CanAct() && InAngle(targetangle))
            {
                if (!cUnit.detected && targetMove != default)
                    weapon.SurpriseAttack();

                else if (!bitArray[(int)Skill.NormalAttack + 1])
                    StartCoroutine(weapon.NormalAttackandDelay());

                else if (!bitArray[(int)Skill.Skill + 1])
                {
                    StartCoroutine(weapon.NormalSkillandDelay());
                }
            }
        }
        else
        {
            Navi_Enermy();

        }
    }

    bool InRange()
    {
        if (!IsTargetActive) return false;

        CObject targetEnemy = TargetEnemy;

        float radius = targetEnemy.ObjectCollider.radius;
        float distance = Vector3.Distance(gameObject.transform.position, targetEnemy.transform.position);
        return distance < range + radius;
    }
    bool InAngle(float targetangle)
    {
        return Mathf.Abs(targetangle - transform.eulerAngles.y) % 360 < 30;
    }
    void Targetting(out float targetangle)
    {
        Vector3 lookat = TargetEnemy.transform.position - transform.position;

        float angle = Mathf.Atan2(lookat.z, lookat.x);

        targetangle = 90 - angle * Mathf.Rad2Deg;
        float asdf = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetangle, Time.deltaTime * nav.angularSpeed * 0.5f);
        transform.eulerAngles = new Vector3(0, asdf, 0);
    }
    bool CanAct()
    {
        return !(bitArray[0] || bitArray[5]);
    }
    protected void Navi_Enermy()
    {
        CObject targetEnemy = TargetEnemy;

        length = range + targetEnemy.ObjectCollider.radius - targetSpeed * 0.02f - cUnit.ObjectCollider.radius * 0.1f;
        nav.SetDestination(targetEnemy.transform.position + (transform.position - targetEnemy.transform.position).normalized * length);
    }
    #endregion
    #region NewTarget
    protected void CallTargetting()
    {
        if (EnermySearch) CallTargetting(cUnit);
    }

    protected abstract void CallTargetting(CUnit gameObject);
    public void AutoTargeting(LinkedListNode<CObject> proximateEnemy)
    {
        if (AutoTargeting())
        {
            SetTarget(proximateEnemy);
            SpeedCheck();
            isCounter = false;

        }

        bool AutoTargeting()
        {
            if (!IsTargetActive && nav.enabled)
                return true;

            return false;
        }

    }
    protected abstract void AddNextBehaviour();
    public void NewTarget(LinkedListNode<CObject> node, OrderType order = OrderType.NowAct)
    {
        if (order == OrderType.NowAct)
        {
            if (!canOrder) return;

            QueueClear();
            SetNewTarget(node);
        }
        else
        {
            orderQueue.Enqueue(() =>
            {
                SetNewTarget(node);
                AddQueue(node);
            });
        }
    }
    void SetNewTarget(LinkedListNode<CObject> node)
    {
        if (node.Value == cUnit) return;

        ChangeHold(false);
        SetTarget(node, true);
        isCounter = true;
    }

    protected virtual void AddQueue(LinkedListNode<CObject> target)
    {

    }
    #endregion
    #region onBattle
    public virtual void OnBattle(bool asdf)
    {
        EnermySearch = asdf;
    }
    public void SetNaviSpeed(float Speed)
    {
        nav.speed = Speed;

    }
    public void SetTurnSpeed(bool isTimeDelay)
    {
        int turnspeed = MaxAngularSpeed;
        if (isTimeDelay)
            turnspeed *= 2;

        nav.angularSpeed = turnspeed;
    }
    public virtual void CounterAttack(CObject gameObject)
    {
        if (!(EnermySearch && canOrder)) return;

        if (!isCounter)
        {
            isCounter = true;
            SetTarget(GameManager.manager.objectManager.GetNode(gameObject));
        }
    }
    protected void SetTarget(LinkedListNode<CObject> node, bool isOrder = false)
    {
        targetNode = node;
        unit_State.FocusTarget(true);
        if (node.Value is CUnit)
        {
            CUnit targetUnit = node.Value as CUnit;
            if (!targetUnit.detected)
                GameManager.manager.onTargettingNonDetected.eventAction?.Invoke(targetUnit.gameObject.layer, targetUnit.transform.position);

            targetMove = targetUnit.unitMove;
        }

        length = range + node.Value.ObjectCollider.radius - targetSpeed * 0.2f - cUnit.ObjectCollider.radius * 0.1f;
        nav.SetDestination(transform.position);
        arrive = false;

        SpeedCheck();
        if (attackMove && !isOrder)
        {
            orderCount = ORDERQUEUE.QUEUE;
            nextBehaviour = () =>
            {
                AttackPosition();
            };

            AddNextBehaviour();
        }
    }

    public void SearchAct()
    {
        if (actstate != (int)ActingState.Superarmor && !EnermySearch)
        {
            SearchAct2();
        }
    }
    protected abstract void SearchAct2();
    #endregion
    #region BattleAnimation
    protected void ActionStart(float num, ActingState actingState, Skill skill)
    {
        bitArray[0] = true;
        bitArray[(int)skill] = true;
        bitArray[(int)skill + 1] = true;

        nTempSpeed = MaxSpeed * num;

        SetActionSpeed();
        actstate = (int)actingState;
    }
    void SetActionSpeed()
    {
        SetNaviSpeed(nTempSpeed < targetSpeed ? nTempSpeed : targetSpeed);
    }
    public void ActionEnd(Skill skill)
    {
        bitArray[(int)skill] = false;


        if (!bitArray[1] && !bitArray[3])
        {
            actstate = (int)ActingState.None;
            SpeedCheck();
        }
    }
    public virtual void Death(in Vector3 attacker_position, bool isLoaded)
    {
        Vector3 vector = attacker_position - transform.position;
        float angle = 90 - Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;

        unit_State.Hit(1, isLoaded);
        if (Mathf.Abs(angle - transform.eulerAngles.y) % 360 < 90)
        {
            unit_State.Death(false, isLoaded);

        }
        else
        {
            unit_State.Death(true, isLoaded);
        }
        enabled = false;
        Stop();
        nav.enabled = false;
        weapon.enabled = false;
        QueueClear();

        if (FearCor != null)
        {
            StopCoroutine(FearCor);
            isFear = false;
        }
    }
    public void NomalAttackAniStart(float slow, ActingState acting)
    {
        unit_State.Attack();
        targetSave = targetNode.Value;
        ActionStart(slow, acting, Skill.NormalAttack);
    }
    public void Hit(int skill)
    {
        if (actstate != (int)ActingState.Superarmor)
        {
            if (React != null) StopCoroutine(React);
            React = Reaction(skill);
            StartCoroutine(React);
        }
    }
    IEnumerator Reaction(int skill)
    {
        unit_State.Hit(skill);           //1 or 3
        bitArray[5] = true;

        yield return new WaitForSeconds(1f);
        bitArray[5] = false;
    }
    public void Dodge()
    {
        if (React != null) StopCoroutine(React);
        React = Reaction();
        StartCoroutine(React);
    }
    IEnumerator Reaction()
    {
        while (bitArray[0])
        {
            yield return null;
        }
        unit_State.Hit(0);
        bitArray[5] = true;

        yield return new WaitForSeconds(1f);
        bitArray[5] = false;
    }
    public void SkillAniStart(float slow, ActingState acting)
    {
        targetSave = targetNode.Value;
        ActionStart(slow, acting, Skill.Skill);
    }
    #endregion
    #region Stop
    public void Stop()
    {
        TargetDefault();

        nav.SetDestination(transform.position);
        attackMove = false;
    }
    public void TargetDefault()
    {
        NonBattleOff();

        if (FindEnermy != null)
        {
            StopCoroutine(FindEnermy);
        }
        FindEnermy = CallTargettingRepeat();
        StartCoroutine(FindEnermy);
    }
    void NonBattleOff()
    {
        TargetOff();
        unit_State.FocusTarget(false);
        isCounter = false;
        SpeedCheck();
        NewOrder();
    }
    IEnumerator CallTargettingRepeat()
    {
        do
        {
            yield return new WaitForSeconds(0.1f);
            CallTargetting();
            yield return new WaitForSeconds(0.1f);
        }
        while (!IsTargetActive);
    }

    protected virtual void DequeueOrder()
    {
        if (orderCount.Equals(ORDERQUEUE.QUEUE))
        {
            nextBehaviour();
            orderCount = ORDERQUEUE.NONE;
        }
        else
        {
            orderQueue.Dequeue()();
        }

    }
    #endregion
    public void StopHoldEnqueue(OrderType orderType, Action action)
    {
        stopHoldOrder[(int)orderType](action);
        destination = transform.position;
    }
    #region Hold
    protected void Hold()
    {
        if (IsTargetActive)
        {
            Targetting(out float targetangle);
            if (InRange())
            {
                if (CanAct() && InAngle(targetangle))
                {
                    if (!bitArray[(int)Skill.NormalAttack + 1])
                    {
                        StartCoroutine(weapon.NormalAttackandDelay());
                    }
                    else if (!bitArray[(int)Skill.Skill + 1])
                    {
                        StartCoroutine(weapon.NormalSkillandDelay());
                    }
                }
            }
            else
            {
                BattleContinue();
            }
        }
    }
    public void ChangeHold(bool onoff)
    {

        isHold = onoff;

        if (onoff)
        {
            attackMove = false;
            nav.SetDestination(transform.position);
            if (FindEnermy != null)
            {
                StopCoroutine(FindEnermy);
            }
            FindEnermy = CallTargettingRepeat();
            StartCoroutine(FindEnermy);
        }
    }
    #endregion
    #region Attack
    public void AttackPosition()
    {
        AttackPosition(destination);
    }

    public virtual void AttackPosition(Vector3 vec)
    {
        NonBattleOff();
        attackMove = true;

        nav.SetDestination(vec);
        depart = transform.position;

        if (FindEnermy != null)
        {
            StopCoroutine(FindEnermy);
        }
        FindEnermy = CallTargettingRepeat();
        StartCoroutine(FindEnermy);
    }
    public void MoveOrAttack(MoveType moveType, Vector3 vec)
    {
        if (!enabled || destination == vec)
            return;
        arrive = true;
        SpeedCheck();
        MovetypetoAct[(int)moveType](vec);
        destination = vec;
    }
    #endregion
    public void Damage(Skill skill)
    {
        if (enabled)
            GameManager.manager.DamageCalculate(cUnit, targetSave, (int)skill);
    }
    public void SkillEnd()
    {
        bitArray[0] = false;
        targetSave = default;
    }
    public void BattleContinue()
    {

        if (orderCount.Equals(ORDERQUEUE.QUEUE) || orderQueue.Count >= 1)
        {
            arrive = true;
            DequeueOrder();
        }
        else
        {
            Stop();
            if (isHold)
            {
                ChangeHold(true);
            }
            else
            {
                NonBattleOff();
            }
        }
        SpeedCheck();
    }
    public void NowActorReserve(MoveType moveType, Vector3 vec, OrderType orderType)
    {
        OrdertoReserve[(int)orderType](moveType, vec);
    }
    public void NowActorCheckCanOrder(MoveType moveType, Vector3 vec, OrderType orderType)
    {
        if (canOrder)
            NowActorReserve(moveType, vec, orderType);
    }
    void Arrive()
    {
        if (!IsTargetActive && !arrive && !isHold && !isFear)
        {
            arrive = true;

            TargetDefault();
            StartCoroutine(NextMove());
            MonsterArrive();
        }
    }
    protected virtual void MonsterArrive()
    {

    }
    IEnumerator NextMove()
    {
        if (orderCount == ORDERQUEUE.NONE && orderQueue.Count < 1)
            LineClear();

        while (!IsTargetActive && movestate.Equals((int)MovingState.Standing))
        {
            if (orderCount.Equals(ORDERQUEUE.QUEUE) || orderQueue.Count >= 1)
                DequeueOrder();
            yield return new WaitForSeconds(0.1f);
        }
    }
    protected virtual void QueueClear()
    {
        orderQueue.Clear();
        orderCount = ORDERQUEUE.NONE;
        //nextBehaviour = null;
    }
    protected abstract void LineClear();

    public void TempNavStop()
    {
        nav.isStopped = true;
    }
    public void TempNavReStart()
    {
        if (nav.enabled)
        {
            nav.isStopped = false;
            nav.ResetPath();
        }
    }
    public void QueueBuildingWork(AddressableManager.BuildingImage buildingType, int managerIndex)
    {
        orderQueue.Enqueue(
            () =>
            {
                //속도 감소
                SetActionSpeed();
                //애니메이션 설정
                unit_State.SetBuildingWork(true);
                unit_State.SetBuildingAnimation(buildingType, managerIndex);
                canOrder = false;
            }
            );
    }
    public void BuildingWorkEnd()
    {
        canOrder = true;
        SpeedReturn();
        unit_State.SetBuildingWork(false);
        QueueClear();
        Stop();
    }
    #region StatusEffect
    public void Fear(int ntime)
    {
        Vector3 direction;
        if (!IsTargetActive)
        {
            int angle = Random.Range(0, 360);

            direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        }
        else
        {
            direction = (transform.position - targetNode.Value.transform.position).normalized;
        }

        if (FearCor != null)
            StopCoroutine(FearCor);

        FearCor = Fearful(ntime, direction);
        StartCoroutine(FearCor);

    }

    IEnumerator Fearful(int ntime, Vector3 direction)
    {
        isFear = true;
        while (ntime > 0)
        {
            ntime--;
            cUnit.dots[(int)SkillData.EFFECTINDEX.FEAR]--;

            for (int i = 0; i < 10; i++)
            {
                Navi_Destination(transform.position + direction);
                yield return new WaitForSeconds(0.1f);
            }
        }
        isFear = false;
        arrive = true;
        CallTargetting();
    }
    #endregion
}
