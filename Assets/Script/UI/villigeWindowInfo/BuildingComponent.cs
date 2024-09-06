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
    GameObject transformObject;


    villigeInteract[] saveVilligeInteract = new villigeInteract[3];
    IEnumerator moveEnum;

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
        bool wasWindowOpen = buildingWindow.gameObject.activeSelf;
        isWindowOpen = (!wasWindowOpen) || buildingWindow.buildingComponent != this;
        WindowOpenCheck(wasWindowOpen);

        SetWindowOpen();
    }

    void SetWindowOpen()
    {
        buildingWindow.SetOpen(this, isWindowOpen, type, upgradeType, infoText, saveVilligeInteract);
    }
    void WindowOpenCheck(bool wasWindowOpen)
    {
        if (wasWindowOpen == isWindowOpen)
        {
            StartInterpolation(TransIenum());
        }
        else
            SetCam();
    }
    void SetCam()
    {
        if (isWindowOpen)
        {
            StartInterpolation(RotateIenum(20, 2, 2));
            camMain.cullingMask -= LayerMask.GetMask("UI");
            camMain.cullingMask -= LayerMask.GetMask("Floor_UI");
        }
        else
        {
            StartInterpolation(RotateIenum(40, 5, 3));
            camMain.cullingMask += LayerMask.GetMask("UI");
            camMain.cullingMask += LayerMask.GetMask("Floor_UI");
        }
        buildingWindow.Collider_UIActive(!isWindowOpen);
    }

    void StartInterpolation(IEnumerator ienum)
    {
        if (transformObject != null)
        {
            StopCoroutine(moveEnum);
            Destroy(transformObject);
        }

        moveEnum = ienum;
        StartCoroutine(moveEnum);
    }
    IEnumerator RotateIenum(float angle, float size, float fX)
    {
        GetPerMove(fX, out Vector3 perVector);

        float perAngle = (angle - camMain.transform.eulerAngles.x) * Time.deltaTime;
        float perSize = (size - camMain.orthographicSize) * Time.deltaTime;

        transformObject.transform.position = camMain.transform.position;
        transformObject.transform.eulerAngles = camMain.transform.eulerAngles;
        //x, z축 이동해서 위치 맞출 것.
        Vector3 addVector = perVector;
        do
        {
            GetCamPosition(transformObject, perAngle, camMain.orthographicSize + perSize, perVector);
            perVector += addVector;
            yield return null;
        } while (camMain.transform.eulerAngles.x > 20 && camMain.transform.eulerAngles.x < 40);
        Destroy(transformObject);
    }
    void GetPerMove(float fX, out Vector3 perVector)
    {
        float perX = (transform.position.x - camMain.transform.position.x + fX) * Time.deltaTime;

        float distance = Vector3.Distance(camMain.transform.position, transform.position);
        float radAngle = Mathf.Deg2Rad * camMain.transform.eulerAngles.x;
        float deltaY = Mathf.Sin(radAngle) * distance;
        float deltaZ = -Mathf.Cos(radAngle) * distance;

        float perY = (transform.position.y - (camMain.transform.position.y - deltaY)) * Time.deltaTime;
        float perZ = (transform.position.z - (camMain.transform.position.z - deltaZ)) * Time.deltaTime;

        transformObject = new GameObject();
        perVector = new Vector3(perX, perY, perZ);
    }
    void GetCamPosition(GameObject tempObj, float angle, float size, in Vector3 vec)
    {
        tempObj.transform.RotateAround(transform.position, Vector3.right, angle);
        camMain.transform.eulerAngles = tempObj.transform.eulerAngles;
        camMain.transform.position = tempObj.transform.position + vec;
        camMain.orthographicSize = size;
    }
    IEnumerator TransIenum()
    {
        GetPerMove(2, out Vector3 perVector);
        float timeCheck = 0;
        do
        {
            camMain.transform.position += perVector * 3;
            timeCheck += Time.deltaTime * 3;
            yield return null;
        } while (timeCheck < 1);
        Destroy(transformObject);
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
