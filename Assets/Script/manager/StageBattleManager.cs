using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class StageBattleManager : StageManager
{
    protected override void VirtualAwake()
    {
#if UNITY_EDITOR
        base.VirtualAwake();
#endif
    }
}
