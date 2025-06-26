using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveData;
using Unit;

namespace SaveData
{
    #region 세이브 데이터
    [Serializable]
    public class SaveDataInfo
    {
        public int day;
        public int nextScene;
        public FloorData floorData;

        public HeroData[] hero;
        public BuildingData[] building;

        public int[] items;

        public StageData stageData;
        public QuestSaveData questSaveData;
        public PlayInfo playInfo;


        public SaveDataInfo()
        {
            day = 0;
            nextScene = 1;
            hero = new HeroData[1] { new HeroData() };
            floorData = new FloorData();
            stageData = new StageData();
            //blank 포함 +1
            items = new int[(int)Materials.Max + 1];
        }
        public SaveDataInfo(int getDay, int getNextScene, FloorData getFloorData, in HeroData[] geHero,
            in BuildingData[] getBuilding, in int[] getItems, StageData getStageData, QuestSaveData getQuestSaveData)
        {
            day = getDay;
            nextScene = getNextScene;
            floorData = getFloorData;
            hero = geHero;
            building = getBuilding;
            items = getItems;
            stageData = getStageData;
            questSaveData = getQuestSaveData;
        }
    }
    [Serializable]
    public class QuirkDefaultData
    {
        public int[] quirks;
        public int length;
        public QuirkDefaultData()
        {
            quirks = new int[5] { 6, 2, 0, 0, 0 };
            length = 2;
        }
        public QuirkDefaultData(int length)
        {
            quirks = new int[length];
            length = 0;
        }
        public QuirkDefaultData(int nowQuirkCount, int quirkMaxCount, QuirkData.QuirkS quirksInfo)
        {
            length = nowQuirkCount;
            quirks = new int[quirkMaxCount];
            ReservoirSampling(1, quirksInfo.quirks.Length - 1, nowQuirkCount, out int[] selected);
            Array.Copy(selected, quirks, nowQuirkCount);


        }
        public QuirkDefaultData(in int[] getQuirks, int getLength)
        {
            quirks = getQuirks;
            length = getLength;
        }
        public void SetData(in QuirkData.Quirk[] quirksArray, int quirkCount)
        {
            int activeQuirkCount = 0;
            for (int i = 0; i < quirkCount; i++)
            {
                quirks[i] = quirksArray[i].index;

                if (quirks[i] != 0)
                    activeQuirkCount++;
            }
            length = activeQuirkCount;
        }
        public void ReservoirSampling(int minInclusive, int maxInclusive, int selectCount, out int[] randomSelectedArray)
        {
            int count = maxInclusive - minInclusive + 1;

            randomSelectedArray = new int[selectCount];

            for (int i = 0; i < selectCount; i++)
                randomSelectedArray[i] = minInclusive + i;

            for (int i = selectCount; i < count; i++)
            {
                //가 기존것과 교체될 확률
                if (IsSelected(selectCount, i))
                {
                    int randomIndex = UnityEngine.Random.Range(0, selectCount);
                    randomSelectedArray[randomIndex] = minInclusive + i;
                }

            }

            bool IsSelected(int selectCount, int arrayIndex)
            {
                return UnityEngine.Random.Range(0, 1) < (float)selectCount / (arrayIndex + 1);
            }
        }
    }
    [Serializable]
    public class QuirkSaveData : QuirkDefaultData
    {
        public bool willChange;
        public int willChangeIndex;

        public QuirkSaveData() : base()
        {
            Init();
        }
        public QuirkSaveData(int count) : base(count)
        {
            Init();
        }
        public QuirkSaveData(int nowCount, int maxCount) : base(nowCount, maxCount, QuirkData.manager.quirkInfo)
        {
            Init();
        }
        void Init()
        {
            willChange = false;
            willChangeIndex = 0;
        }
    }

    [Serializable]
    public class HeroData
    {
        public string name;
        public int lv;
        public QuirkSaveData quirks;
        public QuirkDefaultData disease;
        public string keycode;
        public int[] equipNum;
        public int[] fieldEquipNum;
        public int[] skillNum;
        public int villigeAction;
        public int workBuilding;
        public UnitData unitData;

