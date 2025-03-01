using System;
using System.Collections;
using UnityEngine;

public class TowerComponent : ClickCamTurningComponent
{
    TowerWindow towerWindow;
    SkyScraperComponent skyScraperComponent;
    ObjectAssembly objectAssembly;
    TowerAssembleClick[] towerAssembleClicks;

    public Action windowEnd { get; set; } = () => { };

    protected override void VirtualAwake()
    {
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
        skyScraperComponent.SetTransparent(1);

        SetAssembleCollierActive(isWindowOpen);
    }
    public void SetAssembleCollierActive(bool onoff)
    {
        foreach (var item in towerAssembleClicks)
            item.SetColliderActive(onoff);
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
    public void ChangeAngle(int floor, int heightType, int angle)
    {
        GetMove(floor, angle, heightType, out float yValue);
        StartInterpolation(StraightIenum(3, angle, yValue, 0, -2));
    }
    public void ChangeAngle(int angle)
    {
        GetMove(angle, out float yValue, out float zValue);
        StartInterpolation(StraightIenum(5, angle, yValue, zValue));

        isWindowOpen = false;
        camTurningWindow.Collider_UIActive(!isWindowOpen);

        SetColliderActive(!isWindowOpen);
        SetWindowOpen();

        windowEnd = () =>
        {
            camMain.cullingMask += 32;
            camMain.cullingMask += 4096;
            foreach (int layer in cullingLayers)
                camMain.cullingMask += (int)Mathf.Pow(2, layer);
        };
    }
    IEnumerator StraightIenum(float targetSize, float targetAngle, float yValue, float zValue = 0, float xValue = 0)
    {
        float timeCheck = 0;
        float deltaCam = targetSize - camMain.orthographicSize;
        float deltaAngle = targetAngle - camMain.transform.eulerAngles.x;
        Vector3 addVector = new Vector3(xValue - camMain.transform.position.x, yValue, zValue);
        CamTuringWindow.transformObject = new GameObject();
        do
        {
            camMain.transform.position += addVector * Time.deltaTime;
            camMain.orthographicSize += deltaCam * Time.deltaTime;
            camMain.transform.eulerAngles += deltaAngle * Time.deltaTime * Vector3.right;
            timeCheck += Time.deltaTime;

            yield return null;
        } while (timeCheck < 1);
        Destroy(CamTuringWindow.transformObject);
        windowEnd();
        windowEnd = () => { };
    }
    void GetMove(int floor, int angle, int heightType, out float yValue)
    {
        float height = (-camMain.transform.position.z * Mathf.Tan(Mathf.Deg2Rad * angle))
                        + towerAssembleClicks[floor].transform.position.y + objectAssembly.floors[heightType].height * 0.5f;

        yValue = height - camMain.transform.position.y;
    }
    void GetMove(int angle, out float yValue, out float zValue, float defaultHeight = 10)
    {
        float height = defaultHeight + transform.position.y;
        yValue = height - camMain.transform.position.y;
        zValue = (-height / MathF.Tan(Mathf.Deg2Rad * angle)) - camMain.transform.position.z;
    }
    public void CamBack()
    {
        StartInterpolation(StraightIenum(30, 10, 30 - camMain.transform.position.y, -70 - camMain.transform.position.z));

    }
}