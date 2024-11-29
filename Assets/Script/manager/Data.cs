using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

namespace Unit
{
    public enum Species
    {
        Goblin,
        player,
        MAX
    }
    public enum TypeNum
    {
        Melee = 0,
        Range,
        Magic,
        Surprise,
        Boss,
        Tank
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
        public int HP;
        public int ATK;
        public int DEF;
        public int DOG;
        public int MORALE;
        public int SPEED;
        public int ViewAngle;
        public int ViewRange;
        public string type;
        public int Accuracy;
        public float AtkSpeed;
        public int Range;
        public int Mentality;
        public int Stress;

        public unit_status()
        {
            ID = 6;
            NAME = "디스마스";
            HP = 27;
            ATK = 7;
            DEF = 0;
            DOG = 10;
            MORALE = 0;
            SPEED = 5;
            ViewAngle = 90;
            ViewRange = 6;
            type = "Melee";
            Accuracy = 85;
            AtkSpeed = 1.5f;
            Range = 1;
            Mentality = 30;
            Stress = 5;
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
            HP = int.Parse(csvDatas[2]);
            DEF = int.Parse(csvDatas[3]);
            ATK = int.Parse(csvDatas[4]);
            DOG = int.Parse(csvDatas[5]);
            MORALE = int.Parse(csvDatas[6]);
            SPEED = int.Parse(csvDatas[7]);
            ViewAngle = int.Parse(csvDatas[8]);
            ViewRange = int.Parse(csvDatas[9]);
            type = csvDatas[10];
            Accuracy = int.Parse(csvDatas[11]);
            AtkSpeed = float.Parse(csvDatas[12]);
            Range = int.Parse(csvDatas[13]);
            Mentality = int.Parse(csvDatas[14]);
            Stress = int.Parse(csvDatas[15]);
        }
        public unit_status Clone(unit_status status)
        {
            unit_status newStatus = new();
            newStatus.ID = status.ID;
            newStatus.NAME = status.NAME;
            newStatus.HP = status.HP;
            newStatus.ATK = status.ATK;
            newStatus.DEF = status.DEF;
            newStatus.DOG = status.DOG;
            newStatus.MORALE = status.MORALE;
            newStatus.SPEED = status.SPEED;
            newStatus.ViewAngle = status.ViewAngle;
            newStatus.ViewRange = status.ViewRange;
            newStatus.type = status.type;
            newStatus.Accuracy = status.Accuracy;
            newStatus.AtkSpeed = status.AtkSpeed;
            newStatus.Range = status.Range;
            newStatus.Mentality = status.Mentality;
            newStatus.Stress = status.Stress;

            return newStatus;
        }

        public void Read(Dictionary<string, string> CSVReader)
        {
            ID = int.Parse(CSVReader["ID"]);
            NAME = CSVReader["NAME"];
            HP = int.Parse(CSVReader["HP"]);
            DEF = int.Parse(CSVReader["DEF"]);
            ATK = int.Parse(CSVReader["ATK"]);
            DOG = int.Parse(CSVReader["DOG"]);
            MORALE = int.Parse(CSVReader["MORALE"]);
            SPEED = int.Parse(CSVReader["SPEED"]);
            ViewAngle = int.Parse(CSVReader["ViewAngle"]);
            ViewRange = int.Parse(CSVReader["ViewRange"]);
            type = CSVReader["type"];
            Accuracy = int.Parse(CSVReader["Accuracy"]);
            AtkSpeed = float.Parse(CSVReader["AtkSpeed"]);
            Range = int.Parse(CSVReader["Range"]);
            Mentality = int.Parse(CSVReader["Mentality"]);
            Stress = int.Parse(CSVReader["Stress"]);
        }

    }


    public class Data : MonoBehaviour
    {
        public List<Dictionary<string, string>> Goblinlist = new List<Dictionary<string, string>>();
        public Dictionary<int, unit_status> statusList { get; private set; }

        public static Data Instance;
        string[] monsterSpecies = new string[] { "goblin", "player" };
        string[] type = new string[] { "Melee", "Range", "Magic", "Surprise", "Boss", "Tank" };

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;

            string Goblin_Data = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\DataTable\Monster_Data.csv");
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
            unit_status um = new();

            var specie = from Dictionary in Goblinlist
                         where Dictionary["NAME"].Equals(monsterSpecies[(int)species])
                         where Dictionary["type"].Equals(type[(int)num])
                         select Dictionary;

            IEnumerator<Dictionary<string, string>> enumerable = specie.GetEnumerator();

            if (enumerable.MoveNext())
            {
                um.Read(enumerable.Current);
            }
            return um;
        }
        public unit_status GetInfo(int ID)
        {
            unit_status um = new();

            var specie = from Dictionary in Goblinlist
                         where Dictionary["ID"].Equals(ID.ToString())
                         select Dictionary;

            foreach (var item in specie)
            {
                um.Read(item);
            }
            return um;
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


    }
}