

using UnityEngine;

public class TowerComponent : ClickCamTurningComponent
{
    TowerWindow towerWindow;
    SkyScraperComponent skyScraperComponent;
    ObjectAssembly objectAssembly;
    TowerAssembleClick[] towerAssembleClicks;

    protected override void Awake()
    {
        base.Awake();
        towerWindow = camTurningWindow as TowerWindow;
        skyScraperComponent = GetComponent<SkyScraperComponent>();
        objectAssembly = GetComponent<ObjectAssembly>();
        objectAssembly.init2 += AssmeblyInit;
    }
    protected override void SetMemory()
    {

    }
    void AssmeblyInit(TowerAssembleClick[] clicks)
    {
        towerAssembleClicks = clicks;
    }
    protected override void SetWindowOpen()
    {
        towerWindow.SetOpen(isWindowOpen, type);
        skyScraperComponent.SetTransparent(false);

        foreach (var item in towerAssembleClicks)
            item.SetColliderActive(isWindowOpen);
    }
    public void AssembleClick(int floorNum)
    {
        towerWindow.PopUpWindow();
    }
}
