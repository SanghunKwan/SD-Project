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
                Hero,
                Object,
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
        public class QuestSpot
        {
            public int layer;
            public Vector3 spot;
            public float radius;

            public QuestSpot()
            {
                layer = 7;
                spot = Vector3.zero;
                radius = 1;
            }
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
                Die
            }

            public int layer;
            public UnitActType whatDo;

            public QuestAct()
            {
                layer = 7;
                whatDo = UnitActType.LowHP;
            }
        }
        [Serializable]
        public class QuestReward
        {
            public enum RewardType
            {
                Item,
                NewStage,
                SceneClear
            }
            [Serializable]
            public class RewardItem
            {
                public int itemCode;
                public int itemCount;
                public RewardItem()
                {
                    itemCode = 1;
                    itemCount = 1;
                }
            }
            public RewardType rewardType;
            public RewardItem[] rewardItems;
            public QuestReward()
            {
                rewardType = RewardType.Item;
                rewardItems = new RewardItem[1];
                rewardItems[0] = new RewardItem();

            }
        }

        public int num;
        public string name;
        public string detail;
        public QuestReward reward;
        public QuestHighLight highLight;
        public QuestTrigger trigger;

        public QuestSpot spot;
        public QuestAct act;

        public QuestData()
        {
            num = 1;
            name = "바보들의 대잔치";
            detail = "모든 몬스터 죽이기";
            reward = new QuestReward();
            highLight = new QuestHighLight();
            trigger = QuestTrigger.Location;
            spot = new QuestSpot();
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

    #endregion
    QuestDatas questDatas;

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

    private void Awake()
    {
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
