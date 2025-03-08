using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPool : MonoBehaviour
{
    [SerializeField] QuestTrigger questPrefabs;
    Stack<QuestTrigger> questTriggers;
    Stack<QuestActionInstance> questActInstancePool;

    Transform questPrefabFolder;

    Action<QuestActionInstance>[] questActInstanceAddTypeActions;

    private void Awake()
    {
        questActInstancePool = new Stack<QuestActionInstance>(5);
        questTriggers = new Stack<QuestTrigger>(5);

        questPrefabFolder = new GameObject("questPrefabsFolder").transform;
        questPrefabFolder.SetParent(transform);

        SetStacks();
        SetActions();
    }
    void SetStacks()
    {
        questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));
        questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));
        questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));
        questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));
        questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));

        questActInstancePool.Push(new QuestActionInstance());
        questActInstancePool.Push(new QuestActionInstance());
        questActInstancePool.Push(new QuestActionInstance());
        questActInstancePool.Push(new QuestActionInstance());
        questActInstancePool.Push(new QuestActionInstance());
    }
    void SetActions()
    {
        questActInstanceAddTypeActions
            = new Action<QuestActionInstance>[(int)QuestManager.QuestData.QuestAct.ActCondition.Max];

        questActInstanceAddTypeActions[(int)QuestManager.QuestData.QuestAct.ActCondition.Accumulated]
            = (instance) => instance.TypeAccumulate();
        questActInstanceAddTypeActions[(int)QuestManager.QuestData.QuestAct.ActCondition.LastUnit]
            = (instance) => instance.TypeLastUnit();
        questActInstanceAddTypeActions[(int)QuestManager.QuestData.QuestAct.ActCondition.HasSuperQuest]
            = (instance) => instance.TypeHasSuperQuest();
    }
    private void Start()
    {
        GameManager.manager.questManager.questPool = this;
    }

    public void PlacePrepare(in Vector3 vec, int triggerLayer, in Action<Vector3> triggerAction, float radius, int nowId, bool isMakeQuest = false)
    {
        if (questTriggers.Count <= 0)
            questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));

        QuestTrigger popTrigger = questTriggers.Pop();
        popTrigger.TriggerAllSet(triggerLayer, triggerAction, vec, radius, nowId);
    }
    public void PlaceQuest(QuestManager.QuestData data, in Vector3 vec, in Action<Vector3> triggerAction)
    {
        if (questTriggers.Count <= 0)
            questTriggers.Push(Instantiate(questPrefabs, questPrefabFolder));

        QuestManager.QuestData.QuestRequirements require = data.require;

        QuestTrigger popTrigger = questTriggers.Pop();
        popTrigger.TriggerAllSet(require.layer, triggerAction, vec, require.radius, require.id);

        GameManager.manager.battleClearManager.doingQuestTrigger.Add(popTrigger);
        popTrigger.AddMakeQuestData((vec) =>
            GameManager.manager.battleClearManager.doingQuestTrigger.Remove(popTrigger), data.questType, data.num);
    }

    public void ReturnQuest(QuestTrigger usedTrigger)
    {
        questTriggers.Push(usedTrigger);
    }

    #region QuestActionInstance 관련 함수
    public class QuestActionInstance
    {
        public int progress { get; private set; }
        public int questIndex { get; private set; }
        public QuestManager.QuestType type { get; private set; }
        int layerNum;
        int maxCount;
        Action completeAction;
        Action actionCalled;
        public Action<Action> LateInit { get; set; }
        GameManager.ActionEvent savedEvent;

        public void Init(int nowProgress, QuestManager.QuestData data, in Action nowCompleteAction,
                                    in GameManager.ActionEvent[] eventAction)
        {
            QuestManager.QuestData.QuestRequirements require = data.require;

            progress = nowProgress;
            layerNum = require.layer;
            maxCount = require.accumulatedTime;
            completeAction = nowCompleteAction;
            type = data.questType;
            questIndex = data.num;

            savedEvent = eventAction[(int)require.actionType];
            savedEvent.eventAction += CompleteCheck;

            GameManager.manager.battleClearManager.doingQuestActionInstance.Add(this);
            completeAction += () => GameManager.manager.battleClearManager.doingQuestActionInstance.Remove(this);
        }
        #region progress증가방식설정
        public void ProgressAdd()
        {
            progress++;
        }
        public void TypeAccumulate()
        {
            actionCalled += ProgressAdd;
        }
        public void TypeLastUnit()
        {
            TypeAccumulate();
            maxCount = GameManager.manager.dicNpcCharacter.Count;
        }
        public void TypeHasSuperQuest()
        {
            TypeAccumulate();
            LateInit += SuperQuestProgressAdd;
        }
        void SuperQuestProgressAdd(Action action)
        {
            completeAction += () =>
            {
                action();
                LateInit = null;
            };
        }
        #endregion

        public void CompleteCheck(int objectLayerNum, Vector3 vec)
        {
            if (layerNum != objectLayerNum)
                return;

            actionCalled?.Invoke();

            if (progress >= maxCount)
            {
                CompleteQuest();
            }
        }
        void CompleteQuest()
        {
            completeAction();
            savedEvent.eventAction -= CompleteCheck;

            ClearInstance();
        }
        void ClearInstance()
        {
            progress = 0;
            layerNum = 0;
            maxCount = 0;

            completeAction = null;
            savedEvent = null;
            actionCalled = null;
        }
    }
    public QuestActionInstance GetQuestActInstance(int nowProgress, QuestManager.QuestData data, in Action nowCompleteAction,
                                    in GameManager.ActionEvent[] eventAction)
    {
        if (questActInstancePool.Count <= 0)
            questActInstancePool.Push(new QuestActionInstance());

        QuestActionInstance instance = questActInstancePool.Pop();
        instance.Init(nowProgress, data, nowCompleteAction, eventAction);
        QuestActIntanceSetCountAddType(data.require.actCondition, instance);

        return instance;
    }
    void QuestActIntanceSetCountAddType(QuestManager.QuestData.QuestAct.ActCondition type, QuestActionInstance instance)
    {
        questActInstanceAddTypeActions[(int)type]?.Invoke(instance);
    }
    public void ReturnQuestActInstance(QuestActionInstance usedinstance)
    {
        questActInstancePool.Push(usedinstance);
    }

    #endregion
}
