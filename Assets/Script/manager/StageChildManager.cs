using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChildManager : StageManager
{
    [SerializeField] int editorIndex;
    protected override void VirtualAwake()
    {
#if UNITY_EDITOR
        base.VirtualAwake();
        saveDataIndex = editorIndex;
#endif
    }
}
