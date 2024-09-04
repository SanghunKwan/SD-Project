using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponent : MonoBehaviour
{
    BuildingSetWindow buildingWindow;
    [SerializeField] AddressableManager.BuildingImage type;
    [SerializeField] AddressableManager.BuildingImage upgradeType;
    [SerializeField] string infoText;
    bool isWindowOpen;
    Camera camMain;

    villigeInteract[] saveVilligeInteract = new villigeInteract[3];

    private void Awake()
    {
        camMain = Camera.main;
        buildingWindow = GameObject.FindWithTag("UI").transform.Find("BuildingSetWindow").GetComponent<BuildingSetWindow>();
    }

    private void OnMouseUpAsButton()
    {
        ToggleWindow();
    }
    public void ToggleWindow()
    {
        bool wasWindowOpen = isWindowOpen;
        isWindowOpen = (!isWindowOpen) || buildingWindow.buildingComponent != this;

        if (wasWindowOpen != isWindowOpen)
            SetCam(isWindowOpen);

        SetWindowOpen(isWindowOpen);
    }
    void SetWindowOpen(bool onoff)
    {
        buildingWindow.SetOpen(this, onoff, type, upgradeType, infoText, saveVilligeInteract);
    }
    void SetCam(bool onoff)
    {
        if (!onoff)
        {
            GetCamPosition(-20, 5);
        }
        else
        {
            GetCamPosition(20, 2);
        }
    }
    void GetCamPosition(int angle, int size)
    {
        //camMain.transform.RotateAround(camMain.transform.position, Vector3.left, angle);

        //camMain.transform.position = new Vector3
        //    (camMain.transform.position.x,
        //    -camMain.transform.position.y * Mathf.Cos(angle) + Mathf.Sin(angle) * camMain.transform.position.z,
        //    camMain.transform.position.y * Mathf.Sin(angle) + Mathf.Cos(angle) * camMain.transform.position.z);

        camMain.orthographicSize = size;
    }

    public void SaveData(villigeInteract vil_interact, int index)
    {
        saveVilligeInteract[index] = vil_interact;
    }
    public void ResetData(int index)
    {
        saveVilligeInteract[index] = null;
    }
}