        public HeroData()
        {
            name = "디스마스";
            lv = 1;
            quirks = new QuirkSaveData();
            disease = new QuirkDefaultData(4);
            keycode = "=";
            equipNum = new int[3] { 1, 1, 1 };
            fieldEquipNum = new int[3] { 0, 0, 0 };
            skillNum = new int[4] { 1, 1, 1, 1 };
            villigeAction = 0;
            workBuilding = 0;
            unitData = new UnitData();
        }
        public HeroData(Hero hero) : this()
        {
            InitData(hero);
        }
        public HeroData(in string getName, int getLevel, unit_status statData, QuirkSaveData quirkData, QuirkDefaultData diseaseData)
        {
            name = getName;
            lv = getLevel;
            quirks = quirkData;
            disease = diseaseData;
            keycode = "=";
            equipNum = new int[3] { 1, 1, 1 };
            fieldEquipNum = new int[3] { 0, 0, 0 };
            skillNum = new int[4] { 1, 1, 1, 1 };
            villigeAction = 0;
            workBuilding = 0;
            unitData = new UnitData(statData);
        }
        public void SetHeroData(Hero hero)
        {
            InitData(hero);
        }
        void InitData(Hero hero)
        {
            name = hero.name;
            lv = hero.lv;
            quirks.SetData(hero.quirks, 5);
            disease.SetData(hero.disease, 4);
            keycode = hero.keycode;
            Array.Copy(hero.EquipsNum, equipNum, 3);
            Array.Copy(hero.FieldEquipsNum, fieldEquipNum, 3);
            Array.Copy(hero.SkillsNum, skillNum, 4);
            villigeAction = (int)hero.VilligeAction;
            workBuilding = (int)hero.BuildingAction;
            unitData.SetData(hero);
        }
    }
    [Serializable]
    public class BuildingData
    {
        public int[] workHero = new int[3] { -1, -1, -1 };
        public int dayRemaining;
        public float timeNormalized;
        public ObjectData objectData;


        public BuildingData(BuildingConstructDelay buildingComponent)
        {
            workHero = buildingComponent.buildingComponent.GetWorkHeroArray();

            dayRemaining = buildingComponent.dayRemaining;

            timeNormalized = 1;
            if (buildingComponent.constructingUI != null)
                timeNormalized = buildingComponent.constructingUI.timeNormalized;

            objectData = new ObjectData(buildingComponent.buildingComponent.CObject);
        }
    }

    [Serializable]
    public class InventoryData
    {
        public ItemData[] itemDatas;
        public int[] corpseIndex;
        public InventoryData()
        {
        }
        public void SetData(in ItemData[] itemdatas, in int[] corpseindex)
        {
            itemDatas = itemdatas;
            corpseIndex = corpseindex;
        }
    }
    [Serializable]
    public class ItemData
    {
        public int itemIndex;
        public int itemCount;
        public int slotIndex;

        public ItemData(int getSlotIndex, InventoryStorage.Slot slot)
        {
            slotIndex = getSlotIndex;

            itemIndex = slot.itemCode;
            itemCount = slot.itemCount;

        }
    }
    [Serializable]
    public class StageData
    {
        public int[] heros;
        public int[] floors = new int[5];
        public int nowFloorIndex;
        public bool isClear;
        public bool isEnter;

        public InventoryData inventoryData;

        public FloorUnitData[] floorUnitDatas;
        public List<YetDroppedItem> yetDroppedItems;


