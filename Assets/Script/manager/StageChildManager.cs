using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChildManager : StageManager
{
    protected override void VirtualAwake()
    {
#if UNITY_EDITOR
        base.VirtualAwake();
        saveDataIndex = 1;
#endif
    }
}
