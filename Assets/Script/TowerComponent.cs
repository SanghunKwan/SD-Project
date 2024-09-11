using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerComponent : ClickCamTurningComponent
{
    TowerWindow towerWindow;

    protected override void Awake()
    {
        base.Awake();
        towerWindow = camTurningWindow as TowerWindow;
    }
    protected override void SetMemory()
    {

    }

    protected override void SetWindowOpen()
    {

        towerWindow.SetOpen(isWindowOpen, type);
    }
}
