using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [SerializeField] PlayerNavi playerNavi;
    [SerializeField] MonNavi monNavi;

    //새로운 유닛 매니저 생성해서 관리하도록 할 것.
    //dictionary<gameObject, linkedListNode>
    //linkedlist<cobject>
    //두 종류로 관리하고, 유닛 사망 시 node를 삭제해서 null 체크를 할 수 있도록 구현.
    //기존 기능은 dic과 linkedlist를 나눠서 접근.
    //battleClearManager의 List를 linkedList에 통합해서 관리.

    int playerDetected;
    int nPCDetected;
    int objectCount;


    public static bool isReady { get; private set; }


    public Action[] timeUIEvent = new Action[2];
    public Action<Vector3> screenMove { get; set; } = (screenMove) => { };


    Dictionary<string, string> keyboardConverter = new Dictionary<string, string>();
    public KeyCode[] modifiers { get; set; } = new KeyCode[3] { KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.Space };

    Action[] inputSpace = new Action[3];
    Action[] inputSpaceUp = new Action[3];

    public Action callConstructionUI;
    public Action onBattleClearManagerRegistered { get; set; }

    public enum ModifiersNum
    {
        Shift,
        Ctrl,
        Space
    }
    #region actionEvent
    public class ActionEvent
    {
        public Action<int, Vector3> eventAction;
    }
    public enum ActionButtonNum
    {
        VilligeBuildingScrollBtn = 1,

        VilligeFloorSelectBtn,
        VilligeExpeditionBtn,
        VilligeExpeditionProlongBtn,
        VilligeExpeditionProlongCancelExit,

        VilligeUpgradeWindowCallBtn
    }
    public enum GetMaterialsNum
    {
        Quirk,
        Disease,
        Healing,
        Helmet,
        Armor,
        Weapon,
        WeaponType,
        ArmorType,
        Skill,
        SpecialMove,
        Building,
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
    public ActionEvent onVilligeButton { get; private set; } = new ActionEvent();
    public ActionEvent onEffectedOtherEvent { get; private set; } = new ActionEvent();
    public ActionEvent onTargettingNonDetected { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingChoosed { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingStartConstruction { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingCompleteConstruction { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingHeroAllocation { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingHeroCancellation { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingWindowToggle { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeHeroInteractDrag { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeHeroSummon { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeSummonInteract { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeTowerFloorSelect { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeExpeditionWindow { get; private set; } = new ActionEvent();
    public ActionEvent onItemUseOnStore { get; private set; } = new ActionEvent();
    public ActionEvent onItemUseOnExpedition { get; private set; } = new ActionEvent();
    public ActionEvent onHeroSelect { get; private set; } = new ActionEvent();
    public ActionEvent onCallFormation { get; private set; } = new ActionEvent();
    public ActionEvent onEnemyHorror { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeExpeditionFloorSelect { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeExpeditionFloorDelete { get; private set; } = new ActionEvent();
    public ActionEvent onGetMaterials { get; private set; } = new ActionEvent();
    public ActionEvent onVilligeBuildingWindowClose { get; private set; } = new ActionEvent();
    #endregion


    public PointerEventData pointerEventData { get; set; }

    #region 하위 managers
    public SoundManager soundManager { get; set; }
    public WindowManager windowManager { get; set; }
    public StorageManager storageManager { get; set; }
    public BattleClearManager battleClearManager { get; private set; }
    public QuestManager questManager { get; set; }
    public ObjectManager objectManager { get; set; }
    public SettingLoader settingLoader { get; set; }
    #endregion

    private void Awake()
    {
        manager = this;

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
        ConverterKeyAdd(str, str);
    }
    public void ChangeModifier(in string codeName, ModifiersNum num)
    {
        KeyCode newCode;
        switch (codeName)
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
                if (codeName.Contains("numpad"))
                {
                    string strNumPad = codeName.Replace("num", "key");
                    Enum.TryParse(strNumPad, true, out newCode);
                }
                else
                {
                    Enum.TryParse(codeName, true, out newCode);
                    if (newCode == KeyCode.None)
                        Debug.Log("할당 실패" + codeName);
                }
                break;
        }
        modifiers[(int)num] = newCode;
    }
    public string GetConvert(in string key)
    {
        return keyboardConverter[key];
    }
    public void ConverterKeyDelete(in string key)
    {
        keyboardConverter.Remove(key);
    }
    public void ConverterKeyAdd(in string key, in string value)
    {
        keyboardConverter.Add(key, value);
    }

    #region 오브젝트 활성화/비활성화 체크
    public void HereComesNewChallenger(CUnit unit, string keycode)
    {
        playerNavi.SetTeam(unit.unitMove, keycode);

        EyeOpenIfCountChanged(objectManager.ObjectDictionary[(int)ObjectManager.CObjectType.Hero],
                              objectManager.ObjectDictionary[(int)ObjectManager.CObjectType.Monster],
                              nPCDetected, playerDetected, unit);
    }
    public void HereComesNewEnermy(CUnit gameObject)
    {
        monNavi.MonsterAdd(gameObject);

        EyeOpenIfCountChanged(objectManager.ObjectDictionary[(int)ObjectManager.CObjectType.Monster],
                              objectManager.ObjectDictionary[(int)ObjectManager.CObjectType.Hero],
                              playerDetected, nPCDetected, gameObject);
    }
    void EyeOpenIfCountChanged(IReadOnlyDictionary<GameObject, LinkedListNode<CObject>> AddedDic,
                               IReadOnlyDictionary<GameObject, LinkedListNode<CObject>> eyeCheckDic,
                               int eyeCheckDetectedCount, int addedDetectedCount, CUnit addedUnit)
    {
        if (eyeCheckDic.Count > eyeCheckDetectedCount)
            addedUnit.EyeOpen();

        if (AddedDic.Count != addedDetectedCount + 1) return;

        foreach (var unit in eyeCheckDic.Values)
        {
            ((CUnit)unit.Value).EyeOpen();
        }
    }

    public void ChallengerOut(CUnit gameObject, string keycode, bool detected)
    {
        playerNavi.HeroClear(gameObject, keycode);
        objectManager.OutObject(ObjectManager.CObjectType.Hero, gameObject.gameObject);

        if (detected)
            playerDetected--;
    }
    public void HeroDeathEvent()
    {
        foreach (Monster item in objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster])
        {
            item.curstat.Mentality = item.stat.Mentality;
            item.MentalBarRenew();

            if (playerDetected > 0) continue;

            item.unitMove.OnBattle(false);
        }
    }

    public void MonsterOut(CUnit gameObject, bool detected)
    {
        monNavi.MonsterRemove(gameObject);
        objectManager.OutObject(ObjectManager.CObjectType.Monster, gameObject.gameObject);

        if (detected)
            nPCDetected--;
    }
    public void MonsterDeathEvent()
    {
        IReadOnlyCollection<CObject> monster = objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster];

        foreach (Monster item in monster)
        {
            if (item.unitMove.isFear) continue;

            item.curstat.curMORALE -= 7;
            item.MentalBarRenew();
        }

        if (monster.Count > 0) return;

        foreach (CUnit item in objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero])
        {
            item.SetDetected(false);
            item.unitMove.OnBattle(false);
        }
        playerDetected = 0;
    }
    #endregion 오브젝트 활성화/비활성화 체크
    #region view 타겟 확인
    public void Search(ObjectManager.CObjectType objectType, GameObject foundObject, UnitMove founder)
    {
        Search(objectType, (CUnit)objectManager.ObjectDictionary[(int)objectType][foundObject].Value, founder);
    }
    public void Search(ObjectManager.CObjectType unitType, CUnit cUnit, UnitMove founder)
    {
        if (cUnit.detected) return;

        cUnit.SetDetected(true);

        founder.SearchAct();

        SearchEvent(unitType == ObjectManager.CObjectType.Hero, objectManager.ObjectList[(int)unitType], objectManager.ObjectList[Mathf.Abs((int)unitType - 1)]);
    }

    void SearchEvent(bool isTypeHero, IReadOnlyCollection<CObject> detectedList, IReadOnlyCollection<CObject> detectingList)
    {
        ref int detectedNum = ref (isTypeHero ? ref playerDetected : ref nPCDetected);

        detectedNum++;
        if (detectedNum == detectedList.Count)
        {
            CloseEyes(detectingList);
            //모두 발각되었다는 알림.
        }
    }


    void CloseEyes(IReadOnlyCollection<CObject> detectedList)
    {
        foreach (CUnit item in detectedList)
        {
            item.FindAll();
        }
    }
    #endregion view 타겟 확인
    #region 마우스 입력
    public void OrderUnit(RaycastHit hit)
    {
        playerNavi.SmartOrder(hit, (OrderType)Convert.ToInt32(GetShift()));

        bool GetShift()
        {
            return Input.GetKey(modifiers[(int)ModifiersNum.Shift]);
        }
    }


    public void DragUnitPosition(Vector3 vector3, Vector2 recSize, bool dragging)
    {
        float[] rec = new float[4] { vector3.x,
                                     vector3.x + recSize.x,
                                     vector3.y,
                                     vector3.y + recSize.y };

        foreach (var item in objectManager.ObjectList)
        {
            DragEffectObject(item, rec, dragging);
        }
    }
    void DragEffectObject(IReadOnlyCollection<CObject> values, in float[] rec, bool dragging)
    {
        foreach (var item in values)
        {
            item.DragBoxCollide(rec, dragging);
        }
    }

    public void DragSelectingEnd(int count)
    {
        int selectedCount = 0;
        playerNavi.HeroClear();

        bool isNotSelected = true;
        foreach (var item in objectManager.ObjectList)
        {
            if (isNotSelected)
            {
                if (DragEffect(item, ref selectedCount))
                {
                    isNotSelected = true;
                }
            }
            else
            {
                DragFalse(item);
            }
        }

        if (selectedCount < 1)
            return;

        if (count % 2 == 0)
        {
            //더블 클릭
            if (playerNavi.lists.Count > 0)
                //player nav의 같은 팀원 호출.
                foreach (var item in playerNavi.PlayerCharacter[((Hero)playerNavi.lists[0].cUnit).keycode])
                {
                    item.cUnit.Selected(true);
                }
        }

        onHeroSelect.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);
        Debug.Log(playerNavi.lists.Count);


        bool DragEffect(IReadOnlyCollection<CObject> values, ref int selectedCount)
        {
            bool isSelected = false;
            foreach (var item in values)
            {
                if (item.DragEnd(ModifierCtrl(item), ModifierShift(item)))
                {
                    isSelected = true;
                    selectedCount++;
                }
            }
            return isSelected;
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
    public void DragFalse(IReadOnlyCollection<CObject> values)
    {
        foreach (var item in values)
        {
            item.DragFalse();
        }
    }
    public void PointMove(Vector3 move)
    {
        screenMove(move);
    }
    public void PointMoveConversionToUI(Vector3 move)
    {
        Vector3 conversionVector3 = new Vector3(
                       move.x * Screen.height / (Camera.main.orthographicSize * 2),
                       move.z * Screen.height / (Camera.main.orthographicSize * 2) * Mathf.Sin(Mathf.Deg2Rad * 40));

        screenMove(conversionVector3);
        Debug.Log("이동");
    }
    #endregion
    #region heap
    public CObject GetNearestNDetected(IReadOnlyCollection<CObject> targets, Vector3 position, float maxValue)
    {
        return GetConditionedObject(targets,
           (obj) =>
           {
               if (!((CUnit)obj).detected) return float.MaxValue;

               return Vector3.SqrMagnitude(obj.transform.position - position);
           },
           (current, previous) => current < previous,
           maxValue);
    }
    public CObject GetNearest(IReadOnlyCollection<CObject> targets, Vector3 position)
    {
        return GetConditionedObject(targets,
            (obj) => Vector3.SqrMagnitude(obj.transform.position - position),
            (current, previous) => current < previous,
            float.MaxValue);
    }
    /// <summary>
    /// 조건에 맞는 오브젝트를 찾는 함수.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targets">순회할 배열</param>
    /// <param name="getCurrentValueFunc">배열에서 필요한 값 계산 함수</param>
    /// <param name="conditionFunc">이전 값과 현재 값 비교 함수</param>
    /// <param name="initValue">이전값 초기화(최소 혹은 최대)</param>
    /// <returns></returns>
    CObject GetConditionedObject<T>(IReadOnlyCollection<CObject> targets, Func<CObject, T> getCurrentValueFunc, Func<T, T, bool> conditionFunc, T initValue) where T : struct, IComparable
    {
        CObject tempTarget = null;
        T previousValue = initValue;
        T currentValue = default;

        foreach (var item in targets)
        {
            currentValue = getCurrentValueFunc(item);
            if (conditionFunc(currentValue, previousValue))
            {
                previousValue = currentValue;
                tempTarget = item;
            }
        }
        return tempTarget;
    }
    #endregion
    #region 전투
    public void SetOtheronBattle(IReadOnlyCollection<CObject> cUnitList)
    {
        foreach (CUnit unit in cUnitList)
        {
            unit.unitMove.OnBattle(true);
        }
    }
    public void NewTargetting(IReadOnlyCollection<CObject> values, UnitMove finder, float maxValue = float.MaxValue)
    {
        if (values.Count <= 0) return;

        CObject cunit = GetNearestNDetected(values, finder.transform.position, maxValue);
        if (cunit == null) return;

        finder.AutoTargeting(objectManager.GetNode(cunit));
    }
    public void DamageCalculate(CUnit Attacker, CObject Target, int skill, in SkillData.Skill skillData = null, in Action action = null)
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
                int tempLayer = Target.gameObject.layer;
                Target.Death(Attacker.transform.position);
                onDie.eventAction?.Invoke(tempLayer, Target.transform.position);
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
    public void ArmySelect(in string numbering)
    {
        playerNavi.HeroClear();
        IReadOnlyList<LinkedList<CObject>> linkedListArray = objectManager.ObjectList;
        CUnit tempUnit;

        foreach (var item in linkedListArray)
        {
            DragFalse(item);
        }

        foreach (Character item in playerNavi.PlayerCharacter[GetConvert(numbering)])
        {
            tempUnit = item.cUnit;
            tempUnit.Selected(true);
        }

        if (playerNavi.lists.Count > 0)
            onCallgroup.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);

        onHeroSelect.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);
    }

    public void Formation(in string key)
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
        foreach (CUnit item in objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero])
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
    public void UnitStop(in string key)
    {
        playerNavi.SimpleKey(GetConvert(key));
    }
    public void OrderAdd(in string key)
    {
        playerNavi.QueueOrder(GetConvert(key));
    }
    public void TeamAdd(in string key)
    {
        IReadOnlyList<LinkedList<CObject>> linkedListArray = objectManager.ObjectList;
        CUnit tempUnit;

        DragFalse(linkedListArray[(int)ObjectManager.CObjectType.Monster]);
        DragFalse(linkedListArray[(int)ObjectManager.CObjectType.FieldObject]);

        foreach (var item in playerNavi.PlayerCharacter[GetConvert(key)])
        {
            tempUnit = item.cUnit;
            tempUnit.Selected(true);
        }

        onHeroSelect.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);
    }
    public void RegroupTeam(in string key)
    {
        playerNavi.SetTeam(GetConvert(key));

        if (playerNavi.lists.Count > 0)
            onRegroup.eventAction?.Invoke(playerNavi.lists.Count, playerNavi.getCenter);
    }
    public void ScreenToPoint(in Vector3 vec)
    {
        Vector3 destination = Quaternion.AngleAxis(-50, Vector3.right) * Vector3.up;

        Ray ray = new Ray(vec, destination);
        Vector3 beforePosition = Camera.main.transform.position;
        Camera.main.transform.position = ray.GetPoint((10 - vec.y) / Mathf.Cos(50 * Mathf.Deg2Rad));

        PointMoveConversionToUI(Camera.main.transform.position - beforePosition);
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


    public bool IsOnOneRight(CObject obj, Vector3 vec)
    {
        Quaternion dir = Quaternion.Euler(0, -obj.transform.eulerAngles.y, 0);

        return (dir * (vec - obj.transform.position)).x > 0;
    }
    public void CheckObjectLoadComplete()
    {
        objectCount++;
        int objectTypeLength = (int)ObjectManager.CObjectType.Max;
        int allObjects = 0;

        IReadOnlyList<LinkedList<CObject>> objectListArray = objectManager.ObjectList;


        for (int i = 0; i < objectTypeLength; i++)
        {
            allObjects += objectListArray[i].Count;
        }

        if (objectCount != allObjects)
            return;

        for (int i = 0; i < objectTypeLength; i++)
        {
            ForInverse(objectListArray[i]);
        }

        CheckCloseEye();
    }
    void ForInverse(LinkedList<CObject> values)
    {
        LinkedListNode<CObject> tempNode = values.Last;
        LinkedListNode<CObject> prevNode;
        while (tempNode != null)
        {
            prevNode = tempNode.Previous;
            tempNode.Value.OnInitEnd?.Invoke();
            tempNode = prevNode;
        }
    }
    void CheckCloseEye()
    {
        IReadOnlyCollection<CObject> tempHeroList = objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero];
        IReadOnlyCollection<CObject> tempMonsterList = objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster];

        if (tempHeroList.Count == playerDetected)
            CloseEyes(tempMonsterList);

        if (tempMonsterList.Count == nPCDetected)
            CloseEyes(tempHeroList);
    }
    public void SetViewRendererType(bool isLineRenderer)
    {
        if (objectManager == null) return;
        if (nPCDetected != objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster].Count)
            foreach (CUnit item in objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero])
                item.SetViewRendererType(isLineRenderer);

        if (playerDetected != objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero].Count)
            foreach (CUnit item in objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster])
                item.SetViewRendererType(isLineRenderer);
    }

    public void SetCanvasMask(bool isOn, int layerIndex)
    {
        if (isOn)
        {
            Camera.main.cullingMask &= ~(1 << layerIndex);
        }
        else
        {
            Camera.main.cullingMask |= (1 << layerIndex);
        }
    }
}
