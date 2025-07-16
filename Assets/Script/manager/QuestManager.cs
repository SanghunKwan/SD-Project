using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class QuestManager : JsonLoad
{
    #region 데이터 클래스
    [Serializable]
    public class QuestData
    {
        [Serializable]
        public class QuestHighLight
        {
            public enum HighLightTarget
            {
                None,
                Vector,
                Object,
                NoFocus,
                FocusUI,
                Max
            }
            public bool timeStop;
            public HighLightTarget highLight;
            public Vector3 highLightPosition;
            public float size;
            public string actionName;

            public QuestHighLight()
            {
                timeStop = true;
                highLight = HighLightTarget.Vector;
                highLightPosition = Vector3.zero;
                size = 1;
                actionName = string.Empty;
            }
        }
        [Serializable]
        public class QuestRequirements
        {
            public enum RequirementsType
            {
                spot,
                item,
                repeat,
                Max
            }

            public RequirementsType type;
            public int layer;
            public int id;
            public Vector3 spot;
            public float radius;

            public ItemData[] itemDatas;

            public QuestAct.UnitActType actionType;
            public QuestAct.ActCondition actCondition;
            public QuestAct.ConditionComparison conditionComparison;
            public int accumulatedTime;
            public int stageOffsetIndex;


            public QuestRequirements()
            {
                type = RequirementsType.spot;
                layer = 7;
                id = 0;
                spot = Vector3.zero;
                radius = 1;
                actCondition = QuestAct.ActCondition.Accumulated;
                conditionComparison = QuestAct.ConditionComparison.Equal;
                accumulatedTime = 0;
            }
        }
        [Serializable]
        public class ItemData
        {
            public int itemCode;
            public int itemCount;
        }
        [Serializable]
        public class QuestAct
        {
            public enum UnitActType
            {
                Crying,
                Attack,
                BackAttack,
                Skill,
                LowHP,
                Die,
                Selected,
                ItemUse,
                ReGroup,
                CallGroup,
                LowHPRelease,
                EnterStage,
                VilligeButton,
                EffectedOtherQuest,
                TargettingNonDetected,
                VilligeBuildingChoosed,
                VilligeBuildingStartConstruction,
                VilligeBuildingCompleteConstruction,
                VilligeBuildingHeroAllocation,
                VilligeBuildingHeroCancellation,
                VilligeBuildingWindowToggle,
                VilligeHeroInteractDrag,
                VilligeHeroSummon,
                VilligeSummonInteract,
                VilligeTowerFloorSelect,
                VilligeExpeditionWindow,
                ItemUseOnStore,
                ItemUseOnExpedition,
                HeroSelect,
                CallFormation,
                EnemyHorror,
                VilligeExpeditionFloorSelect,
                VilligeExpeditionFloorDelete,
                GetMaterials,
                ItemDescriptionPopUp,
                StageQuestClear,
                MinimapInput,

                Max
            }
            public enum ActCondition
            {
                Accumulated,
                LastUnit,
                HasSuperQuest,
                Max
            }
            public enum ConditionComparison
            {
                Equal = 1,
                MoreThan = 2,
                LessThan = 4
            }

            public Vector3 spot;
            public float radius;
            public int layer;
            public int id;
            public UnitActType whatCause;
            public ConditionComparison conditionComparison;


            public QuestAct()
            {
                spot = Vector3.zero;
                radius = 1;
                layer = 7;
                id = 0;
                whatCause = UnitActType.LowHP;
                conditionComparison = ConditionComparison.Equal;
            }
        }
        [Serializable]
        public class QuestReward
        {
            public enum RewardType
            {
                Item,
                NewStage,
                StageSet,
                Max
            }
            public RewardType rewardType;
            public ItemData[] rewardItems;
            public QuestReward()
            {
                rewardType = RewardType.Item;
                rewardItems = new ItemData[1];
                rewardItems[0] = new ItemData();

            }
        }
        [Serializable]
        public class QuestFollow
        {
            [Serializable]
            public class QuestFollowData
            {
                public QuestType type;
                public int questNum;

                public QuestFollowData()
                {
                    type = QuestType.StagePerformOnlyOne;
                    questNum = 0;
                }
            }

            public QuestFollowData[] datas;
            public bool needCallNextQuest;
            public QuestFollow()
            {
                datas = new QuestFollowData[1];
                needCallNextQuest = false;
            }
        }
        [Serializable]
        public class QuestEvent
        {
            public EventType eventType;
            public int eventIndex;
            public int eventNum;
            public string actionName;

            public QuestEvent()
            {
                eventType = EventType.None;
                eventIndex = 0;
                eventNum = 0;
                actionName = string.Empty;
            }

            public enum EventType
            {
                None,
                FreeMaterial,
                ItemTracking,
                Accumulated,
                CurrentCount,
                LastUnit,
                ShowLayerNum,
                Max
            }
        }
        [Serializable]
        public class QuestHappening
        {
            public enum QuestHappeningType
            {
                None,
                ItemSpawn,


                Max
            }

            public QuestHappeningType type;
            public Vector3 vec;
            public int index;

            public QuestHappening()
            {
                type = QuestHappeningType.None;
                vec = Vector3.zero;
                index = 0;
            }
        }

        public int num;
        public QuestType questType;
        public QuestGrade questGrade;

        public string name;
        public string detail;
        public QuestReward reward;
        public QuestHighLight highLight;
        public QuestTrigger trigger;

        public QuestRequirements require;
        public QuestAct act;
        public QuestFollow follow;
        public QuestEvent questEvent;
        public QuestHappening questHappening;

        public QuestData()
        {
            num = 1;
            questType = QuestType.FloorQuest;
            questGrade = QuestGrade.튜토리얼;
            name = "바보들의 대잔치";
            detail = "모든 몬스터 죽이기";
            reward = new QuestReward();
            highLight = new QuestHighLight();
            trigger = QuestTrigger.Location;
            require = new QuestRequirements();
            act = new QuestAct();
            questEvent = new QuestEvent();
            questHappening = new QuestHappening();
        }
    }

    [Serializable]
    public class QuestDatas
    {
        public QuestData[] this[QuestType index] { get { return data[(int)index]; } }

        public QuestData[] stagePerformOnedata;
        public QuestData[] villigePerformOnedata;
        public QuestData[] floorQuestdata;
        public QuestData[] villigeQuestdata;

        QuestData[][] data;
        public void Init()
        {

            data = new QuestData[][] { stagePerformOnedata, villigePerformOnedata, floorQuestdata, villigeQuestdata };
        }
        public QuestDatas()
        {
            stagePerformOnedata = new QuestData[1];
            villigePerformOnedata = new QuestData[1];
            floorQuestdata = new QuestData[1];
            villigeQuestdata = new QuestData[1];
        }
    }

    public enum QuestType
    {
        StagePerformOnlyOne,
        VilligePerformOnlyOne,
        FloorQuest,
        VilligeQuest
    }
    public enum QuestGrade
    {
        튜토리얼,
        스테이지,

    }
    public enum QuestTrigger
    {
        Location,
        UnitSpecificAct,
        Max
    }
    #endregion

    QuestDatas questDatas;
    public QuestPool questPool { get; set; }
    public bool isBuildingUnderControl { get; set; }
    public Action onBuildingControlFinish { get; set; }

    private void Awake()
    {
        SDF<QuestDatas>();
        questDatas = LoadData<QuestDatas>("QuestData");
        questDatas.Init();
    }
    void Start()
    {
        GameManager.manager.questManager = this;
    }


    #region 외부 데이터 전달
    public QuestData GetQuestData(QuestType type, int index)
    {
        return questDatas[type][index];
    }
    public int GetQuestCount(QuestType type)
    {
        return questDatas[type].Length;
    }
    #endregion
    #region 안 쓰는 기능
    public override void Init()
    {
        throw new NotImplementedException();
    }

    #endregion
}
