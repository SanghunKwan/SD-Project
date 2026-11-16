using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;

public class PlayerNavi : MonoBehaviour
{
    public static PlayerNavi nav;


    Vector3 des_center, now_center, last_vec;
    [SerializeField] int distance_each;
    public List<Character> lists = new List<Character>();

    delegate void Action();

    delegate void Action<in T1>(T1 t1);
    Action<OrderType>[] KeytoFormation1 = new Action<OrderType>[3];
    Action<OrderType>[] KeytoFormation2 = new Action<OrderType>[3];
    Action<OrderType>[] KeytoFormation3 = new Action<OrderType>[3];
    Action<OrderType>[] KeytoFormation4 = new Action<OrderType>[3];


    delegate void Action<in T1, in T2>(T1 t1, T2 t2);
    Action<MoveType, OrderType>[] MoveFormation1 = new Action<MoveType, OrderType>[3];
    Action<MoveType, OrderType>[] MoveFormation2 = new Action<MoveType, OrderType>[3];
    Action<MoveType, OrderType>[] MoveFormation3 = new Action<MoveType, OrderType>[3];
    Action<MoveType, OrderType>[] MoveFormation4 = new Action<MoveType, OrderType>[3];


    Dictionary<string, Action<OrderType>[]> keySave = new Dictionary<string, Action<OrderType>[]>();
    Dictionary<string, Action<MoveType, OrderType>[]> formationSave = new Dictionary<string, Action<MoveType, OrderType>[]>();
    Dictionary<string, Action<OrderType>> simpleKey = new Dictionary<string, Action<OrderType>>();

    Vector3 mousePosition;

    string formationKey;

    public Dictionary<string, List<Character>> PlayerCharacter { get; private set; } = new();
    Dictionary<string, int> key2EventIndex = new Dictionary<string, int>();

    public Vector3 getCenter { get { return Now_center(); } }

    private void Awake()
    {
        nav = this;
    }
    void Start()
    {
        SetTeam();
        now_center = transform.position;

        StartDelegateSet();
        StartKeySet();
        formationKey = "Q";
    }
    void StartDelegateSet()
    {
        KeytoFormation1[0] = (OrderType) => { };
        KeytoFormation2[0] = (OrderType) => { };
        KeytoFormation3[0] = (OrderType) => { };
        KeytoFormation4[0] = (OrderType) => { };

        MoveFormation1[0] = (movetype, OrderType) => { };
        MoveFormation2[0] = (movetype, OrderType) => { };
        MoveFormation3[0] = (movetype, OrderType) => { };
        MoveFormation4[0] = (movetype, OrderType) => { };

        KeytoFormation1[1] = (OrderType) => NoFormation(OrderType);
        KeytoFormation2[1] = (OrderType) => NoFormation(OrderType);
        KeytoFormation3[1] = (OrderType) => NoFormation(OrderType);
        KeytoFormation4[1] = (OrderType) => NoFormation(OrderType);

        MoveFormation1[1] = (movetype, OrderType) => NoFormationMove(movetype, OrderType);
        MoveFormation2[1] = (movetype, OrderType) => NoFormationMove(movetype, OrderType);
        MoveFormation3[1] = (movetype, OrderType) => NoFormationMove(movetype, OrderType);
        MoveFormation4[1] = (movetype, OrderType) => NoFormationMove(movetype, OrderType);


        KeytoFormation1[2] = (OrderType) => CircleFormation(OrderType);
        KeytoFormation2[2] = (OrderType) => SquareFormation(OrderType);
        KeytoFormation3[2] = (OrderType) => LinearFormation(OrderType);
        KeytoFormation4[2] = (OrderType) => LinearFormationSecond(OrderType);

        MoveFormation1[2] = (movetype, OrderType) => CircleFormationMove(movetype, OrderType);
        MoveFormation2[2] = (movetype, OrderType) => SquareFormationMove(movetype, OrderType);
        MoveFormation3[2] = (movetype, OrderType) => LinearFormationMove(movetype, OrderType);
        MoveFormation4[2] = (movetype, OrderType) => LinearFormationSecondMove(movetype, OrderType);
    }
    void StartKeySet()
    {
        keySave.Add("Q", KeytoFormation1);
        keySave.Add("W", KeytoFormation2);
        keySave.Add("E", KeytoFormation3);
        keySave.Add("R", KeytoFormation4);

        formationSave.Add("Q", MoveFormation1);
        formationSave.Add("W", MoveFormation2);
        formationSave.Add("E", MoveFormation3);
        formationSave.Add("R", MoveFormation4);


        simpleKey.Add("S", (ordertype) => TeamStop(ordertype));
        simpleKey.Add("D", (ordertype) => TeamHolD(ordertype));
        simpleKey.Add("V", (ordertype) => TeamMove(ordertype));
        simpleKey.Add("A", (ordertype) => AttackMove(ordertype));

        key2EventIndex.Add("Q", 0);
        key2EventIndex.Add("W", 1);
        key2EventIndex.Add("E", 2);
        key2EventIndex.Add("R", 3);
    }
    void SetTeam()
    {
        PlayerCharacter.Add($"`", new());
        for (int i = 1; i < 10; i++)
        {
            PlayerCharacter.Add($"{i}", new());
        }
        PlayerCharacter.Add($"0", new());
        PlayerCharacter.Add($"-", new());
        PlayerCharacter.Add($"=", new());

    }
    public void SetTeam(UnitMove move, in string keycode)
    {
        PlayerCharacter[keycode].Add(move as Character);
    }