        public StageData() : this(new int[] { 0 }, new int[] { 0 }) { }
        public StageData(in int[] getFloors, in int[] getHeros)
        {
            heros = getHeros;
            floors = getFloors;
            inventoryData = new InventoryData();
            nowFloorIndex = 0;
            isEnter = true;
            isClear = false;
        }
        public void CloneValue(StageData stageData)
        {
            heros = stageData.heros;
            floors = (int[])stageData.floors.Clone();
            nowFloorIndex = stageData.nowFloorIndex;
            isClear = stageData.isClear;
            isEnter = stageData.isEnter;
        }
    }
    [Serializable]
    public class FloorUnitData
    {
        public MonsterData[] monsterData;
        public ObjectData[] objectDatas;
        public DropItemData[] dropItemDatas;
    }
    [Serializable]
    public class MonsterData
    {
        public WaitingTypeNum waitingTypeNum;
        public UnitState_Monster.StandType standType;
        public Vector3 originTransform;
        public Vector3 patrolDestination;

        public UnitData unitData;
        public MonsterData()
        {
            waitingTypeNum = WaitingTypeNum.Wandering;
            standType = UnitState_Monster.StandType.Basic;
            originTransform = Vector3.zero;
            patrolDestination = Vector3.zero;
            unitData = new UnitData();
        }
        public MonsterData(Monster monster)
        {
            MonsterMove move = monster.monsterMove;

            waitingTypeNum = move.waitType;
            standType = move.standType;
            originTransform = move.originTransform;
            patrolDestination = move.patrolDestination;
            unitData = new UnitData(monster);
        }
    }
    [Serializable]
    public class UnitData
    {
        public bool detected;
        public bool attackMove;
        public bool ishold;
        public Vector3 destination;
        public Vector3 depart;
        public ObjectData objectData;
        public UnitData()
        {
            detected = false;
            attackMove = false;
            ishold = false;
            destination = Vector3.zero;
            depart = Vector3.zero;
            objectData = new ObjectData();
        }
        public UnitData(CUnit unit)
        {
            detected = unit.detected;
            attackMove = unit.unitMove.attackMove;
            ishold = unit.unitMove.isHold;
            destination = unit.unitMove.destination;
            depart = unit.unitMove.depart;
            objectData = new ObjectData(unit);
        }
        public UnitData(unit_status stat)
        {
            detected = false;
            attackMove = false;
            ishold = false;
            destination = Vector3.left * 7.8f;
            depart = Vector3.zero;
            objectData = new ObjectData(stat);
        }

        public void SetData(CUnit unit)
        {
            detected = unit.detected;
            attackMove = unit.unitMove.attackMove;
            ishold = unit.unitMove.isHold;
            destination = unit.unitMove.destination;
            depart = unit.unitMove.depart;
            objectData.SetData(unit);
        }
    }
    [Serializable]
    public class ObjectData
    {
        public Vector3 position;
        public Vector3 dotsDirection;
        public Quaternion quaternion;
        public bool selected;
        public bool isDead;
        public unit_status cur_status;
        public int[] dots;
        public ObjectData()
        {
            position = 2 * Vector3.forward;
            quaternion = Quaternion.Euler(0, 180, 0);
            selected = false;
            isDead = false;
            cur_status = new unit_status();
            dots = new int[(int)SkillData.EFFECTINDEX.MAX] { 0, 0, 0, 0 };
        }
        public ObjectData(CObject cObject)
        {
            position = cObject.transform.position;
            quaternion = cObject.transform.rotation;
            selected = cObject.selected;
            cur_status = new unit_status();
            cur_status.Clone(cObject.curstat);
            isDead = cur_status.curHP <= 0;
            dots = new int[cObject.dots.Length];
            Array.Copy(cObject.dots, dots, dots.Length);
        }
        public ObjectData(unit_status stat)
        {
            position = new Vector3(-7.8f, 0, 0);
            quaternion = Quaternion.Euler(0, 180, 0);
            selected = false;
            isDead = false;
            cur_status = stat;
            dots = new int[(int)SkillData.EFFECTINDEX.MAX] { 0, 0, 0, 0 };
        }
        public void SetData(CObject cObject)
        {
            position = cObject.transform.position;
            quaternion = cObject.transform.rotation;
            selected = cObject.selected;
            cur_status.Clone(cObject.curstat);
            isDead = cur_status.curHP <= 0;
            Array.Copy(cObject.dots, dots, dots.Length);
        }

    }
    [Serializable]
    public class DropItemData
    {
        public Vector3 position;
        public int index;
        public int stageIndex;
        public DropItemData(ItemComponent itemComponent)
        {
            position = itemComponent.transform.position;
            index = itemComponent.Index;
            stageIndex = itemComponent.transform.parent.GetSiblingIndex();
        }
    }
    [Serializable]
    public class YetDroppedItem
    {
        public enum DropType
        {
            Drop,
            Throw,
        }

