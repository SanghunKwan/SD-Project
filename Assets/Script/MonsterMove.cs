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
        GameManager.manager.NewTargetting(GameManager.manager.playerCharacter, this, (cUnit) => cUnit.detected);
        SpeedCheck();
    }


    IEnumerator CryDelay()
    {

        unitState_Monster.Aggravation();

        ActionStart(0f, ActingState.Superarmor, Skill.Skill);
        Navi_Destination(transform.position);

        yield return new WaitForSeconds(2f);
        GameManager.manager.SetOtheronBattle(GameManager.manager.nPCharacter);

        yield return new WaitForSeconds(1f);
        bitArray[0] = false;
        ActionEnd(Skill.Skill);
    }
    public override void Death(Vector3 attacker_position)
    {
        base.Death(attacker_position);
        unitState_Monster.SetStandType(standType);
    }

    protected override void AddNextBehaviour()
    {
        return;
    }

    protected override void LineClear()
    {

    }
    public void MakeWay()
    {
        if (loadWait != null)
            return;
        SetTarget(GameManager.manager.GetNearest(GameManager.manager.objects, transform.position, (asdf) => true, 1000));
        isCounter = true;
        loadWait = WaitforLoad();
        StartCoroutine(loadWait);
    }
    void WayBlocked()
    {
        if (nav.path.status == NavMeshPathStatus.PathPartial)
        {
            foreach (CUnit item in GameManager.manager.nPCharacter)
            {
                MonsterMove monMove = item.unitMove as MonsterMove;
                monMove.MakeWay();

            }
        }
    }
    IEnumerator WaitforLoad()
    {
        yield return new WaitUntil(() => nav.path.status == NavMeshPathStatus.PathComplete);
        SetTarget(GameManager.manager.GetNearest(GameManager.manager.playerCharacter, transform.position, (asdf) => true, 1000));
        loadWait = null;
    }
    public override void CounterAttack(CObject gameObject)
    {
        if (!EnermySearch)
            return;

        base.CounterAttack(gameObject);

        WayBlocked();
    }
}
