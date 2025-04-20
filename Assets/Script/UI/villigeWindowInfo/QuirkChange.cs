using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuirkChange : MonoBehaviour
{

    QuirkRemem[] quirkRemems;
    [SerializeField] quirkType type;
    public enum quirkType
    {
        quirk = 0,
        disease = 1
    }

    public void Awake()
    {
        quirkRemems = transform.GetChild(1).GetComponentsInChildren<QuirkRemem>();

        foreach (var item in quirkRemems)
        {
            item.RegistQuirkType(type);
        }
    }

    public void SetQuirk(in QuirkData.Quirk[] quirks)
    {
        for (int i = 0; i < quirkRemems.Length; i++)
        {
            quirkRemems[i].QuirkRemember(quirks[i].index);
        }
    }
    public void SetQuirk(in SaveData.QuirkDefaultData quirks)
    {
        for (int i = 0; i < quirkRemems.Length; i++)
        {
            quirkRemems[i].QuirkRemember(quirks.quirks[i]);
        }
    }


}