    void SetMoveFormation(string key)
    {
        if (lists.Count >= 1) lists[0].Formation = key;

        formationKey = key;
        GameManager.manager.onCallFormation.eventAction?.Invoke(key2EventIndex[key], getCenter);
    }
    public void Navi_Destination(Vector3 click, MoveType moveType, OrderType order)
    {
        des_center = click;

        formationSave[formationKey][Mathf.Min(lists.Count, 2)](moveType, order);
    }
    Vector3 Now_center()
    {
        Vector3 nowVec = new Vector3();
        foreach (var item in lists)
        {
            nowVec += item.transform.position;

        }
        ;
        return nowVec / lists.Count;
    }
    public void TargetSet(Collider collider, OrderType order)
    {
        foreach (var item in lists)
        {
            item.NewTarget(GameManager.manager.objectManager.GetNode(collider.gameObject), order);

            //collider.gameobject로 접근해서
            //UnitManager가 가지고 있는 linkedList에 접근해서 Node 가져오기.
            //Node에 있는 CObject를 타겟으로 지정.
            //이후에 오브젝트 파괴 시 단순 null 체크만으로도 오류 방지 가능.
            //Hero 상속 class에서도 null ? index -1 : transform.position;

            //그럴려면 오브젝트 생성 시 Node를 ArrayList로 관리하고,
            //gameobject를 Key, Node를 Value로 하는 Dictionary를 만들어야 할듯.
        }
    }
    public void HeroAdd(CUnit gameObject)
    {
        Character character = gameObject.unitMove as Character;
        Hero hero = character.cUnit as Hero;
        if (PlayerCharacter[hero.keycode].Count > 0 && character == PlayerCharacter[hero.keycode][0])
        {
            lists.Insert(0, character);
        }
        else
        {
            lists.Add(character);
        }

        formationKey = lists[0].Formation;
    }
    public void HeroClear()
    {
        lists.Clear();

    }
    public void HeroClear(CUnit gameObject, string keycode)
    {
        Character character = gameObject.unitMove as Character;
        PlayerCharacter[keycode].Remove(character);
        if (lists.Contains(character))
        {
            lists.Remove(character);
        }
    }

