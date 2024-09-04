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
        public int Lv;

        public unit_status()
        {
            ID = 0;
            NAME = "";
            HP = 0;
            ATK = 0;
            DEF = 0;
            DOG = 0;
            MORALE = 0;
            SPEED = 0;
            ViewAngle = 0;
            ViewRange = 0;
            type = "";
            Accuracy = 0;
            AtkSpeed = 0;
            Range = 0;
            Mentality = 0;
            Stress = 0;
            Lv = 1;
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
            newStatus.Lv = status.Lv;

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
            Lv = 1;
        }
    }


    public class Data : MonoBehaviour
    {
        public List<Dictionary<string, string>> Goblinlist = new List<Dictionary<string, string>>();

        public static Data Instance;
        string[] monsterSpecies = new string[] { "goblin", "player" };
        string[] type = new string[] { "Melee", "Range", "Magic", "Surprise", "Boss", "Tank" };

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
            string Goblin_Data = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\DataTable\Monster_Data.csv");
            //ReadCSV list = new ReadCSV();
            Dictionary<string, string> ReadCSV = new Dictionary<string, string>();

            using (StreamReader MonsterData = new StreamReader(Goblin_Data))
            {
                string str_goblin_Data = MonsterData.ReadLine();
                str_goblin_Data = str_goblin_Data.Replace("\"", "");
                string[] qwer = str_goblin_Data.Split(',');

                str_goblin_Data = MonsterData.ReadLine();
                while (str_goblin_Data != null)
                {
                    str_goblin_Data = str_goblin_Data.Replace("\"", "");
                    string[] uiop = str_goblin_Data.Split(',');
                    for (int i = 0; i < uiop.Length; i++)
                    {
                        ReadCSV.Add(qwer[i], uiop[i]);
                    }
                    str_goblin_Data = MonsterData.ReadLine();
                    Goblinlist.Add(new Dictionary<string, string>(ReadCSV));
                    ReadCSV.Clear();
                }
            }

            Debug.Log("로딩완료");
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