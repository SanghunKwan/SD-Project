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
    IEnumerator rotateEnum;

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
            RotateInterpolation(40, 5, new Vector3(0, 6, -14));
            camMain.cullingMask += LayerMask.GetMask("UI");
            camMain.cullingMask += LayerMask.GetMask("Floor_UI");
        }
        else
        {
            RotateInterpolation(20, 2, new Vector3(2, 10, -11));
            camMain.cullingMask -= LayerMask.GetMask("UI");
            camMain.cullingMask -= LayerMask.GetMask("Floor_UI");
        }
    }
    void GetCamPosition(GameObject tempObj, float angle, float size, in Vector3 vec)
    {
        tempObj.transform.RotateAround(transform.position, Vector3.right, angle);
        camMain.transform.eulerAngles = tempObj.transform.eulerAngles;
        camMain.transform.position = tempObj.transform.position + vec;
        camMain.orthographicSize = size;
    }
    void RotateInterpolation(int angle, int size, in Vector3 vec)
    {
        if (rotateEnum != null)
            StopCoroutine(rotateEnum);

        rotateEnum = RotateIenum(angle, size, vec);
        StartCoroutine(rotateEnum);
    }
    IEnumerator RotateIenum(float angle, float size, Vector3 vec)
    {
        float perAngle = (angle - camMain.transform.eulerAngles.x) * Time.deltaTime;
        float perSize = (size - camMain.orthographicSize) * Time.deltaTime;
        float perX = (transform.position.x - camMain.transform.position.x + vec.x) * Time.deltaTime;
        float perY = (transform.position.y - camMain.transform.position.y + vec.y) * Time.deltaTime;
        float perZ = (transform.position.z - camMain.transform.position.z + vec.z) * Time.deltaTime;

        GameObject newGameobject = new GameObject();
        newGameobject.transform.position = camMain.transform.position;
        newGameobject.transform.eulerAngles = camMain.transform.eulerAngles;
        //x, z축 이동해서 위치 맞출 것.
        Vector3 perVector = new Vector3(perX, perY, perZ);
        do
        {
            GetCamPosition(newGameobject, perAngle, camMain.orthographicSize + perSize, perVector);
            perVector += new Vector3(perX, perY, perZ);
            yield return null;
        } while (camMain.transform.eulerAngles.x > 20 && camMain.transform.eulerAngles.x < 40);
        Destroy(newGameobject);
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
