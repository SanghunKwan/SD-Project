using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUICallChangeWithBool : CheckUICallChange
{
    [SerializeField] CheckUICondition[] conditions;
    bool isTrue
    {
        get
        {
            bool tempBool = true;
            foreach (var item in conditions)
            {
                if (item.gameObject.activeSelf)
                    tempBool &= item.condition;
            }
            return tempBool;
        }
    }

    public void CheckNowIndex()
    {
        nowIndex = Convert.ToInt32(isTrue);
        SetUICall();
    }
}
