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
        public FloorManager.FloorData floorData;

        public HeroData[] hero;
        public BuildingData[] building;

        public int[] items;

        public StageData stageData;
        public QuestSaveData questSaveData;


        public SaveDataInfo()
        {
            day = 0;
            nextScene = 1;
            hero = new HeroData[1] { new HeroData() };
            floorData = new FloorManager.FloorData();
            stageData = new StageData();
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
        public QuirkDefaultData(int count)
        {
            quirks = new int[count];
            length = 0;
        }
        public QuirkDefaultData(in int[] getQuirks, int getLength)
        {
            quirks = getQuirks;
            length = getLength;
        }
    }
    [Serializable]
    public class QuirkSaveData : QuirkDefaultData
    {
        public bool willChange;
        public int willChangeIndex;

        public QuirkSaveData()
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
        public int[] skillNum;
        public int villigeAction;
        public int workBuilding;
        public bool isDead;
        public UnitData unitData;

        public HeroData()
        {
            name = "디스마스";
            lv = 1;
            quirks = new QuirkSaveData();
            disease = new QuirkDefaultData(4);
            keycode = "=";
            equipNum = new int[3] { 1, 1, 1 };
            skillNum = new int[4] { 1, 1, 1, 1 };
            villigeAction = 0;
            workBuilding = 0;
            isDead = false;
            unitData = new UnitData();
        }
    }
    public class BuildingData
    {
        public int[] workHero = new int[3];
        public ObjectData objectData;
    }
    [Serializable]
    public class StageData
    {
        public int[] heros;
        public int[] floors = new int[5];
        public int nowFloorIndex;
        public bool isClear;

        public InventoryStorage.Slot[] slots;

        public UnitData[] unitData;
        public MonsterData[] monsterData;
        public ObjectData[] objectDatas;
        public DropItemData[] dropItemDatas;
        public StageData()
        {
            heros = new int[1] { 0 };
            floors = new int[] { 0 };
            nowFloorIndex = 0;
            isClear = false;
        }
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
    }
    [Serializable]
    public class ObjectData
    {
        public Vector3 position;
        public Vector3 dotsDirection;
        public Quaternion quaternion;
        public bool selected;
        public unit_status cur_status;
        public int id;
        public int[] dots;
        public ObjectData()
        {
            position = Vector3.forward;
            quaternion = Quaternion.Euler(0, 180, 0);
            selected = false;
            id = 101;
            cur_status = new unit_status();
            dots = new int[(int)SkillData.EFFECTINDEX.MAX] { 0, 0, 0, 0 };
        }
        public ObjectData(CObject cObject)
        {
            position = cObject.transform.position;
            quaternion = cObject.transform.rotation;
            selected = cObject.selected;
            cur_status = cObject.curstat;
            id = cObject.id;
            dots = cObject.dots;
        }

    }
    [Serializable]
    public class DropItemData
    {
        public Vector3 position;
        public int index;
        public DropItemData(ItemComponent itemComponent)
        {
            position = itemComponent.transform.position;
            index = itemComponent.Index;
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
    public void CreateSaveFile(int index)
    {
        SaveData(new SaveDataInfo(), "save" + (index + 1).ToString());
    }
    public void DeleteSaveFile(int index)
    {
        DeleteSave("save" + (index + 1).ToString());
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
