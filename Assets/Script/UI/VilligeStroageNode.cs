using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VilligeStroageNode : InitObject
{
    VilligeStorage storage;
    TextMeshProUGUI text;
    [SerializeField] int m_itemType;
    public int ItemType { get { return m_itemType; } }

    public override void Init()
    {
        storage = transform.parent.GetComponent<VilligeStorage>();
        text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        storage.OnItemCountChanged[m_itemType] += CountChanged;
    }
    void CountChanged(int count)
    {
        text.text = count.ToString();
    }


}