        public DropType type;
        public int currentItemIndex;
        public int listIndex;
        public int stageIndex;
        public Vector3 itemsPosition;
        public List<int> items;
        public float offset;


        public YetDroppedItem(int length, int index, in Vector3 position, float colliderRadius)
        {
            type = DropType.Drop;
            currentItemIndex = 0;
            stageIndex = index;
            items = new List<int>(length);
            itemsPosition = position;
            offset = colliderRadius + 0.5f; // 아이템이 떨어지는 위치를 설정하기 위한 오프셋 값
        }
        public YetDroppedItem(int length, int itemCode, CObject thrower, bool isRight)
        {
            type = DropType.Throw;
            currentItemIndex = 0;
            stageIndex = thrower.stageIndex;
            items = new List<int>(length);
            for (int i = 0; i < length; i++)
            {
                items.Add(itemCode);
            }
            itemsPosition = thrower.transform.position;
            offset = thrower.transform.eulerAngles.y + Convert.ToInt32(!isRight) * 180;            //회전값
        }
    }

    [Serializable]
    public class QuestSaveData
    {
        public enum SaveDataBit
        {
            Disable,
            Enable,
            Doing,
            Complete
        }
        [Serializable]
        public class BitSaveData
        {
            [Serializable]
            public class QuestProgress
            {
                public int questIndex;
                public int nowProgress;
                public Vector3 callPosition;

                public QuestProgress(QuestTrigger triggerObject)
                {
                    questIndex = triggerObject.questIndex;
                    nowProgress = 0;
                    callPosition = triggerObject.transform.position;
                }
                public QuestProgress(QuestPool.QuestActionInstance instance)
                {
                    questIndex = instance.questIndex;
                    nowProgress = instance.progress;
                    callPosition = Vector3.zero;
                }
            }

            //bit 2개에 퀘스트 하나의 정보 저장.
            public long[] bits;
            public List<QuestProgress> nowQuestList;
            public BitSaveData(int MaxQuestNum)
            {
                int bitCount = GetBitsIndex(MaxQuestNum) + 1;
                nowQuestList = new List<QuestProgress>(5);
                bits = new long[bitCount];
            }
            public static int GetBitsIndex(int questNum)
            {
                return questNum / 32;
            }
            public SaveDataBit GetQuestState(int questNum)
            {
                Debug.Log(questNum);
                long questState = bits[GetBitsIndex(questNum)];
                Debug.Log(questState);

                return (SaveDataBit)((questState >> GetNumShift(questNum)) & 3);
            }
            public void AddQuestState(int questNum, SaveDataBit setBit)
            {
                int shiftIndex = GetNumShift(questNum);
                long changeBit = (long)setBit;
                bits[GetBitsIndex(questNum)] = (bits[GetBitsIndex(questNum)] & ~(3L << shiftIndex)) | (changeBit << shiftIndex);
            }
            int GetNumShift(int questNum)
            {
                return (questNum % 32) * 2;
            }
        }

        public BitSaveData stagePerformOneData;
        public BitSaveData villigePerformOneData;
        public BitSaveData floorQuestData;
        public BitSaveData villigeQuestData;
        public bool isLoaded;

        BitSaveData[] data;
        public BitSaveData this[QuestManager.QuestType type] { get { return data[(int)type]; } }

        public QuestSaveData()
        {
            isLoaded = false;
        }

