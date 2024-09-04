using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;


public class MonNavi : MonoBehaviour
{
    List<UnitMove> unitMoves = new List<UnitMove> ();

    public void MonsterAdd(CUnit unit)
    {
        unitMoves.Add(unit.unitMove);
    }
    public void MonsterRemove(CUnit unit)
    {
        unitMoves.Remove(unit.unitMove);
    }
}
