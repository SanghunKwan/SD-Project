using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSpawner : MonoBehaviour
{
    QuestManager questManager;
    QuestSaveData questSaveData;
    GameManager.ActionEvent[] actionEvent;

    Action<QuestManager.QuestData.QuestAct, QuestManager.QuestType, int>[] prepareQuestActions;
    Action<Vector3, Vector3, float>[] highLightActions;
    Action<QuestManager.QuestData, Action, int>[] createQuestRequirementsActions;
    Action<QuestManager.QuestData.QuestReward>[] EventClearActions;

    public QuestManager.QuestType[] types;
    [SerializeField] QuestUIViewer questUIViewer;
    [SerializeField] RectTransform backGroundTransform;
    QuestBackGround questBackGround;
    QuestPool questPool;

    [SerializeField] InventoryStorage inventoryStorage;
    [SerializeField] InventoryComponent inventoryComponent;

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

        highLightActions = new Action<Vector3, Vector3, float>[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Max];
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Vector]
            = (highLightPosition, vec, size) => HighLightVector(highLightPosition, size);
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Object] = HighLightObject;
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.NoFocus] = HighLightNoFocus;
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.FocusUI] = HighLightUI;

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
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingScroll] = GameManager.manager.onVilligeBuildingScroll;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.EffectedOtherQuest] = GameManager.manager.onEffectedOtherEvent;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.TargettingNonDetected] = GameManager.manager.onTargettingNonDetected;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingChoosed] = GameManager.manager.onVilligeBuildingChoosed;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingStartConstruction] = GameManager.manager.onVilligeBuildingStartConstruction;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingCompleteConstruction] = GameManager.manager.onVilligeBuildingCompleteConstruction;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingHeroAllocation] = GameManager.manager.onVilligeBuildingHeroAllocation;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingHeroCancellation] = GameManager.manager.onVilligeBuildingHeroCancellation;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeBuildingWindowOpen] = GameManager.manager.onVilligeBuildingWindowOpen;
        actionEvent[(int)QuestManager.QuestData.QuestAct.UnitActType.VilligeHeroInteractDrag] = GameManager.manager.onVilligeHeroInteractDrag;
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
        QuestHighLight(data.highLight, callPosition, slot);

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
                    PrepareQuest(datas[i].type, datas[i].questNum);
                }

            }
        };

        //퀘스트 클리어 조건 형성
        CreateQuestRequirements(data, completeAction, nowProgress);

        GameManager.manager.onEffectedOtherEvent.eventAction?.Invoke(questNum, Vector3.zero);
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
                if (inventoryStorage.ItemCounts[itemDatas[i].itemCode] < itemDatas[i].itemCount)
                    return;
            }

            action();
            inventoryStorage.SubtractListener(Wrapper);
        }
        inventoryStorage.AddListener(Wrapper);

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
            inventoryStorage.ItemCountChange(item.itemCode, item.itemCount);
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
    #region 하이라이트 actions
    void QuestHighLight(QuestManager.QuestData.QuestHighLight highlight, in Vector3 callPosition, QuestUISlot slot)
    {
        float nowScale = Time.timeScale;
        slot.hideAction = () =>
        {
            if (highlight.highLight != QuestManager.QuestData.QuestHighLight.HighLightTarget.None)
            {
                questBackGround.SetOff();
                Time.timeScale = nowScale;
            }
        };

        if (highlight.timeStop)
        {
            Time.timeScale = 0;
            slot.TimeStopHighLight();
        }
        highLightActions[(int)highlight.highLight]?.Invoke(highlight.highLightPosition, callPosition, highlight.size);
    }
    void HighLightVector(in Vector3 highLightPosition, float highLightSize)
    {
        questBackGround.SetHighLight(highLightPosition, highLightSize);
    }
    void HighLightObject(Vector3 highLightPosition, Vector3 callPosition, float highLightSize)
    {
        //카메라 object 주변으로 이동.
        GameManager.manager.ScreenToPoint(callPosition);
        questBackGround.SetHighLight(callPosition + highLightPosition, highLightSize);
    }
    void HighLightNoFocus(Vector3 highLightPosition, Vector3 callPosition, float highLightSize)
    {
        questBackGround.SetHighLight(callPosition, highLightSize);
        questBackGround.SetActiveHole(false);
    }
    void HighLightUI(Vector3 highLightPosition, Vector3 callPosition, float highLightSize)
    {
        questBackGround.SetHighLightUI(highLightPosition, highLightSize);
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
