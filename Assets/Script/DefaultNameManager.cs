using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DefaultNameManager : MonoBehaviour
{
    public static DefaultNameManager mananger;

    public string[] names { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        mananger = this;

        string nameStr = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "DataTable/DefaultName.csv"));

        names = nameStr.Split(",");

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = names[i].Replace("\"", "");
        }
    }

    public void GetRandomName(out string name)
    {
        int index = Random.Range(0, names.Length);
        name = names[index];
    }
}
