using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;
using UnityEngine.AI;

//길찾기, 기본 스탠딩 애니메이션 등


public class MonsterMove : UnitMove
{
    public Vector3 originTransform { get; set; }
    public Vector3 patrolDestination { get; set; }
    [SerializeField] GameObject anchor;
    IEnumerator monArrive;
    IEnumerator loadWait;
    UnitState_Monster unitState_Monster;
    Action[] action2;



    public WaitingTypeNum waitType;

    public UnitState_Monster.StandType standType;


    protected override void Awake()
    {
        base.Awake();
        if (!isLoaded)
            depart = originTransform = transform.position;
        unit_State.SelectSet(1);
        unitState_Monster = unit_State as UnitState_Monster;
    }
    protected override void Start()
    {
        action2 = new Action[] { Wandering, Warden, Patrol };
        MaxSpeed = 1;
        base.Start();



        if (!isLoaded && waitType.Equals(WaitingTypeNum.Patrol))
            SetPatrolPosition();

        unitState_Monster.SetStandType(standType);

        void SetPatrolPosition()
        {
            int layermask = 1 << LayerMask.NameToLayer("Floor");
            if (Physics.Raycast(anchor.transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, layermask))
            {
                patrolDestination = hit.point;
            }
        }
        bitArray[(int)Skill.Skill + 1] = true;
    }
    public bool isStandType()
    {
        if (standType.Equals(UnitState_Monster.StandType.PickupEat))
            return false;
        return true;
    }
    protected override void MonsterArrive()
    {
        Stop();
        if (monArrive != null)
            StopCoroutine(monArrive);
        monArrive = ArriveWait();
        StartCoroutine(monArrive);

    }


    public void NonBattle()
    {
        if (enabled)
            action2[(int)waitType]();
    }
    void Wandering()
    {
        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(-5f, 5f);

        Navi_Destination(originTransform + new Vector3(randomX, 0, randomY));
        SetNaviSpeed(1);
    }
    void Warden()
    {
        StartCoroutine(IEnumWarden());
    }
    IEnumerator IEnumWarden()
    {
        yield return unitState_Monster.WardenHeadWeigh();
    }
    void Patrol()
    {
        if ((transform.position - originTransform).sqrMagnitude < 1f)
        {
            Patrol(patrolDestination);
        }
        else
            Patrol(originTransform);

        void Patrol(Vector3 destination)
        {
            Navi_Destination(destination);
            SetNaviSpeed(1);
        }
    }

    IEnumerator ArriveWait()
    {
        yield return new WaitForSeconds(4);
        NonBattle();
        monArrive = null;
    }
    public override void OnBattle(bool asdf)
    {
        base.OnBattle(asdf);
        if (monArrive != null)
            StopCoroutine(monArrive);
        CallTargetting();
        unitState_Monster.SetStandType(UnitState_Monster.StandType.Basic);

        if (!asdf)
            unitState_Monster.SetStandType(standType);
    }
    protected override void SearchAct2()
    {
        if (monArrive != null)
            StopCoroutine(monArrive);
        StartCoroutine(CryDelay());
    }

    protected override void CallTargetting(CUnit gameObject)
    {
        //같은 스테이지에 있는 대상 중 가장 가까이 있는 대상 타겟팅.
        GameManager.manager.NewTargetting(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero], this);
        SpeedCheck();
    }


    IEnumerator CryDelay()
    {

        unitState_Monster.Aggravation();

        ActionStart(0f, ActingState.Superarmor, Skill.Skill);
        Navi_Destination(transform.position);

        yield return new WaitForSeconds(2f);
        GameManager.manager.SetOtheronBattle(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster]);
        GameManager.manager.onCry.eventAction?.Invoke(gameObject.layer, transform.position);

        yield return new WaitForSeconds(1f);
        bitArray[0] = false;
        ActionEnd(Skill.Skill);
    }
    public override void Death(in Vector3 attacker_position, bool isLoaded)
    {
        base.Death(attacker_position, isLoaded);
        unitState_Monster.SetStandType(standType);
    }

    protected override void AddNextBehaviour()
    {
        return;
    }

    protected override void LineClear()
    {

    }



    public override void CounterAttack(CObject gameObject)
    {
        if (!EnermySearch)
            return;

        base.CounterAttack(gameObject);

        WayBlocked();
    }
    void WayBlocked()
    {
        if (nav.path.status == NavMeshPathStatus.PathPartial)
        {
            foreach (CUnit item in GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster])
            {
                MonsterMove monMove = item.unitMove as MonsterMove;
                monMove.MakeWay();

            }
        }
    }
    public void MakeWay()
    {
        if (loadWait != null) return;

        LinkedListNode<CObject> nearestNode = GameManager.manager.objectManager.GetNode(GameManager.manager.GetNearestNDetected(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.FieldObject], transform.position, float.MaxValue));
        SetTarget(nearestNode);
        isCounter = true;
        loadWait = WaitforLoad();
        StartCoroutine(loadWait);
    }
    IEnumerator WaitforLoad()
    {
        yield return new WaitUntil(() => nav.path.status == NavMeshPathStatus.PathComplete);
        LinkedListNode<CObject> nearestNode = GameManager.manager.objectManager.GetNode(GameManager.manager.GetNearestNDetected(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.FieldObject], transform.position, float.MaxValue));
        SetTarget(nearestNode);
        loadWait = null;
    }
}
