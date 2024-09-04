using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterView : View
{
    [SerializeField] Material enermyMaterial;
    protected override void OnEnable()
    {
        base.OnEnable();
        filter.GetComponent<MeshRenderer>().material = enermyMaterial;
        filter.gameObject.layer = 11;
    }
    protected override float HeadTurn()
    {
        return Head.transform.eulerAngles.y;
    }
}
