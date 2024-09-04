using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;



public class DropInfo
{
    public int ID;
    public string NAME;
    public Materials material;
    public int MaxNum;
    public float Percentage;
    public float PercentageEquips;


    public DropInfo(Dictionary<string, string> Dic)
    {
        ID = int.Parse(Dic["ID"]);
        NAME = Dic["NAME"];
        material = (Materials)System.Enum.Parse(typeof(Materials), Dic["Materials"]);
        MaxNum = int.Parse(Dic["MaxNum"]);
        Percentage = float.Parse(Dic["Percentage"]);
        PercentageEquips = float.Parse(Dic["PercentageEquips"]);
    }
}
public enum Materials
{
    Gray,
    Black,
    White,
    Timber,
    Weapon,
    Berry,
    Olive,
    Apple,
    Banana,
    Tomato,
    GreenFruit
}
public class DropManager : MonoBehaviour
{
    public static DropManager instance;

    public List<Dictionary<string, string>> Objectlist = new List<Dictionary<string, string>>();
    public ItemPool pool { get; private set; }

    private void Awake()
    {
        pool = GetComponent<ItemPool>();
        instance = this;

        string Item_Data = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\DataTable\Drop_Data.csv");

        using(StreamReader reader = new StreamReader(Item_Data))
        {
            string sLine = reader.ReadLine();
            sLine = sLine.Replace("\"", "");
            string[] sList = sLine.Split(',');
            sLine = reader.ReadLine();
            Dictionary<string, string> dDetail = new Dictionary<string, string>();
            while (sLine != null)
            {
                sLine = sLine.Replace("\"", "");
                string[] sDetail  = sLine.Split(',');
                for (int i = 0; i < sList.Length; i++)
                {
                    dDetail.Add(sList[i], sDetail[i]);
                }
                Objectlist.Add(new Dictionary<string, string>(dDetail));
                dDetail.Clear();
                sLine = reader.ReadLine();
            }

        }
    }
    public DropInfo GetDropInfo(int id)
    {
        var asdf = from dic in Objectlist
                   where dic["ID"].Equals(id.ToString())
                   select dic;

        foreach (Dictionary<string, string> item in asdf)
        {
            return new DropInfo(item);
        }
        Debug.Log("버그 발생");
        return null;
    }

}
