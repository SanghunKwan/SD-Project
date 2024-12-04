using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

[RequireComponent(typeof(BattleClearPool))]
public class BattleClearManager : MonoBehaviour
{
    public enum OBJECTNUM
    {
        BONEWALL,
    }
    private void Start()
    {
        GameManager.manager.battleClearManager = this;
    }

    public CObject CallObject(OBJECTNUM num, Transform trans)
    {
        CObject obj = transform.GetChild((int)num).GetChild(0).GetComponent<CObject>();
        obj.gameObject.SetActive(true);
        obj.transform.position = trans.position;
        obj.transform.SetParent(transform);

        return obj;
    }
}
