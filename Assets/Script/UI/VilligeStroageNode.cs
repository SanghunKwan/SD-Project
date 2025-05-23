using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VilligeStroageNode : InitObject
{
    VilligeStorage storage;
    TextMeshProUGUI text;
    [SerializeField] int m_itemType;
    Animator anim;
    static int animHighLight = Animator.StringToHash("HighLight");
    public int ItemType { get { return m_itemType; } }

    public override void Init()
    {
        storage = transform.parent.GetComponent<VilligeStorage>();
        anim = GetComponent<Animator>();
        text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        storage.OnItemCountChanged[m_itemType] += CountChanged;
    }
    void CountChanged(int count)
    {
        text.text = count.ToString("N0");
        Debug.Log("텍스트 변경2");
    }
    public void PlayHighLight()
    {
        anim.SetTrigger(animHighLight);
    }

}