        public void SetDataSize()
        {
            QuestManager questManager = GameManager.manager.questManager;

            stagePerformOneData = new BitSaveData(questManager.GetQuestCount(QuestManager.QuestType.StagePerformOnlyOne));
            villigePerformOneData = new BitSaveData(questManager.GetQuestCount(QuestManager.QuestType.VilligePerformOnlyOne));
            floorQuestData = new BitSaveData(questManager.GetQuestCount(QuestManager.QuestType.FloorQuest));
            villigeQuestData = new BitSaveData(questManager.GetQuestCount(QuestManager.QuestType.VilligeQuest));

            stagePerformOneData.bits[0] = 1;
            villigePerformOneData.bits[0] = 1;
        }
        public void Init()
        {
            data = new BitSaveData[] { stagePerformOneData, villigePerformOneData, floorQuestData, villigeQuestData };
        }


        public SaveDataBit GetQuestState(QuestManager.QuestType type, int questNum)
        {
            return data[(int)type].GetQuestState(questNum);
        }
        public void SetQuestState(QuestManager.QuestType type, int questNum, SaveDataBit bit)
        {
            data[(int)type].AddQuestState(questNum, bit);
        }
        public void SetStageQuest()
        {
            QuestManager questManager = GameManager.manager.questManager;
            floorQuestData = new BitSaveData(questManager.GetQuestCount(QuestManager.QuestType.FloorQuest));
            floorQuestData.bits[0] = 1;
        }
    }
    [Serializable]
    public class PlayInfo
    {
        public Vector3 camPosition;
        public int[] enableUpgrades;
        public int canSummonHeroCount;
        public SummonHeroData[] canSummonHero;
        public bool isEnter;

        public PlayInfo()
        {
            camPosition = new Vector3();
            ResetPosition();
            enableUpgrades = new int[] { 1, 1, 1, 1, 1, 1, 1 };
            canSummonHero = null;
            canSummonHeroCount = 2;
            isEnter = true;
        }
        public void SaveData()
        {
            camPosition.Set(Camera.main.transform.position.x, 10, Camera.main.transform.position.z);
        }
        public void ResetPosition()
        {
            camPosition.Set(0.25f, 10f, -9.38f);
        }
    }
    [Serializable]
    public class SummonHeroData
    {
        public HeroData heroData;
        public bool isSummoned;
        public SummonHeroData(HeroData getData)
        {
            heroData = getData;
            isSummoned = false;
        }
    }
    [Serializable]
    public class FloorData
    {
        public int topFloor;
        public int[] floorLooks;
        public float[] floorAngles;

        public FloorData()
        {
            topFloor = 1;

            //floorLooks = new int[9] { 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            floorLooks = new int[1] { 0 };
            //floorAngles = new float[9] { 27, 47, 180, -46, 27, 60, 90, -127, 50 };
            floorAngles = new float[1] { 27 };
        }
        public void SetData(int floorLevel, int[] nowTowerLooks, float[] nowAngles)
        {
            topFloor = floorLevel;
            floorLooks = nowTowerLooks;
            floorAngles = nowAngles;

        }
    }
}
#endregion
public class LoadSaveManager : JsonSaveLoad
{

    #region 데이터 체크
    public bool IsValidData(int index)
    {
        return SaveDataExist(index);
    }
    public void DataNowFloor(int index, out int floor, out int day)
    {
        SaveDataFloor(index, out floor, out day);
    }
    #endregion
    #region 데이터 관리
    public void LoadData(int index, out SaveDataInfo info)
    {
        info = LoadSave<SaveDataInfo>(index);
    }
    public void DeleteSaveFile(int index)
    {
        DeleteSave(GetSaveFileName(index));
    }
    public void OverrideSaveFile(int index, SaveDataInfo info)
    {
        SaveData(info, GetSaveFileName(index));
    }

    string GetSaveFileName(int index)
    {
        return "save" + (index + 1).ToString();
    }
    #endregion
    [ContextMenu("json파일 생성")]
    public void SDF()
    {
        SDF<SaveDataInfo>();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
