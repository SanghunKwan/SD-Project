using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SetBuildingMat : MonoBehaviour
{
    [SerializeField] MaterialsData manager;
    TextMeshProUGUI title;

    [Serializable]
    public class Materials
    {
        public GameObject[] gameobj;
        public TextMeshProUGUI numText;
    }
    [SerializeField] Materials[] materials;
    TextMeshProUGUI gold;
    TextMeshProUGUI time;
    TextMeshProUGUI description;

    private void Awake()
    {
        title = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        gold = title.transform.Find("Gold&Time").Find("GoldText").GetComponent<TextMeshProUGUI>();
        time = gold.transform.parent.Find("TimeText").GetComponent<TextMeshProUGUI>();
        description = title.transform.Find("desc").GetComponent<TextMeshProUGUI>();
    }

    public void GetDate(int i)
    {
        SetActiveMaterialsDesc(0, manager.data.Needs[i].grayNum);
        SetActiveMaterialsDesc(1, manager.data.Needs[i].timberNum);
        SetActiveMaterialsDesc(2, manager.data.Needs[i].blackNum);
        SetActiveMaterialsDesc(3, manager.data.Needs[i].whiteNum);
        gold.text = manager.data.Needs[i].money.ToString();
        time.text = manager.data.Needs[i].turn.ToString();
        description.text = manager.data.Needs[i].desc;
        title.text = manager.data.Needs[i].name;
    }

    void SetActiveMaterialsDesc(int i, int numCount)
    {
        bool onoff = !numCount.Equals(0);

        for (int j = 0; j < materials[i].gameobj.Length; j++)
        {
            materials[i].gameobj[j].SetActive(onoff);
        }
        materials[i].numText.gameObject.SetActive(onoff);

        if (onoff)
            materials[i].numText.text = numCount.ToString();

    }
}
