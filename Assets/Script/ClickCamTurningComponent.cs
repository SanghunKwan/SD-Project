using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClickCamTurningComponent : MonoBehaviour
{
    protected CamTuringWindow camTurningWindow { get; private set; }
    protected bool isWindowOpen { get; private set; }
    protected Camera camMain;
    IEnumerator moveIenum;

    [SerializeField] string windowName;
    [SerializeField] float fX;
    [SerializeField] float fZ;
    [SerializeField] float camSize;

    [SerializeField] protected AddressableManager.BuildingImage type;

    protected virtual void Awake()
    {
        camMain = Camera.main;
        camTurningWindow = GameObject.FindWithTag("UI").transform.Find(windowName).GetComponent<CamTuringWindow>();
    }

    private void OnMouseUpAsButton()
    {
        ToggleWindow();
    }
    public void ToggleWindow()
    {
        camTurningWindow.GetTurningComponent(this);

        bool wasWindowOpen = camTurningWindow.gameObject.activeSelf;
        isWindowOpen = (!wasWindowOpen) || camTurningWindow.clickCamturningComponent != this;
        WindowOpenCheck(wasWindowOpen);

        SetWindowOpen();
    }

    void WindowOpenCheck(bool wasWindowOpen)
    {
        if (wasWindowOpen != isWindowOpen)
            SetCam();
        else
            StartInterpolation(RotateIenum(20, camSize, fX, fZ, 3));
        SetMemory();
    }
    void SetCam()
    {
        if (isWindowOpen)
        {
            StartInterpolation(RotateIenum(20, camSize, fX, fZ));
            camMain.cullingMask -= LayerMask.GetMask("UI");
            camMain.cullingMask -= LayerMask.GetMask("Floor_UI");
        }
        else
        {
            StartInterpolation(RotateIenum(40, 5, fX * 1.5f, fZ / 4));
            camMain.cullingMask += LayerMask.GetMask("UI");
            camMain.cullingMask += LayerMask.GetMask("Floor_UI");
        }
        camTurningWindow.Collider_UIActive(!isWindowOpen);
    }


    void StartInterpolation(IEnumerator ienum)
    {
        if (CamTuringWindow.transformObject != null)
        {
            CamTuringWindow.ienumOwner.StopmoveIenum();
            Destroy(CamTuringWindow.transformObject);
        }
        moveIenum = ienum;
        StartCoroutine(moveIenum);
        CamTuringWindow.ienumOwner = this;
    }
    public void StopmoveIenum()
    {
        StopCoroutine(moveIenum);
    }
    IEnumerator RotateIenum(float angle, float size, float fX, float fZ, float speed = 1)
    {
        GetPerMove(fX, fZ, out Vector3 perVector);

        float perAngle = (angle - camMain.transform.eulerAngles.x);
        float perSize = (size - camMain.orthographicSize);

        CamTuringWindow.transformObject.transform.position = camMain.transform.position;
        CamTuringWindow.transformObject.transform.eulerAngles = camMain.transform.eulerAngles;
        //x, z축 이동해서 위치 맞출 것.
        Vector3 addVector = perVector;
        float timeCheck = 0;
        perVector *= Time.deltaTime * speed;
        do
        {
            float delta = Time.deltaTime * speed;

            GetCamPosition(perAngle * delta, camMain.orthographicSize + (perSize * delta), perVector);
            perVector += addVector * delta;
            timeCheck += delta;
            yield return null;
        } while (timeCheck < 1);

        yield return null;
        float radAngle = Mathf.Deg2Rad * camMain.transform.eulerAngles.x;
        float offsetY = (10 - camMain.transform.position.y) / Mathf.Tan(radAngle);
        camMain.transform.position = new Vector3(camMain.transform.position.x, 10, camMain.transform.position.z - offsetY);

        Destroy(CamTuringWindow.transformObject);
    }
    void GetPerMove(float fX, float fZ, out Vector3 perVector)
    {
        float perX = (transform.position.x - camMain.transform.position.x + fX);

        float distance = Vector3.Distance(new Vector3(0, camMain.transform.position.y, camMain.transform.position.z),
                                         new Vector3(0, transform.position.y, transform.position.z));
        float radAngle = Mathf.Deg2Rad * camMain.transform.eulerAngles.x;
        float deltaY = Mathf.Sin(radAngle) * distance;
        float deltaZ = -Mathf.Cos(radAngle) * distance;

        float perY = (transform.position.y - (camMain.transform.position.y - deltaY));
        float perZ = (transform.position.z - (camMain.transform.position.z - deltaZ) + fZ);

        CamTuringWindow.transformObject = new GameObject();
        perVector = new Vector3(perX, perY, perZ);
    }
    void GetCamPosition(float angle, float size, in Vector3 vec)
    {
        CamTuringWindow.transformObject.transform.RotateAround(transform.position, Vector3.right, angle);
        camMain.transform.eulerAngles = CamTuringWindow.transformObject.transform.eulerAngles;
        camMain.transform.position = CamTuringWindow.transformObject.transform.position + vec;
        camMain.orthographicSize = size;
    }
    protected abstract void SetWindowOpen();
    protected abstract void SetMemory();
}
