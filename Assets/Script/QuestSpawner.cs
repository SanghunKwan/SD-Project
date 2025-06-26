using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestSpawner : MonoBehaviour
{
    delegate void HighLightActions(Vector3 highLightPosition, Vector3 callPosition, float highLightSize, ref Action action);

    HighLightActions[] highLightActions2;

    QuestManager questManager;
    QuestSaveData questSaveData;
    GameManager.ActionEvent[] actionEvent;

    Action<QuestManager.QuestData.QuestAct, QuestManager.QuestType, int>[] prepareQuestActions;
    Action<QuestManager.QuestData, Action, int>[] createQuestRequirementsActions;
    Action<QuestManager.QuestData.QuestReward>[] EventClearActions;
    Action<QuestManager.QuestData.QuestEvent, QuestUISlot, QuestManager.QuestData>[] createQuestEventActions;

    public QuestManager.QuestType[] types;
    [SerializeField] QuestUIViewer questUIViewer;
    [SerializeField] RectTransform backGroundTransform;
    QuestBackGround questBackGround;
    QuestPool questPool;

    [SerializeField] StorageComponent storage;
    [SerializeField] SetBuildingMat setBuildingMat;

    Dictionary<QuestManager.QuestData, QuestPool.QuestActionInstance> doingActionQuestDictionary;

    // Start is called before the first frame update
    private void Awake()
    {
        questBackGround = backGroundTransform.Find("QuestBackGround").GetComponent<QuestBackGround>();
        questPool = GetComponent<QuestPool>();

        doingActionQuestDictionary = new Dictionary<QuestManager.QuestData, QuestPool.QuestActionInstance>();

        SetActions();
    }
    void SetActions()
    {
        prepareQuestActions
            = new Action<QuestManager.QuestData.QuestAct, QuestManager.QuestType, int>[(int)QuestManager.QuestTrigger.Max];
        prepareQuestActions[(int)QuestManager.QuestTrigger.Location] = PrepareQuestbyLocation;
        prepareQuestActions[(int)QuestManager.QuestTrigger.UnitSpecificAct] = PrepareQuestbySpecificAction;

        highLightActions2 = new HighLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Max];
        highLightActions2[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Vector] = HighLightVector;
        highLightActions2[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Object] = HighLightObject;
        highLightActions2[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.NoFocus] = HighLightNoFocus;
        highLightActions2[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.FocusUI] = HighLightUI;

        createQuestRequirementsActions
            = new Action<QuestManager.QuestData, Action, int>[(int)QuestManager.QuestData.QuestRequirements.RequirementsType.Max];
        createQuestRequirementsActions[(int)QuestManager.QuestData.QuestRequirements.RequirementsType.spot]
            = CreateQuestForSpot;
        createQuestRequirementsActions[(int)QuestManager.QuestData.QuestRequirements.RequirementsType.item]
            = CreateQuestForItem;
        createQuestRequirementsActions[(int)QuestManager.QuestData.QuestRequirements.RequirementsType.repeat]
            = CreateQuestForAction;

        EventClearActions = new Action<QuestManager.QuestData.QuestReward>[(int)QuestManager.QuestData.QuestReward.RewardType.Max];
        EventClearActions[(int)QuestManager.QuestData.QuestReward.RewardType.Item] = ClearForItem;
        EventClearActions[(int)QuestManager.QuestData.QuestReward.RewardType.NewStage] = (reward) => ClearForStage();
        EventClearActions[(int)QuestManager.QuestData.QuestReward.RewardType.StageSet] = (reward) => ClearStageArrive();

        createQuestEventActions = new Action<QuestManager.QuestData.QuestEvent, QuestUISlot, QuestManager.QuestData>[(int)QuestManager.QuestData.QuestEvent.EventType.Max];
        createQuestEventActions[(int)QuestManager.QuestData.QuestEvent.EventType.FreeMaterial] = QuestEventFreeMaterial;
        createQuestEventActions[(int)QuestManager.QuestData.QuestEvent.EventType.ItemTracking] = QuestEventItemCountTracking;
        createQuestEventActions[(int)QuestManager.QuestData.QuestEvent.EventType.Accumulated] = QuestEventAccumulatedTracking;
        createQuestEventActions[(int)QuestManager.QuestData.QuestEvent.EventType.CurrentCount] = QuestEventShowCurrentCount;
        createQuestEventActions[(int)QuestManager.QuestData.QuestEvent.EventType.LastUnit] = QuestShowLastEnemy;
        createQuestEventActions[(int)QuestManager.QuestData.QuestEvent.EventType.ShowLayerNum] = QuestShowLayerNum;
    }
    void Start()
    {
        SetGameManagerActionArray();
        questManager = GameManager.manager.questManager;
        questSaveData = GameManager.manager.battleClearManager.SaveDataInfo.questSaveData;
        CheckDataEmptyNInit(questSaveData);

        MakeNewQuest();
        MakeExistingQuest();

        GameManager.manager.battleClearManager.IsCleared(this);
    }
    void SetGameManagerActionArray()
    {
        actionEvent = new GameManager.ActionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.Max];
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.Crying] = GameManager.manager.onCry;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.Attack] = GameManager.manager.onAttack;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.BackAttack] = GameManager.manager.onBackAttack;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.Skill] = GameManager.manager.onSkill;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.LowHP] = GameManager.manager.onLowHp;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.Die] = GameManager.manager.onDie;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.Selected] = GameManager.manager.onSelected;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.ItemUse] = GameManager.manager.onItemUse;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.ReGroup] = GameManager.manager.onRegroup;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.CallGroup] = GameManager.manager.onCallgroup;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.LowHPRelease] = GameManager.manager.onLowHPRelease;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.EnterStage] = GameManager.manager.onPlayerEnterStage;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeButton] = GameManager.manager.onVilligeButton;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.EffectedOtherQuest] = GameManager.manager.onEffectedOtherEvent;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.TargettingNonDetected] = GameManager.manager.onTargettingNonDetected;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingChoosed] = GameManager.manager.onVilligeBuildingChoosed;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingStartConstruction] = GameManager.manager.onVilligeBuildingStartConstruction;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingCompleteConstruction] = GameManager.manager.onVilligeBuildingCompleteConstruction;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingHeroAllocation] = GameManager.manager.onVilligeBuildingHeroAllocation;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingHeroCancellation] = GameManager.manager.onVilligeBuildingHeroCancellation;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingWindowToggle] = GameManager.manager.onVilligeBuildingWindowToggle;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeHeroInteractDrag] = GameManager.manager.onVilligeHeroInteractDrag;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeHeroSummon] = GameManager.manager.onVilligeHeroSummon;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeSummonInteract] = GameManager.manager.onVilligeSummonInteract;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeTowerFloorSelect] = GameManager.manager.onVilligeTowerFloorSelect;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeExpeditionWindow] = GameManager.manager.onVilligeExpeditionWindow;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.ItemUseOnStore] = GameManager.manager.onItemUseOnStore;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.ItemUseOnExpedition] = GameManager.manager.onItemUseOnExpedition;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.HeroSelect] = GameManager.manager.onHeroSelect;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.CallFormation] = GameManager.manager.onCallFormation;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.EnemyHorror] = GameManager.manager.onEnemyHorror;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeExpeditionFloorSelect] = GameManager.manager.onVilligeExpeditionFloorSelect;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeExpeditionFloorDelete] = GameManager.manager.onVilligeExpeditionFloorDelete;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.GetMaterials] = GameManager.manager.onGetMaterials;
    }
    void CheckDataEmptyNInit(QuestSaveData data)
    {
        if (!data.isLoaded)
        {
            data.SetDataSize();
            Debug.Log("newData");
        }
        data.Init();
    }

    void MakeExistingQuest()
    {
        int length;
        List<QuestSaveData.BitSaveData.QuestProgress> tempList;
        foreach (var item in types)
        {
            tempList = questSaveData[item].nowQuestList;
            length = tempList.Count;
            for (int i = 0; i < length; i++)
            {
                MakeQuest(item, tempList[i].questIndex, tempList[i].callPosition, tempList[i].nowProgress);
            }
        }

    }
    public void MakeNewQuest()
    {
        QuestSaveData.SaveDataBit data;
        int length;
        foreach (var item in types)
        {
            length = questManager.GetQuestCount(item);
            for (int i = 0; i < length; i++)
            {
                data = questSaveData.GetQuestState(item, i);
                if (data != QuestSaveData.SaveDataBit.Enable)
                    continue;

                PrepareQuest(item, i);
            }
        }
    }

    #region makeQuest
    void MakeQuest(QuestManager.QuestType type, int questNum, Vector3 callPosition, int nowProgress = 0)
    {
        QuestManager.QuestData data = questManager.GetQuestData(type, questNum);

        //퀘스트 설명 UI 생성
        questUIViewer.ShowNewQuest(out QuestUISlot slot);
        slot.SetQuestText(data.name, data.detail);

        //퀘스트 관련 backGround
        QuestHighLight2(data.highLight, callPosition, slot);

        //퀘스트 현재 상태 변경
        SetBitSave(type, questNum, QuestSaveData.SaveDataBit.Doing);



        Action completeAction = () =>
        {
            SetBitSave(type, questNum, QuestSaveData.SaveDataBit.Complete);
            questUIViewer.ClearQuest(slot);

            QuestManager.QuestData.QuestFollow.QuestFollowData[] datas = data.follow.datas;
            if (data.follow.needCallNextQuest)
            {
                int length = datas.Length;
                for (int i = 0; i < length; i++)
                {
                    SetBitSave(datas[i].type, datas[i].questNum, QuestSaveData.SaveDataBit.Enable);

                    if (ContainsType(datas[i].type))
                        PrepareQuest(datas[i].type, datas[i].questNum);
                }

            }
        };

        //퀘스트 클리어 조건 형성
        CreateQuestRequirements(data, completeAction, nowProgress);

        CreateQuestEvent(data, slot);

        GameManager.manager.onEffectedOtherEvent.eventAction?.Invoke(questNum, Vector3.zero);
    }
    bool ContainsType(QuestManager.QuestType type)
    {
        return Array.IndexOf(types, type) >= 0;
    }

    void SetBitSave(QuestManager.QuestType type, int questNum, QuestSaveData.SaveDataBit nowState)
    {
        questSaveData.SetQuestState(type, questNum, nowState);
    }

    #region createQuestRequirementsActions
    void CreateQuestRequirements(QuestManager.QuestData data, in Action completeAction, int nowProgress)
    {
        createQuestRequirementsActions[(int)data.require.type](data, completeAction, nowProgress);
    }
    void CreateQuestForSpot(QuestManager.QuestData data, Action completeAction, int nowProgress)
    {
        QuestManager.QuestData.QuestRequirements require = data.require;

        questPool.PlaceQuest(data, require.spot
            + GameManager.manager.battleClearManager.GetStageComponent(require.stageOffsetIndex).transform.position,
            (vec) =>
        {
            EventClear(data.reward);
            completeAction();
        });
    }
    void CreateQuestForItem(QuestManager.QuestData data, Action completeAction, int nowProgress)
    {
        DisposableActionItem(() =>
        {
            EventClear(data.reward);
            completeAction();
        },
        data.require.itemDatas);
    }
    void DisposableActionItem(Action action, QuestManager.QuestData.ItemData[] itemDatas)
    {
        void Wrapper(int layerNum)
        {
            int length = itemDatas.Length;
            for (int i = 0; i < length; i++)
            {
                if (storage.ItemCounts[itemDatas[i].itemCode] < itemDatas[i].itemCount)
                    return;
            }

            action();
            storage.SubtractListener(Wrapper);
        }
        storage.AddListener(Wrapper);
    }

    void CreateQuestForAction(QuestManager.QuestData data, Action completeAction, int nowProgress)
    {
        Action complete = () =>
        {
            completeAction();
            EventClear(data.reward);
            questPool.ReturnQuestActInstance(doingActionQuestDictionary[data]);
            doingActionQuestDictionary.Remove(data);
        };

        QuestPool.QuestActionInstance instance = questPool.GetQuestActInstance(nowProgress, data, complete, actionEvent);

        doingActionQuestDictionary.Add(data, instance);
        doingActionQuestDictionary[data].LateInit?.Invoke(() => doingActionQuestDictionary[questManager.GetQuestData(data.questType, data.require.stageOffsetIndex)].CompleteCheck(0, Vector3.zero));

    }
    #endregion
    #region CreateQuestEvent
    void CreateQuestEvent(QuestManager.QuestData questData, QuestUISlot slot)
    {
        QuestManager.QuestData.QuestEvent questEvent = questData.questEvent;
        //퀘스트 이벤트 발생
        createQuestEventActions[(int)questEvent.eventType]?.Invoke(questEvent, slot, questData);
    }
    void QuestEventFreeMaterial(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData)
    {
        int eventIndex = questEvent.eventIndex;
        int eventNum = questEvent.eventNum;
        //자원 무료 사용
        //priceInfoBox 표기값 변경, isBuildable true로 변경
        setBuildingMat.AddQuest((SetBuildingMat.MaterialsType)eventIndex, eventNum);
        slot.hideAction += () => setBuildingMat.RemoveQuest((SetBuildingMat.MaterialsType)eventIndex, eventNum);
    }
    void QuestEventItemCountTracking(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData)
    {
        //아이템 개수 추적. quest창에 표기.
        QuestManager.QuestData.ItemData tempData;
        int length = questData.require.itemDatas.Length;
        slot.GetQuestText(out string originalTitle, out string originalDetail);
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        AddText();


        DisposableActionItem(() =>
        {
            slot.SetQuestText(originalTitle, originalDetail);
            AddText();
        }, questData.require.itemDatas);

        void AddText()
        {

            for (int i = 0; i < length; i++)
            {
                tempData = questData.require.itemDatas[i];
                stringBuilder.AppendJoin(' ', "\n<color=#AAAAAA>", InventoryManager.i.info.items[tempData.itemCode].name, "<color=#FFA500>" + storage.ItemCounts[tempData.itemCode], "</color>/</color>", tempData.itemCount);
                slot.AddQuestText("", stringBuilder.ToString());
                stringBuilder.Clear();
            }
        }
    }
    void QuestEventAccumulatedTracking(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData)
    {
        //Accumulated 추적. quest창에 표기.
        QuestShowCount(questEvent, slot, questData, questData.require.accumulatedTime);
    }

    void QuestEventShowCurrentCount(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData)
    {
        //Accumulated 추적. quest창에 표기.
        QuestShowCount(questEvent, slot, questData, questData.require.layer);
    }
    void QuestShowCount(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData, int length)
    {
        slot.GetQuestText(out string originalTitle, out string originalDetail);
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        AddText();


        DisposableAccumulateCount(() =>
        {
            slot.SetQuestText(originalTitle, originalDetail);
            AddText();
        }, doingActionQuestDictionary[questData]);

        void AddText()
        {
            stringBuilder.AppendJoin(' ', "\n<color=#AAAAAA>", questEvent.actionName, "\n<color=#FFA500>" + doingActionQuestDictionary[questData].progress, "</color>회 /</color>", length, "<color=#AAAAAA>회");
            slot.AddQuestText("", stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
    void DisposableAccumulateCount(Action action, QuestPool.QuestActionInstance questInstance)
    {
        //dictionary 에 있는 progress 값을 추적하는 함수.
        questInstance.ActionAdd(action);
    }
    void QuestShowLastEnemy(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData)
    {
        slot.GetQuestText(out string originalTitle, out string originalDetail);
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        AddText();


        DisposableAccumulateCount(() =>
        {
            slot.SetQuestText(originalTitle, originalDetail);
            AddText();
        }, doingActionQuestDictionary[questData]);

        void AddText()
        {
            stringBuilder.AppendJoin(' ', "\n<color=#AAAAAA>", "남은 적", "\n<color=#FFA500>" + GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Monster].Count, "</color>마리</color>");
            slot.AddQuestText("", stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
    void QuestShowLayerNum(QuestManager.QuestData.QuestEvent questEvent, QuestUISlot slot, QuestManager.QuestData questData)
    {
        slot.GetQuestText(out string originalTitle, out string originalDetail);
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        AddText(0);


        DisposableLayerNum((layerNum) =>
        {
            slot.SetQuestText(originalTitle, originalDetail);
            AddText(layerNum);
        }, doingActionQuestDictionary[questData]);

        void AddText(int layerNum)
        {
            stringBuilder.AppendJoin(' ', "\n<color=#AAAAAA>", questEvent.actionName, "\n<color=#FFA500>" + layerNum, "</color>회 /</color>", doingActionQuestDictionary[questData].layerNum, "<color=#AAAAAA>회");
            slot.AddQuestText("", stringBuilder.ToString());
            stringBuilder.Clear();
        }

        void DisposableLayerNum(Action<int> action, QuestPool.QuestActionInstance questInstance)
        {
            questInstance.layerCalled += action;
        }
    }
    #endregion
    #region eventClearActions
    void EventClear(QuestManager.QuestData.QuestReward reward)
    {
        //이벤트 보상 주기
        EventClearActions[(int)reward.rewardType](reward);
    }
    void ClearForItem(QuestManager.QuestData.QuestReward reward)
    {
        foreach (var item in reward.rewardItems)
        {
            storage.ItemCountChange(item.itemCode, item.itemCount);
        }
    }
    void ClearForStage()
    {
        GameManager.manager.battleClearManager.ActivateNextFloor(this);

        //새로운 스테이지 floor 활성화
    }
    void ClearStageArrive()
    {
        GameManager.manager.battleClearManager.ActiveStageObject();

        //새로운 스테이지 오브젝트 활성화
    }

    #endregion
    #endregion

    #region HighLightActions2
    void QuestHighLight2(QuestManager.QuestData.QuestHighLight highlight, in Vector3 callPosition, QuestUISlot slot)
    {
        float nowScale = Time.timeScale;

        Action action = () =>
        {
            if (highlight.timeStop)
            {
                Time.timeScale = 0;
                slot.TimeStopHighLight();
            }
        };
        //마을에서 건물 관련 효과 진행중일 때 지연.
        //UI 관련 효과가 아닐 경우 action에 더해줌.
        highLightActions2[(int)highlight.highLight]?.Invoke(highlight.highLightPosition, callPosition, highlight.size, ref action);

        slot.hideAction += () =>
        {
            if (highlight.highLight != QuestManager.QuestData.QuestHighLight.HighLightTarget.None)
            {
                questBackGround.SetOff();
                Time.timeScale = nowScale;

                GameManager.manager.onBattleClearManagerRegistered -= action;
            }
        };
    }
    void HighLightVector(Vector3 highLightPosition, Vector3 callPosition, float highLightSize, ref Action action)
    {
        action += () =>
        questBackGround.SetHighLight(highLightPosition, highLightSize);

        DelayHighLight(action);
    }
    void HighLightObject(Vector3 highLightPosition, Vector3 callPosition, float highLightSize, ref Action action)
    {
        //카메라 object 주변으로 이동.
        action += () =>
        {
            GameManager.manager.ScreenToPoint(callPosition);
            questBackGround.SetHighLight(callPosition + highLightPosition, highLightSize);
        };

        DelayHighLight(action);
    }
    void HighLightNoFocus(Vector3 highLightPosition, Vector3 callPosition, float highLightSize, ref Action action)
    {
        action += () =>
        {
            questBackGround.SetHighLight(callPosition, highLightSize);
            questBackGround.SetActiveHole(false);
        };
        DelayHighLight(action);
    }
    void HighLightUI(Vector3 highLightPosition, Vector3 callPosition, float highLightSize, ref Action action)
    {
        action();
        questBackGround.SetHighLightUI(highLightPosition, highLightSize);
    }
    void DelayHighLight(Action action)
    {
        //바로 작동할 수 있을 때
        if (!questManager.isBuildingUnderControl)
            action();
        else
            questManager.onBuildingControlFinish += action;

    }
    #endregion


    #region 외부 이벤트 요청
    public void PrepareQuest(QuestManager.QuestType type, int questNum)
    {
        //시작 가능한 퀘스트 trigger 생성.
        QuestManager.QuestData data = questManager.GetQuestData(type, questNum);
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        prepareQuestActions[(int)data.trigger](data.act, type, questNum);
    }
    void PrepareQuestbyLocation(QuestManager.QuestData.QuestAct act, QuestManager.QuestType type, int questNum)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;
        //collider를 통해 퀘스트 시작 포인트 가져오기
        questPool.PlacePrepare(act.spot + battleClearManager.GetStageComponent(0).transform.position,
            act.layer, (vec) => MakeQuest(type, questNum, vec), act.radius, act.id);
    }
    void PrepareQuestbySpecificAction(QuestManager.QuestData.QuestAct act, QuestManager.QuestType type, int questNum)
    {
        DisposableAction((vec) => MakeQuest(type, questNum, vec), act.whatCause, act.layer, 0);
    }
    void DisposableAction(Action<Vector3> action, QuestManager.QuestData.QuestAct.UnitActType actType, int layer, int maxCount, int nowProgress = 0)
    {

        int num = nowProgress;
        void Wrapper(int layerNum, Vector3 vec)
        {
            if (!layer.Equals(layerNum))
                return;

            if (num < maxCount)
                return;

            action(vec);
            actionEvent[(int)actType].eventAction -= Wrapper;
            Debug.Log("actionSubtracted");
        }
        Debug.Log("actionAdded");
        actionEvent[(int)actType].eventAction += Wrapper;
    }



    #endregion
}
