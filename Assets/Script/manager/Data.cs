using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using static EquipDataManager;

namespace Unit
{
    public enum Species
    {
        Goblin,
        Object = 100,
        Building = 200,
        player = 1,
        MAX = 4
    }
    public enum TypeNum
    {
        Melee = 0,
        Range,
        Magic,
        Surprise,
        Boss,
        Tank,
        Object = 100,
        Building = 200,
        PlayerTypeLength = 3
    }
    public enum WaitingTypeNum
    {
        Wandering = 0,
        Warden,
        Patrol
    }
    [Serializable]
    public class unit_status
    {
        public int ID;
        public string NAME;
        public Species SPECIES;
        public int HP;
        public int ATK;
        public int DEF;
        public int DOG;
        public int MORALE;
        public int SPEED;
        public int ViewAngle;
        public int ViewRange;
        public TypeNum type;
        public int Accuracy;
        public float AtkSpeed;
        public int Range;
        public int Mentality;
        public int Stress;

        public int curHP;
        public int curMORALE;

        public unit_status()
        {
            ID = 301;
            NAME = "playerMelee";
            SPECIES = Species.player;
            HP = 27;
            ATK = 7;
            DEF = 0;
            DOG = 10;
            MORALE = 0;
            SPEED = 5;
            ViewAngle = 90;
            ViewRange = 6;
            type = TypeNum.Melee;
            Accuracy = 85;
            AtkSpeed = 1.5f;
            Range = 1;
            Mentality = 30;
            Stress = 5;

            curHP = 27;
            curMORALE = 0;
        }
        public unit_status(int key, in string[] csvDatas)
        {
            if (csvDatas.Length == 16)
            {
                Debug.Log("데이터를 확인해주세요");
                return;
            }
            ID = key;
            NAME = csvDatas[1];
            SPECIES = Enum.Parse<Species>(csvDatas[2]);
            HP = int.Parse(csvDatas[3]);
            ATK = int.Parse(csvDatas[4]);
            DEF = int.Parse(csvDatas[5]);
            DOG = int.Parse(csvDatas[6]);
            MORALE = int.Parse(csvDatas[7]);
            SPEED = int.Parse(csvDatas[8]);
            ViewAngle = int.Parse(csvDatas[9]);
            ViewRange = int.Parse(csvDatas[10]);
            type = Enum.Parse<TypeNum>(csvDatas[11]);
            Accuracy = int.Parse(csvDatas[12]);
            AtkSpeed = float.Parse(csvDatas[13]);
            Range = int.Parse(csvDatas[14]);
            Mentality = int.Parse(csvDatas[15]);
            Stress = int.Parse(csvDatas[16]);
        }
        public void Clone(unit_status status)
        {
            ID = status.ID;
            NAME = status.NAME;
            SPECIES = status.SPECIES;
            HP = status.HP;
            ATK = status.ATK;
            DEF = status.DEF;
            DOG = status.DOG;
            MORALE = status.MORALE;
            SPEED = status.SPEED;
            ViewAngle = status.ViewAngle;
            ViewRange = status.ViewRange;
            type = status.type;
            Accuracy = status.Accuracy;
            AtkSpeed = status.AtkSpeed;
            Range = status.Range;
            Mentality = status.Mentality;
            Stress = status.Stress;

            curHP = status.curHP;
            curMORALE = status.curMORALE;
        }

