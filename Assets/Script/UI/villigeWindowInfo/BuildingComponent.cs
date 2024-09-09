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
    IEnumerator moveIenum;

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
        if (wasWindowOpen != isWindowOpen)
            SetCam();
        else
            StartInterpolation(RotateIenum(20, 2, 2, 3));
        SetMemory();
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
        if (buildingWindow.transformObject != null)
        {
            buildingWindow.ienumOwner.StopmoveIenum();
            Destroy(buildingWindow.transformObject);
        }
        moveIenum = ienum;
        StartCoroutine(moveIenum);
        buildingWindow.ienumOwner = this;
    }
    public void StopmoveIenum()
    {
        StopCoroutine(moveIenum);
    }
    IEnumerator RotateIenum(float angle, float size, float fX, float speed = 1)
    {
        GetPerMove(fX, out Vector3 perVector);

        float perAngle = (angle - camMain.transform.eulerAngles.x);
        float perSize = (size - camMain.orthographicSize);

        buildingWindow.transformObject.transform.position = camMain.transform.position;
        buildingWindow.transformObject.transform.eulerAngles = camMain.transform.eulerAngles;
        //x, z축 이동해서 위치 맞출 것.
        Vector3 addVector = perVector;
        float timeCheck = 0;
        do
        {
            float delta = Time.deltaTime * speed;
            GetCamPosition(perAngle * delta, camMain.orthographicSize + (perSize * delta), perVector);
            perVector += addVector * speed;
            timeCheck += delta;
            yield return null;
        } while (timeCheck < 1);

        float radAngle = Mathf.Deg2Rad * camMain.transform.eulerAngles.x;
        float offsetY = (10 - camMain.transform.position.y) / Mathf.Tan(radAngle);
        camMain.transform.position = new Vector3(camMain.transform.position.x, 10, camMain.transform.position.z - offsetY);

        Destroy(buildingWindow.transformObject);
    }
    void GetPerMove(float fX, out Vector3 perVector)
    {
        float perX = (transform.position.x - camMain.transform.position.x + fX) * Time.deltaTime;

        float distance = Vector3.Distance(new Vector3(0, camMain.transform.position.y, camMain.transform.position.z),
                                         new Vector3(0, transform.position.y, transform.position.z));
        float radAngle = Mathf.Deg2Rad * camMain.transform.eulerAngles.x;
        float deltaY = Mathf.Sin(radAngle) * distance;
        float deltaZ = -Mathf.Cos(radAngle) * distance;

        float perY = (transform.position.y - (camMain.transform.position.y - deltaY)) * Time.deltaTime;
        float perZ = (transform.position.z - (camMain.transform.position.z - deltaZ)) * Time.deltaTime;

        buildingWindow.transformObject = new GameObject();
        perVector = new Vector3(perX, perY, perZ);
    }
    void GetCamPosition(float angle, float size, in Vector3 vec)
    {
        buildingWindow.transformObject.transform.RotateAround(transform.position, Vector3.right, angle);
        camMain.transform.eulerAngles = buildingWindow.transformObject.transform.eulerAngles;
        camMain.transform.position = buildingWindow.transformObject.transform.position + vec;
        camMain.orthographicSize = size;
    }
    void SetMemory()
    {
        if (type != AddressableManager.BuildingImage.Tomb)
            saveVilligeInteract[0] = null;
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
        Destroy(buildingWindow.transformObject);
    }

    public void SaveData(villigeInteract vil_interact, int index)
    {
        saveVilligeInteract[index] = vil_interact;
    }
    public void ResetData(int index)
    {
        saveVilligeInteract[index].SaveWorkPlace(null, 0);
        saveVilligeInteract[index] = null;
    }
    public bool IsDataNull(int index, out villigeInteract saveVillige)
    {
        saveVillige = saveVilligeInteract[index];
        return saveVillige == null;
    }
}
