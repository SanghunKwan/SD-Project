using System.Collections;
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
        AssembleClickReset();
        towerWindow.PopUpWindow(floorNum + 1);
    }
    public void AssembleClickReset()
    {
        for (int i = 0; i < towerAssembleClicks.Length; i++)
        {
            towerAssembleClicks[i].isLastClick = false;
        }
    }
    public float GetCanvasYfromFloor(int clickIndex, int floorLooks, float ratioInFloor)
    {

        return camMain.WorldToScreenPoint(towerAssembleClicks[clickIndex].transform.position
            + (objectAssembly.floors[floorLooks].height * ratioInFloor * Vector3.up)).y;
    }
    public void ChangeAngle(int floor)
    {
        GetMove(floor, out float yOffset);


        StartInterpolation(StraightIenum(yOffset));
    }
    IEnumerator StraightIenum(float yOffset)
    {
        
        do
        {

            //camMain.transform.position += vec;

            yield return null;
        } while (true);
    }
    void GetMove(int floor, out float yValue)
    {
        
        yValue = towerAssembleClicks[floor].transform.position.y;
    }

}
