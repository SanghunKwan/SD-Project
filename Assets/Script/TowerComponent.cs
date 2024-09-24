using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerComponent : ClickCamTurningComponent
{
    TowerWindow towerWindow;
    SkyScraperComponent skyScraperComponent;

    protected override void Awake()
    {
        base.Awake();
        towerWindow = camTurningWindow as TowerWindow;
        skyScraperComponent = GetComponent<SkyScraperComponent>();
    }
    protected override void SetMemory()
    {

    }

    protected override void SetWindowOpen()
    {

        towerWindow.SetOpen(isWindowOpen, type);
        skyScraperComponent.SetTransparent(false);

    }
}