    void GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer("Floor");
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
        {
            mousePosition = hit.point;
            Debug.Log(mousePosition);
        }
        else
        {
            Debug.Log("raycasthit 찾지 못함.");
            mousePosition = Vector3.zero;
        }
    }
    public void MakeFormation(string key)
    {
        GetMousePoint();

        SetMoveFormation(key);
        keySave[formationKey][Mathf.Min(lists.Count, 2)](OrderType.NowAct);
    }
    void NoFormation(OrderType order)
    {
        now_center = Now_center();

        last_vec = now_center;
        lists[0].NowActorCheckCanOrder(MoveType.Move, last_vec, order);
    }
    void NoFormationMove(MoveType moveType, OrderType order)
    {
        now_center = lists[0].transform.position;

        lists[0].NowActorCheckCanOrder(moveType, des_center, order);
    }
    #region 기본 원형
    Vector3 GetCircleDestination(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - mousePosition).normalized;
        return GetCircleDestination(i, dir, looking);
    }
    Vector3 GetCircleDestination(int i, int dir, Vector3 looking)
    {
        return now_center + (Quaternion.AngleAxis((i * 360) / lists.Count * dir, Vector3.up) * looking) * distance_each * (lists.Count + 1) * 0.2f;
    }
    Vector3 GetCircleDestinationMove(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - des_center).normalized;
        return GetCircleDestinationMove(i, dir, looking);
    }
    Vector3 GetCircleDestinationMove(int i, int dir, Vector3 looking)
    {
        return des_center + (Quaternion.AngleAxis((i * 360) / lists.Count * dir, Vector3.up) * looking) * distance_each * (lists.Count + 1) * 0.2f;
    }

    void CircleFormation(OrderType order)
    {
        now_center = Now_center();
        int dir = Direction(out Vector3 looking);

        for (int i = 0; i < lists.Count; i++)
        {
            lists[i].NowActorCheckCanOrder(MoveType.Move, GetCircleDestination(i, dir, looking), order);
        }
        int Direction(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetCircleDestination(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[0].transform.position, GetCircleDestination(0, 1, looking));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetCircleDestination(lists.Count - 1, -1, looking));
            float minus2 = Vector3.Distance(lists[0].transform.position, GetCircleDestination(0, -1, looking));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }

            return 1;
        }
    }
    void CircleFormationMove(MoveType moveType, OrderType order)
    {
        now_center = Now_center();
        int dir = DirectiontoDestination(out Vector3 looking);
        Vector3 vector;

        for (int i = 0; i < lists.Count; i++)
        {
            vector = GetCircleDestinationMove(i, dir, looking);
            lists[i].NowActorCheckCanOrder(moveType, vector, order);
        }
        int DirectiontoDestination(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetCircleDestinationMove(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[0].transform.position, GetCircleDestinationMove(0, 1, looking));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetCircleDestinationMove(lists.Count - 1, -1, looking));
            float minus2 = Vector3.Distance(lists[0].transform.position, GetCircleDestinationMove(0, -1, looking));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }
            return 1;
        }
    }
    #endregion
    #region 센터한원


    Vector3 GetSquareDestination(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - mousePosition).normalized;
        return GetSquareDestination(i, dir, looking, lists.Count);
    }
    Vector3 GetSquareDestination(int i, int dir, Vector3 looking, int numCount)
    {
        float num = numCount + 1.5f;
        if (numCount.Equals(1))
            num = 0;
        return now_center + (Quaternion.AngleAxis((i * 360) / numCount * dir, Vector3.up) * looking) * distance_each * num * 0.25f;
    }
    Vector3 GetSquareDestinationMove(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - des_center).normalized;
        return GetSquareDestinationMove(i, dir, looking, lists.Count);
    }
    Vector3 GetSquareDestinationMove(int i, int dir, Vector3 looking, int numCount)
    {
        float num = numCount + 1.5f;
        if (numCount.Equals(1))
            num = 0;
        return des_center + (Quaternion.AngleAxis((i * 360) / numCount * dir, Vector3.up) * looking) * distance_each * num * 0.25f;
    }

    void SquareFormation(OrderType order)
    {
        now_center = Now_center();
        int innerCircleNum = Mathf.RoundToInt(lists.Count / 5f);
        int dir = SquareDirection(out Vector3 looking);

        for (int i = 0; i < innerCircleNum; i++)
        {
            lists[i].NowActorCheckCanOrder(MoveType.Move, GetSquareDestination(i, dir, looking, innerCircleNum), order);
        }

        for (int i = innerCircleNum; i < lists.Count; i++)
        {
            lists[i].NowActorCheckCanOrder(MoveType.Move, GetSquareDestination(i, dir, looking, lists.Count - innerCircleNum), order);
        }
        int SquareDirection(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetSquareDestination(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[innerCircleNum].transform.position, GetSquareDestination(innerCircleNum, 1, looking, lists.Count - innerCircleNum));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetSquareDestination(lists.Count - 1, -1, looking, lists.Count - innerCircleNum));
            float minus2 = Vector3.Distance(lists[innerCircleNum].transform.position, GetSquareDestination(innerCircleNum, -1, looking, lists.Count - innerCircleNum));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }

            return 1;
        }
    }
    void SquareFormationMove(MoveType moveType, OrderType order)
    {
        now_center = Now_center();
        int innerCircleNum = Mathf.RoundToInt(lists.Count / 5f);
        int dir = SquareDirectiontoDestination(out Vector3 looking);

        Vector3 vector;

        for (int i = 0; i < innerCircleNum; i++)
        {
            vector = GetSquareDestinationMove(i, dir, looking, innerCircleNum);
            lists[i].NowActorCheckCanOrder(moveType, vector, order);
        }

        for (int i = innerCircleNum; i < lists.Count; i++)
        {
            vector = GetSquareDestinationMove(i, dir, looking, lists.Count - innerCircleNum);
            lists[i].NowActorCheckCanOrder(moveType, vector, order);
        }

        int SquareDirectiontoDestination(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetSquareDestinationMove(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[1].transform.position, GetSquareDestinationMove(innerCircleNum, 1, looking, lists.Count - innerCircleNum));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetSquareDestinationMove(lists.Count - 1, -1, looking, lists.Count - innerCircleNum));
            float minus2 = Vector3.Distance(lists[1].transform.position, GetSquareDestinationMove(innerCircleNum, -1, looking, lists.Count - innerCircleNum));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }
            return 1;
        }
    }
    #endregion
    #region 선형
    Vector3 GetLinearDestination(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - mousePosition).normalized;
        return GetLinearDestination(i, dir, looking);
    }
    Vector3 GetLinearDestination(int i, int dir, Vector3 looking)
    {
        return now_center + (Quaternion.AngleAxis(90 * dir, Vector3.up) * looking) * distance_each * (i - ((float)(lists.Count - 1) / 2));
    }
    Vector3 GetLinearDestinationMove(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - des_center).normalized;
        return GetLinearDestinationMove(i, dir, looking);
    }
    Vector3 GetLinearDestinationMove(int i, int dir, Vector3 looking)
    {
        return des_center + (Quaternion.AngleAxis(90 * dir, Vector3.up) * looking) * distance_each * (i - ((float)(lists.Count - 1) / 2));
    }
    void LinearFormation(OrderType order)
    {
        now_center = Now_center();
        int dir = LinearDirection(out Vector3 looking);


        for (int i = 0; i < lists.Count; i++)
        {
            lists[i].NowActorCheckCanOrder(MoveType.Move, GetLinearDestination(i, dir, looking), order);
        }

        int LinearDirection(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestination(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[0].transform.position, GetLinearDestination(0, 1, looking));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestination(lists.Count - 1, -1, looking));
            float minus2 = Vector3.Distance(lists[0].transform.position, GetLinearDestination(0, -1, looking));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }

            return 1;
        }
    }
    void LinearFormationMove(MoveType moveType, OrderType order)
    {
        now_center = Now_center();
        int dir = LinearDirectiontoDestination(out Vector3 looking);

        Vector3 vector;

        for (int i = 0; i < lists.Count; i++)
        {
            vector = GetLinearDestinationMove(i, dir, looking);
            lists[i].NowActorCheckCanOrder(moveType, vector, order);
        }

        int LinearDirectiontoDestination(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestinationMove(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[0].transform.position, GetLinearDestinationMove(0, 1, looking));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestinationMove(lists.Count - 1, -1, looking));
            float minus2 = Vector3.Distance(lists[0].transform.position, GetLinearDestinationMove(0, -1, looking));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }

            return 1;
        }
    }

    #endregion
    #region 선형 순서 변형
    Vector3 GetLinearDestinationSecond(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - mousePosition).normalized;
        return GetLinearDestinationSecond(i, dir, looking);
    }
    Vector3 GetLinearDestinationSecond(int i, int dir, Vector3 looking)
    {
        return now_center + (Quaternion.AngleAxis(90 * dir, Vector3.up) * looking) * distance_each * (Mathf.Pow(-1, i + 1) * Mathf.CeilToInt((float)i / 2) - 0.5f * ((lists.Count + 1) % 2));
    }
    Vector3 GetLinearDestinationSecondMove(int i, int dir, out Vector3 looking)
    {
        looking = (now_center - des_center).normalized;
        return GetLinearDestinationSecondMove(i, dir, looking);
    }
    Vector3 GetLinearDestinationSecondMove(int i, int dir, Vector3 looking)
    {
        Debug.Log(Mathf.CeilToInt((float)i / 2));
        return des_center + (Quaternion.AngleAxis(90 * dir, Vector3.up) * looking) * distance_each * (Mathf.Pow(-1, i + 1) * Mathf.CeilToInt((float)i / 2) - 0.5f * ((lists.Count + 1) % 2));
    }
    void LinearFormationSecond(OrderType order)
    {
        now_center = Now_center();
        int dir = LinearDirection(out Vector3 looking);


        for (int i = 0; i < lists.Count; i++)
        {
            lists[i].NowActorCheckCanOrder(MoveType.Move, GetLinearDestinationSecond(i, dir, looking), order);
        }

        int LinearDirection(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestinationSecond(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[1].transform.position, GetLinearDestinationSecond(0, 1, looking));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestinationSecond(lists.Count - 1, -1, looking));
            float minus2 = Vector3.Distance(lists[1].transform.position, GetLinearDestinationSecond(0, -1, looking));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }

            return 1;
        }
    }
    void LinearFormationSecondMove(MoveType moveType, OrderType order)
    {
        now_center = Now_center();
        int dir = LinearDirectiontoDestination(out Vector3 looking);

        Vector3 vector;

        for (int i = 0; i < lists.Count; i++)
        {
            vector = GetLinearDestinationSecondMove(i, dir, looking);
            lists[i].NowActorCheckCanOrder(moveType, vector, order);
        }

        int LinearDirectiontoDestination(out Vector3 _looking)
        {
            float plus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestinationSecondMove(lists.Count - 1, 1, out Vector3 looking));
            _looking = looking;
            float plus2 = Vector3.Distance(lists[1].transform.position, GetLinearDestinationSecondMove(0, 1, looking));

            float minus1 = Vector3.Distance(lists[lists.Count - 1].transform.position, GetLinearDestinationSecondMove(lists.Count - 1, -1, looking));
            float minus2 = Vector3.Distance(lists[1].transform.position, GetLinearDestinationSecondMove(0, -1, looking));

            if ((plus1 + plus2) > (minus1 + minus2))
            {
                return -1;
            }

            return 1;
        }
    }
    #endregion
    #region 키보드 입력
    public void SimpleKey(string key)
    {
        simpleKey[key](OrderType.NowAct);
    }
    void TeamStop(OrderType order)
    {
        foreach (var item in lists)
        {
            item.StopHoldEnqueue(order, () => item.Stop());
        }
    }
    void TeamHolD(OrderType order)
    {
        foreach (var item in lists)
        {
            item.StopHoldEnqueue(order, () => item.ChangeHold(true));
        }
    }
    void TeamMove(OrderType order)
    {
        GetMousePoint();
        Navi_Destination(mousePosition, MoveType.Move, order);
    }
    void AttackMove(OrderType order)
    {
        if (GetMouseTarget(out RaycastHit hit))
        {
            TargetSet(hit.collider, order);
        }
        else
        {
            GetMousePoint();
            Navi_Destination(mousePosition, MoveType.Attack, order);
        }
    }
    bool GetMouseTarget(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Character", "Obstacle", "Enermy");
        if (Physics.Raycast(ray, out RaycastHit _hit, float.MaxValue, layerMask))
        {
            hit = _hit;
            return true;
        }
        hit = default;
        return false;
    }
    public void QueueOrder(string key)
    {
        simpleKey[key](OrderType.InQueue);

    }
    public virtual void SetTeam(string keycode)
    {
        List<Character> keylist = new List<Character>(lists);
        PlayerCharacter[keycode] = keylist;
    }
    #endregion
    #region 정보요청
    public void CallFormation(out string key)
    {
        key = formationKey;
    }
    #endregion
    public void GetItem(Vector3 pos, OrderType order)
    {
        if (lists.Count <= 0)
            return;

        int i = Random.Range(0, lists.Count - 1);
        lists[i].NowActorCheckCanOrder(MoveType.Move, pos, order);
    }
    public string GetEmptyTeamString()
    {
        foreach (var item in PlayerCharacter.Keys)
        {
            if (PlayerCharacter[item].Count <= 0)
                return item;
        }
        return null;
    }
    public void SmartOrder(RaycastHit hit, OrderType type)
    {
        if (lists.Count <= 0) return;

        switch (hit.collider.gameObject.layer)
        {
            case 10:
                TargetSet(hit.collider, type);
                break;

            case 15:
                GetItem(hit.collider.transform.position, type);
                break;

            default:
                Navi_Destination(hit.point, MoveType.Move, type);
                break;
        }
    }
}
