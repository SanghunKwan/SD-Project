using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unit;
using UnityEditor;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [SerializeField] PlayerNavi playerNavi;
    [SerializeField] MonNavi monNavi;

    public List<CUnit> nPCharacter { get; private set; } = new();
    public List<CObject> objects { get; private set; } = new();
    public List<CUnit> playerCharacter { get; private set; } = new List<CUnit>();
    public int playerDetected;
    public int nPCDetected;
    int objectCount;
    int keyDictionaryCount;

    public float[] rec { get; private set; } = new float[4];

    public static bool isReady { get; private set; }


    public Action[] timeUIEvent = new Action[2];
    public Action<Vector3> screenMove { get; set; } = (screenMove) => { };


    Dictionary<string, string> keyboardConverter = new Dictionary<string, string>();
    public KeyCode shiftCode { get; private set; } = KeyCode.LeftShift;

    Action[] inputSpace = new Action[3];
    Action[] inputSpaceUp = new Action[3];

    public Action callConstructionUI;
    public Action onBattleClearManagerRegistered { get; set; }
    #region actionEvent
    public class ActionEvent
    {
        public Action<int, Vector3> eventAction;
    }
    public ActionEvent onCry { get; private set; } = new ActionEvent();
    public ActionEvent onAttack { get; private set; } = new ActionEvent();
    public ActionEvent onBackAttack { get; private set; } = new ActionEvent();
    public ActionEvent onSkill { get; private set; } = new ActionEvent();
    public ActionEvent onLowHp { get; private set; } = new ActionEvent();
    public ActionEvent onDie { get; private set; } = new ActionEvent();
    public ActionEvent onSelected { get; private set; } = new ActionEvent();
    public ActionEvent onItemUse { get; private set; } = new ActionEvent();
    public ActionEvent onRegroup { get; private set; } = new ActionEvent();
    public ActionEvent onCallgroup { get; private set; } = new ActionEvent();
    public ActionEvent onLowHPRelease { get; private set; } = new ActionEvent();
    public ActionEvent onPlayerEnterStage { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingScroll { get; private set; } = new ActionEvent();
    public ActionEvent onEffectedOtherEvent { get; private set; } = new ActionEvent();
    public ActionEvent onTargettingNonDetected { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingChoosed{ get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingStartConstruction { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingCompleteConstruction { get; private set; } = new ActionEvent();
    #endregion
    public PointerEventData pointerEventData { get; set; }

    #region 하위 managers
    public SoundManager soundManager { get; set; }
    public WindowManager windowManager { get; set; }
    public StorageManager storageManager { get; set; }
    public BattleClearManager battleClearManager { get; private set; }
    public QuestManager questManager { get; set; }
    #endregion

    private void Awake()
    {
        manager = this;
        heapList.Add(new UnitDistance());

        isReady = true;

        inputSpace[0] = () => { };
        inputSpace[1] = TimeDelay;
        inputSpace[2] = VilligeBuildingScroll;

        inputSpaceUp[0] = () => { };
        inputSpaceUp[1] = TimeDelayEnd;
        inputSpaceUp[2] = () => { };



        DefaultKeyboardConverter();

        onCry = new ActionEvent();
        onAttack = new ActionEvent();
        onBackAttack = new ActionEvent();
        onSkill = new ActionEvent();
        onLowHp = new ActionEvent();
        onDie = new ActionEvent();
        onSelected = new ActionEvent();
    }
    void DefaultKeyboardConverter()
    {
        ConverterSame("`");
        ConverterSame("1");
        ConverterSame("2");
        ConverterSame("3");
        ConverterSame("4");
        ConverterSame("5");
        ConverterSame("6");
        ConverterSame("7");
        ConverterSame("8");
        ConverterSame("9");
        ConverterSame("0");
        ConverterSame("-");
        ConverterSame("=");

        ConverterSame("A");
        ConverterSame("S");
        ConverterSame("D");
        ConverterSame("V");

        ConverterSame("Q");
        ConverterSame("W");
        ConverterSame("E");
        ConverterSame("R");
    }
    void ConverterSame(in string str)
    {
        keyboardConverter.Add(str, str);
    }
    #region 오브젝트 활성화/비활성화 체크
    public void HereComesNewChallenger(CUnit unit, string keycode)
    {
        playerCharacter.Add(unit);

        playerNavi.SetTeam(unit.unitMove, keycode);

        foreach (CUnit item in nPCharacter)
        {
            item.EyeOpen();
        }

        if (nPCharacter.Count > nPCDetected)
            unit.EyeOpen();


    }
    public void HereComesNewEnermy(CUnit gameObject)
    {
        nPCharacter.Add(gameObject);
        monNavi.MonsterAdd(gameObject);

        foreach (CUnit unit in playerCharacter)
        {
            unit.EyeOpen();
        }

        if (playerCharacter.Count > playerDetected)
            gameObject.EyeOpen();
    }
    public void ChallengerOut(CUnit gameObject, string keycode, bool detected)
    {
        playerCharacter.Remove(gameObject);
        playerNavi.HeroClear(gameObject, keycode);
        TargetOutMonster(gameObject);

        if (detected)
            playerDetected--;

        MonsteronBattleoff();

        foreach (Monster item in nPCharacter)
        {
            item.curstat.Mentality = item.stat.Mentality;
            item.MentalBarRenew();
        }

    }
    public void MonsterOut(CUnit gameObject, bool detected)
    {
        nPCharacter.Remove(gameObject);
        monNavi.MonsterRemove(gameObject);
        TargetOutCharacter(gameObject);

        if (detected)
            nPCDetected--;

        PlayeronBattleoff();

        IEnumerable<Monster> monster = from unit in nPCharacter
                                       where !((Monster)unit).unitMove.isFear
                                       select (Monster)unit;

        foreach (Monster item in monster)
        {
            item.curstat.curMORALE -= 7;
            item.MentalBarRenew();
        }

        if (monster.Count() <= 0)
        {
            foreach (var item in playerCharacter)
            {
                item.SetDetected(false);
            }
        }
    }

    public void HereComesNewObject(CObject gameObject)
    {
        objects.Add(gameObject);
    }
    public void ObjectOut(CObject gameObject)
    {
        objects.Remove(gameObject);
        TargetOutMonster(gameObject);
    }
    void TargetOutCharacter(CObject gameObject)
    {
        var character = from one in playerCharacter
                        where one.unitMove.targetEnermy == gameObject
                        select one;
        foreach (CUnit item in character)
        {
            item.unitMove.BattleContinue();
        }
    }
    void TargetOutMonster(CObject gameObject)
    {
        var monster = from one in nPCharacter
                      where one.unitMove.targetEnermy == gameObject
                      select one;

        foreach (CUnit item in monster)
        {
            item.unitMove.BattleContinue();
        }
        TargetOutCharacter(gameObject);
    }
    void PlayeronBattleoff()
    {
        if (nPCharacter.Count == 0)
        {
            foreach (CUnit item in playerCharacter)
            {
                item.SetDetected(false);
                item.unitMove.OnBattle(false);
            }
            playerDetected = 0;
        }
    }
    void MonsteronBattleoff()
    {
        if (playerDetected == 0)
        {
            foreach (CUnit mon in nPCharacter)
            {
                mon.unitMove.OnBattle(false);
            }
        }
    }

    #endregion 오브젝트 활성화/비활성화 체크
    #region view 타겟 확인
    public void Search(GameObject foundObject, UnitMove founder)
    {
        CUnit cUnit = foundObject.GetComponent<CUnit>();
        Search(cUnit, founder);
    }
    public void Search(CUnit cUnit, UnitMove founder)
    {
        if (cUnit.detected)
            return;
        cUnit.SetDetected(true);

        founder.SearchAct();


        if (founder is MonsterMove)
        {
            playerDetected++;
            if (playerDetected == playerCharacter.Count)
            {
                CloseEyes(nPCharacter);
                //모두 발각되었다는 알림.
            }
        }
        else
        {
            nPCDetected++;
            if (nPCDetected == nPCharacter.Count)
            {
                Debug.Log(nPCDetected);
                CloseEyes(playerCharacter);
                //모든 적 발견 알림.
            }
        }
    }
    void CloseEyes(List<CUnit> cUnits)
    {
        foreach (var item in cUnits)
        {
            item.FindAll();
        }
    }
    #endregion view 타겟 확인
    #region 마우스 입력
    public void OrderUnit(RaycastHit hit)
    {
        if (string.Compare(hit.collider.tag, "Enermy") == 0)
        {
            playerNavi.TargetSet(hit.collider, (OrderType)System.Convert.ToInt32(GetShift()));
        }
        else if (string.Compare(hit.collider.tag, "Item") == 0)
        {
            playerNavi.GetItem(hit.collider.transform.position, (OrderType)System.Convert.ToInt32(GetShift()));
        }
        else
        {
            playerNavi.Navi_Destination(hit.point, MoveType.Move, (OrderType)System.Convert.ToInt32(GetShift()));
        }

        bool GetShift()
        {
            return Input.GetKey(shiftCode);
        }


    }
    public void ChangeShift(in string newShift)
    {
        KeyCode newCode;
        switch (newShift)
        {
            case "leftctrl":
                newCode = KeyCode.LeftControl;
                break;
            case "rightctrl":
                newCode = KeyCode.RightControl;
                break;
            case "enter":
                newCode = KeyCode.Return;
                break;
            case "printscreen":
                newCode = KeyCode.Print;
                break;
            default:
                if (newShift.Contains("numpad"))
                {
                    string strNumPad = newShift.Replace("num", "key");
                    System.Enum.TryParse(strNumPad, true, out newCode);
                }
                else
                {
                    System.Enum.TryParse(newShift, true, out newCode);
                    if (newCode == KeyCode.None)
                        Debug.Log("할당 실패" + newShift);
                }
                break;
        }


        shiftCode = newCode;
    }
    public void DragUnitPosition(Vector3 vector3, Vector2 vector2, bool dragging)
    {
        rec[0] = vector3.x;
        rec[1] = vector3.x + vector2.x;
        rec[2] = vector3.y;
        rec[3] = vector3.y + vector2.y;

        DragEffect(objects);
        DragEffect(nPCharacter);
        DragEffect(playerCharacter);

        void DragEffect<T>(List<T> values) where T : CObject
        {
            foreach (var item in values)
            {
                item.DragBoxCollide(rec, dragging);
            }
        }
    }
    public void DragSelectingEnd(int count)
    {
        playerNavi.HeroClear();

        DragEffect(objects);
        DragEffect(nPCharacter);
        DragEffect(playerCharacter);

        var list = from item in playerCharacter
                   where item.selected
                   select item;
        if (list.Count() < 1)
            return;
        if (count % 2 == 0)
        {
            CUnit target = list.First();

            var group = from team in playerNavi.PlayerCharacter.Values
                        from one in team
                        where team.Any(Unit => Unit.cUnit == target) && one.cUnit.Getnum == target.Getnum
                        select one.cUnit;

            foreach (CUnit item in group)
            {
                item.Selected(true);
            }

        }
        else
        {
            playerNavi.HeroClear();
            foreach (CUnit item in list)
            {
                item.Selected(true);
            }
        }


        void DragEffect<T>(List<T> values) where T : CObject
        {
            foreach (var item in values)
            {
                item.DragEnd(ModifierCtrl(item), ModifierShift(item));
            }
        }
        bool ModifierShift<T>(T item) where T : CObject
        {
            return Keyboard.current.shiftKey.isPressed && item.selected;
        }
        bool ModifierCtrl<T>(T item) where T : CObject
        {
            //선택되지 않은 유닛 누를 때는 shift와 같음.
            //선택된 유닛 누를 때는 해당 유닛 제외.
            // (selecting || shift || ctrl) && default true
            //                              && !(selecting && ctrl)
            return Keyboard.current.ctrlKey.isPressed && item.selected;
        }
    }

    public void PointMove(Vector3 move)
    {
        screenMove(move);
    }
    #endregion
    #region heap
    struct UnitDistance
    {
        public CObject unit;
        public float distance;

        public UnitDistance(CObject cUnit, float num)
        {
            unit = cUnit;
            distance = num;
        }
    }
    List<UnitDistance> heapList = new List<UnitDistance>();
    void AddHeapList(CObject unit, float distance, List<UnitDistance> values)
    {
        UnitDistance sData = new UnitDistance(unit, distance);
        values.Add(sData);
        HeapCompare(values.Count - 1, values);
    }
    void HeapCompare(int i, List<UnitDistance> values)
    {
        if (values[i].distance >= values[i / 2].distance || i < 2)
            return;

        HeapSwap(i, i / 2, values);
        HeapCompare(i / 2, values);
    }
    void HeapSwap(int index1, int index2, List<UnitDistance> values)
    {
        (values[index1], values[index2]) = (values[index2], values[index1]);
    }

    public CObject GetNearest<T>(in List<T> units, Vector3 targetPos, Predicate<T> predicate, float range) where T : CObject
    {
        List<UnitDistance> heap = new List<UnitDistance>();
        heap.Add(new UnitDistance());

        var list = from unit in units
                   where predicate(unit)
                   select unit;

        foreach (CObject item in list)
        {
            AddHeapList(item, Vector3.Distance(item.transform.position, targetPos), heap);
        }

        if (list.Count() < 1 || heap[1].distance > range)
            return null;

        return heap[1].unit;
    }
    #endregion
    #region 전투
    public void SetOtheronBattle(List<CUnit> list)
    {
        foreach (CUnit unit in list)
        {
            unit.unitMove.OnBattle(true);
        }
    }
    public void NewTargetting<T>(List<T> values, UnitMove finder, Predicate<T> del, float range = 1000) where T : CObject
    {
        if (values.Count <= 0)
            return;

        CObject cunit = GetNearest(values, finder.transform.position, del, range);
        if (cunit == null)
            return;
        finder.AutoTargeting(cunit);
    }

    public void DamageCalculate(CUnit Attacker, CObject Target, int skill, in SkillData.Skill skillData = null, in System.Action action = null)
    {
        if (Target == default || !Target.enabled || Target.curstat.HP <= 0)
            return;

        int hitweigh = 0;
        int atkMultiply = 100;
        bool ambush = false;
        bool detected = Attacker.detected;

        Target.DetectbyHit(Attacker);
        Target.Counter(Attacker);

        if (skillData != null)
        {
            hitweigh = skillData.hitWeigh;
            atkMultiply = skillData.atkMultiply;
            ambush = skillData.type.Equals(SkillData.SkillType.Ambush);
        }

        if (skillData != null)
            onSkill.eventAction?.Invoke(Attacker.gameObject.layer, Attacker.transform.position);
        else
            onAttack.eventAction?.Invoke(Attacker.gameObject.layer, Attacker.transform.position);

        int hit = UnityEngine.Random.Range(0, 101) + Attacker.curstat.Accuracy + hitweigh;
        if (hit < 100)
        {
            Target.PrintMiss();
        }
        else if ((hit - Target.curstat.DOG < 100) || (ambush && detected))
        {
            Target.Dodge();
            Target.PrintDodge();
        }
        else
        {
            int Dmg = Attacker.curstat.ATK * atkMultiply * (100 - Target.curstat.DEF) / 10000;

            Target.curstat.curHP -= Dmg * Convert.ToInt32(UnityEngine.SceneManagement.SceneManager.GetActiveScene() != UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(2));
            Target.Hit(skill);
            Target.PrintDamage(-Dmg);

            if (action != null)
                action();


            MentalityCalculate();

            if (Target.curstat.curHP <= 0)
            {
                onDie.eventAction?.Invoke(Target.gameObject.layer, Target.transform.position);
                Target.Death(Attacker.transform.position);
            }



            void MentalityCalculate()
            {
                Attacker.SetMentality(true, Attacker);
                Target.SetMentality(false, Attacker);
            }
        }



    }

    #endregion 전투
    #region 키보드 입력
    public void ArmySelect(string numbering)
    {
        playerNavi.HeroClear();

        Unselect(playerCharacter);
        Unselect(objects);
        Unselect(nPCharacter);

        foreach (Character item in playerNavi.PlayerCharacter[GetConvert(numbering)])
        {
            CUnit cUnit = item.cUnit;
            cUnit.Selected(true);
        }

        if (playerNavi.lists.Count > 0)
            onCallgroup.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);
    }
    public string GetConvert(in string key)
    {
        return keyboardConverter[key];
    }
    public void Unselect<T>(List<T> values) where T : CObject
    {
        foreach (var item in values)
        {
            item.Selected(false);
        }
    }
    public void Formation(string key)
    {
        playerNavi.MakeFormation(GetConvert(key));
    }

    public void InputSpace()
    {
        if (Time.timeScale <= 0)
            return;

        inputSpace[StageManager.instance.GetIndexScene()]();

    }
    void TimeDelay()
    {
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        timeUIEvent[0]();
        TimeTurningAccel(true);
    }
    void TimeTurningAccel(bool isTimeDelay)
    {
        foreach (CUnit item in playerCharacter)
        {
            item.unitMove.SetTurnSpeed(isTimeDelay);
        }
    }
    public void SpaceUp()
    {
        if (Time.timeScale <= 0)
            return;

        inputSpaceUp[StageManager.instance.GetIndexScene()]();
    }
    public void TimeDelayEnd()
    {
        if (Time.timeScale <= 0)
            return;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        timeUIEvent[1]();
        TimeTurningAccel(false);
    }
    public void UnitStop(string key)
    {
        playerNavi.SimpleKey(GetConvert(key));
    }
    public void OrderAdd(string key)
    {
        playerNavi.QueueOrder(GetConvert(key));
    }
    public void TeamAdd(in string key)
    {
        string str = GetConvert(key);
        playerNavi.HeroClear();

        var list = from item in playerCharacter
                   where item.selected || playerNavi.PlayerCharacter[str].Contains(item.unitMove as Character)
                   select item;


        foreach (var item in list)
        {
            item.Selected(true);
        }
    }
    public void RegroupTeam(in string key)
    {
        playerNavi.SetTeam(GetConvert(key));

        if (playerNavi.lists.Count > 0)
            onRegroup.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);
    }
    public void ScreenToPoint(Vector3 vec)
    {
        Vector3 destination = Quaternion.AngleAxis(-50, Vector3.right) * Vector3.up;

        Ray ray = new Ray(vec, destination);
        Vector3 beforePosition = Camera.main.transform.position;
        Camera.main.transform.position = ray.GetPoint((10 - vec.y) / Mathf.Cos(50 * Mathf.Deg2Rad));

        float uiWidth = (Camera.main.transform.position.x - beforePosition.x) * Screen.height / (Camera.main.orthographicSize * 2);
        float uiHeight = (Camera.main.transform.position.z - beforePosition.z) * Screen.height / (Camera.main.orthographicSize * 2) * Mathf.Sin(Mathf.Deg2Rad * 40); ;

        PointMove(new Vector3(uiWidth, uiHeight, 0));
    }

    void VilligeBuildingScroll()
    {
        callConstructionUI();
    }
    #endregion 키보드 입력
    #region Manager 할당
    public void SetBattleClearManager(BattleClearManager newBattleClearManager)
    {
        battleClearManager = newBattleClearManager;
        onBattleClearManagerRegistered?.Invoke();
        onBattleClearManagerRegistered = null;
    }
    #endregion

    public void ReadytoSceneLoad()
    {
        isReady = false;
    }

    public void ConverterChange(in string key, in string value)
    {
        keyboardConverter.Add(key, value);
        Debug.Log(keyboardConverter.Count.ToString() + "  " + key + "  " + value);
    }
    public string GetEmptyKey()
    {
        return "empty" + keyDictionaryCount++.ToString();
    }
    public void KeyConverterKeyDelete(in string oldStr, out string value)
    {
        value = keyboardConverter[oldStr];
        keyboardConverter.Remove(oldStr);
    }
    public bool IsOnOneRight(CObject obj, Vector3 vec)
    {
        Quaternion dir = Quaternion.Euler(0, -obj.transform.eulerAngles.y, 0);

        return (dir * (vec - obj.transform.position)).x > 0;
    }
    public void CheckObjectLoadComplete()
    {
        objectCount++;
        int allObjects = objects.Count + nPCharacter.Count + playerCharacter.Count;

        if (objectCount != allObjects)
            return;

        Debug.Log("실행");

        ForInverse(objects);
        ForInverse(nPCharacter);
        ForInverse(playerCharacter);

        CheckCloseEye();
    }
    void ForInverse<T>(in List<T> collections) where T : CObject
    {
        int length = collections.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            collections[i].OnInitEnd?.Invoke();
        }
    }

    void CheckCloseEye()
    {
        if (playerCharacter.Count == playerDetected)
            CloseEyes(nPCharacter);

        if (nPCharacter.Count == nPCDetected)
            CloseEyes(playerCharacter);
    }
}
