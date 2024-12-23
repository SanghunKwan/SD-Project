using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            public QuestHighLight()
            {
                timeStop = true;
                highLight = HighLightTarget.Vector;
                highLightPosition = Vector3.zero;
                size = 1;
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
            public Vector3 spot;
            public float radius;

            public ItemData[] itemDatas;

            public QuestAct.UnitActType actionType;
            public QuestAct.ActCondition actCondition;
            public int accumulatedTime;


            public QuestRequirements()
            {
                type = RequirementsType.spot;
                layer = 7;
                spot = Vector3.zero;
                radius = 1;
                actCondition = QuestAct.ActCondition.Accumulated;
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
                Max
            }
            public enum ActCondition
            {
                Accumulated,
                LastUnit,
                PreviousQuest,
            }

            public Vector3 spot;
            public float radius;
            public int layer;
            public UnitActType whatCause;


            public QuestAct()
            {
                spot = Vector3.zero;
                radius = 1;
                layer = 7;
                whatCause = UnitActType.LowHP;
            }
        }
        [Serializable]
        public class QuestReward
        {
            public enum RewardType
            {
                Item,
                NewStage,
                SceneClear,
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

        public int num;
        public string name;
        public string detail;
        public QuestReward reward;
        public QuestHighLight highLight;
        public QuestTrigger trigger;

        public QuestRequirements require;
        public QuestAct act;
        public QuestFollow follow;

        public QuestData()
        {
            num = 1;
            name = "바보들의 대잔치";
            detail = "모든 몬스터 죽이기";
            reward = new QuestReward();
            highLight = new QuestHighLight();
            trigger = QuestTrigger.Location;
            require = new QuestRequirements();
            act = new QuestAct();

        }
    }
    [Serializable]
    public class QuestDatas
    {
        public QuestData[] this[QuestType index] { get { return data[(int)index]; } }

        public QuestData[] stagePerformOnedata;
        public QuestData[] villigePerformOnedata;
        public QuestData[] floorQuestdata;
        public QuestData[] stageQuestdata;
        public QuestData[] villigeQuestdata;

        QuestData[][] data;
        public void Init()
        {

            data = new QuestData[][] { stagePerformOnedata, villigePerformOnedata, floorQuestdata, stageQuestdata, villigeQuestdata };
        }
        public QuestDatas()
        {
            stagePerformOnedata = new QuestData[1];
            villigePerformOnedata = new QuestData[1];
            floorQuestdata = new QuestData[1];
            stageQuestdata = new QuestData[1];
            villigeQuestdata = new QuestData[1];
        }
    }

    public enum QuestType
    {
        StagePerformOnlyOne,
        VilligePerformOnlyOne,
        FloorQuest,
        StageQuest,
        VilligeQuest
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
        throw new System.NotImplementedException();
    }

    #endregion
}
