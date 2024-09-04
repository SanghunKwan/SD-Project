using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class QuirkInfo : MonoBehaviour
{
    TextMeshProUGUI[] details;
    


    private void Awake()
    {
        details = transform.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void Print(int infoindex, QuirkChange.quirkType type)
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();
        dic.EnsureCapacity(12);

        if (type == QuirkChange.quirkType.quirk)
        {
            AddDic(QuirkData.manager.quirkInfo);
        }
        else
        {
            AddDic(QuirkData.manager.diseaseInfo);
        }

        var posDic = dic.Where(val => val.Value > 0).ToDictionary(key => key.Key, val => val.Value);
        var negDic = dic.Where(val => val.Value < 0).ToDictionary(key => key.Key, val => val.Value);



        int i = 0;
        DetailTextChange(posDic, ref i);

        DetailTextChange(negDic, ref i);


        void AddDic(QuirkData.QuirkS quirkS)
        {
            dic.Add("체력", quirkS.quirks[infoindex].HP);
            dic.Add("회피", quirkS.quirks[infoindex].DOG);
            dic.Add("공격력", quirkS.quirks[infoindex].ATK);
            dic.Add("이동속도", quirkS.quirks[infoindex].SPEED);
            dic.Add("방어력", quirkS.quirks[infoindex].DEF);
            dic.Add("시야범위", quirkS.quirks[infoindex].ViewAngle);
            dic.Add("시야각", quirkS.quirks[infoindex].ViewRange);
            dic.Add("명중률", quirkS.quirks[infoindex].Accuracy);
            dic.Add("공격속도", quirkS.quirks[infoindex].AtkSpeed);
            dic.Add("사거리", quirkS.quirks[infoindex].Range);
            dic.Add("정신력", quirkS.quirks[infoindex].Mentality);
            dic.Add("사기공격", quirkS.quirks[infoindex].Stress);
        }

        string ToString(int figure)
        {
            if (figure > 0)
                return "+" + figure.ToString() + "%";
            else if (figure < 0)
                return figure.ToString() + "%";
            else
                throw new System.Exception("quirk 안내 오류");
        }

        void DetailTextChange(in Dictionary<string, int> dic, ref int i)
        {
            int lastCount = i + 3;
            foreach (var item in dic)
            {
                details[i].text = item.Key + " " + ToString(item.Value);
                i++;
            }

            while(i < lastCount)
            {
                details[i].text = "";
                i++;
            }
        }

    }
    


}