        public void Read(Dictionary<string, string> CSVReader)
        {
            ID = int.Parse(CSVReader["ID"]);
            NAME = CSVReader["NAME"];
            SPECIES = Enum.Parse<Species>(CSVReader["Speices"]);
            HP = int.Parse(CSVReader["HP"]);
            DEF = int.Parse(CSVReader["DEF"]);
            ATK = int.Parse(CSVReader["ATK"]);
            DOG = int.Parse(CSVReader["DOG"]);
            MORALE = int.Parse(CSVReader["MORALE"]);
            SPEED = int.Parse(CSVReader["SPEED"]);
            ViewAngle = int.Parse(CSVReader["ViewAngle"]);
            ViewRange = int.Parse(CSVReader["ViewRange"]);
            type = Enum.Parse<TypeNum>(CSVReader["Type"]);
            Accuracy = int.Parse(CSVReader["Accuracy"]);
            AtkSpeed = float.Parse(CSVReader["AtkSpeed"]);
            Range = int.Parse(CSVReader["Range"]);
            Mentality = int.Parse(CSVReader["Mentality"]);
            Stress = int.Parse(CSVReader["Stress"]);
        }
        public void RefreshStatus(unit_status originalStat, int equipDataIndex)
        {
            EquipValueData data = Data.Instance.equipDataManager.GetEquipData(equipDataIndex);

            HP = originalStat.HP + data.HP;
            ATK = originalStat.ATK + data.ATK;
            DOG = originalStat.DOG + data.DOG;
            SPEED = originalStat.SPEED + data.SPEED;
            ViewAngle = originalStat.ViewAngle + data.ViewAngle;
            ViewRange = originalStat.ViewRange + data.ViewRange;
            Accuracy = originalStat.Accuracy + data.Accuracy;
            AtkSpeed = originalStat.AtkSpeed + data.AtkSpeed;
            Range = originalStat.Range + data.Range;
            Mentality = originalStat.Mentality + data.Mentality;
            Stress = originalStat.Stress + data.Stress;

            curHP += data.HP;
        }
        public void QuirkDiseaseCalculate(in QuirkData.Quirk[] quirkArray, in QuirkData.Quirk[] diseaseArray)
        {
            //질병 및 기벽 반영

            QuirkData.Quirk quirkSum = new QuirkData.Quirk();

            foreach (var item in quirkArray)
            {
                AddQuirkNum(item, ref quirkSum);
            }

            foreach (var item in diseaseArray)
            {
                AddQuirkNum(item, ref quirkSum);
            }
            //기벽 반영 시 curStat에 곱연산을 마지막으로 진행.
            CalculateQuirk(quirkSum);

        }
        void AddQuirkNum(in QuirkData.Quirk quirk, ref QuirkData.Quirk quirkSum)
        {
            quirkSum.HP += quirk.HP;
            quirkSum.ATK += quirk.ATK;
            quirkSum.DEF += quirk.DEF;
            quirkSum.DOG += quirk.DOG;
            quirkSum.SPEED += quirk.SPEED;
            quirkSum.ViewAngle += quirk.ViewAngle;
            quirkSum.ViewRange += quirk.ViewRange;
            quirkSum.Accuracy += quirk.Accuracy;
            quirkSum.AtkSpeed += quirk.AtkSpeed;
            quirkSum.Range += quirk.Range;
            quirkSum.Mentality += quirk.Mentality;
            quirkSum.Stress += quirk.Stress;
        }
        void CalculateQuirk(QuirkData.Quirk quirkSum)
        {
            MakeQuirkNum(ref HP, quirkSum.HP);
            MakeQuirkNum(ref ATK, quirkSum.ATK);
            MakeQuirkNum(ref DEF, quirkSum.DEF);
            MakeQuirkNum(ref DOG, quirkSum.DOG);
            MakeQuirkNum(ref SPEED, quirkSum.SPEED);
            MakeQuirkNum(ref ViewAngle, quirkSum.ViewAngle);
            MakeQuirkNum(ref ViewRange, quirkSum.ViewRange);
            MakeQuirkNum(ref Accuracy, quirkSum.Accuracy);
            MakeQuirkNum(ref AtkSpeed, quirkSum.AtkSpeed);
            MakeQuirkNum(ref Range, quirkSum.Range);
            MakeQuirkNum(ref Mentality, quirkSum.Mentality);
            MakeQuirkNum(ref Stress, quirkSum.Stress);
        }
        void MakeQuirkNum(ref float statNum, int quirkSumNum)
        {
            float multiplyNum = (100 + quirkSumNum) / 10f;
            if (quirkSumNum >= 0)
                statNum = Mathf.Ceil(statNum * multiplyNum) / 10f;
            else
                statNum = Mathf.Floor(statNum * multiplyNum) / 10f;
        }
        void MakeQuirkNum(ref int statNum, int quirkSumNum)
        {
            statNum = Mathf.CeilToInt(statNum * (1 + (quirkSumNum / 100f)));
        }

    }


    public class Data : JsonSaveLoad
    {
        public List<Dictionary<string, string>> Goblinlist = new List<Dictionary<string, string>>();
        public Dictionary<int, unit_status> statusList { get; private set; } = new Dictionary<int, unit_status>();

        public static Data Instance;

        public EquipDataManager equipDataManager { get; set; }

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;

            string Goblin_Data = Path.Combine(GetFolder("DataTable"), @"Monster_Data.csv");
            //ReadCSV list = new ReadCSV();
            using (StreamReader MonsterData = new StreamReader(Goblin_Data))
            {
                string str_goblin_Data = MonsterData.ReadLine();
                str_goblin_Data = str_goblin_Data.Replace("\"", "");
                string[] itemName = str_goblin_Data.Split(',');

                string[] itemDetail;
                str_goblin_Data = MonsterData.ReadLine();
                int key;
                while (str_goblin_Data != null)
                {
                    str_goblin_Data = str_goblin_Data.Replace("\"", "");
                    itemDetail = str_goblin_Data.Split(',');

                    key = int.Parse(itemDetail[0]);
                    statusList.Add(key, new unit_status(key, itemDetail));

                    str_goblin_Data = MonsterData.ReadLine();
                }
            }
            Debug.Log("monsterData 로딩완료");
        }
        //정보 호출
        public unit_status GetInfo(Species species, TypeNum num)
        {
            var specie = from status in statusList.Values
                         where status.SPECIES.Equals(species)
                         where status.type.Equals(num)
                         select status;

            return specie.First();
        }
        public unit_status GetInfo(int ID)
        {
            return statusList[ID];
        }

        public Vector3 CameratoCanvas(Vector3 position)
        {
            return Camera.main.WorldToScreenPoint(position) + Vector3.left * Screen.width * 0.5f + Vector3.down * Screen.height * 0.5f;
        }
        public Vector3 UItoCanvas(Vector3 position)
        {
            return position + Vector3.left * Screen.width * 0.5f + Vector3.down * Screen.height * 0.5f;
        }
        public Vector3 CanvastoUI(Vector3 position)
        {
            return position + Vector3.right * Screen.width * 0.5f + Vector3.up * Screen.height * 0.5f;
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }
    }
}