using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class QuirkData : MonoBehaviour
{
    public static QuirkData manager;
    public QuirkS quirkInfo { get; private set; }
    public QuirkS diseaseInfo { get; private set; }
    

    [System.Serializable]
    public class Quirk
    {
        public int index;
        public string name;
        public int HP;
        public int ATK;
        public int DEF;
        public int DOG;
        public int SPEED;
        public int ViewAngle;
        public int ViewRange;
        public int Accuracy;
        public int AtkSpeed;
        public int Range;
        public int Mentality;
        public int Stress;

    }
    [System.Serializable]
    public class QuirkS
    {
        public Quirk[] quirks;
    }
    private void Awake()
    {
        manager = this;
        string dataAddress = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/Quirk_Data.json"));

        quirkInfo = (QuirkS)JsonUtility.FromJson(dataAddress, typeof(QuirkS));
        


        string dataAddress2 = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Assets/DataTable/Disease_Data.json"));

        diseaseInfo = (QuirkS)JsonUtility.FromJson(dataAddress2, typeof(QuirkS));

    }





    [ContextMenu("JsonPrototype")]
    public void SDF()
    {
        Quirk asdf = new Quirk();
        QuirkS quirks = new QuirkS();
        quirks.quirks = new Quirk[3];
        quirks.quirks[0] = asdf;
        quirks.quirks[1] = asdf;
        quirks.quirks[2] = asdf;

        string wnth = Path.Combine(Application.dataPath, "DataTable/asdf.json");

        File.WriteAllText(wnth, JsonUtility.ToJson(quirks, true));

    }
}
