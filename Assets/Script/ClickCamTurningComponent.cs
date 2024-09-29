using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClickCamTurningComponent : MonoBehaviour
{
    protected CamTuringWindow camTurningWindow { get; private set; }
    protected bool isWindowOpen { get; private set; }
    protected Camera camMain;
    CapsuleCollider capsuleCollider;
    IEnumerator moveIenum;

    [SerializeField] string windowName;
    [SerializeField] float fX;
    [SerializeField] float fZ;
    [SerializeField] float camSize;

    [SerializeField] protected AddressableManager.BuildingImage type;
    [SerializeField] float camAngle;
    [SerializeField] int[] cullingLayers;

    Action delaySetCam = () => { };
    public Action tickCamMove = () => { };

    protected virtual void Awake()
    {
        camMain = Camera.main;
        camTurningWindow = GameObject.FindWithTag("UI").transform.Find(windowName).GetComponent<CamTuringWindow>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnMouseUpAsButton()
    {
        if (GameManager.manager.pointerEventData.pointerCurrentRaycast.gameObject.name == "InputUI")
        {
            ToggleWindow();
        }


    }

    public void ToggleWindow()
    {
        ToggleActive(camTurningWindow.gameObject.activeSelf);

        camTurningWindow.GetTurningComponent(this);
    }
    void ToggleActive(bool wasWindowOpen)
    {
        isWindowOpen = (!wasWindowOpen) || camTurningWindow.clickCamturningComponent != this;
        WindowOpenCheck(wasWindowOpen);
        SetColliderActive(!isWindowOpen);

        SetWindowOpen();
    }

    void WindowOpenCheck(bool wasWindowOpen)
    {
        if (wasWindowOpen != isWindowOpen)
            SetCam();
        else
            StartInterpolation(RotateIenum(camAngle, camSize, fX, fZ, 3));

        SetMemory();
    }
    void SetCam()
    {
        camTurningWindow.CloseOtherWindow();
        camTurningWindow.Collider_UIActive(!isWindowOpen);

        if (isWindowOpen)
        {
            StartInterpolation(RotateIenum(camAngle, camSize, fX, fZ));
            camMain.cullingMask -= 32;
            camMain.cullingMask -= 4096;
            foreach (int layer in cullingLayers)
                camMain.cullingMask -= (int)Mathf.Pow(2, layer);
        }
        else
        {
            StartInterpolation(RotateIenum(40, 5, fX * 1.5f, 1));
            delaySetCam = () =>
                {
                    camMain.cullingMask += 32;
                    camMain.cullingMask += 4096;
                    foreach (int layer in cullingLayers)
                        camMain.cullingMask += (int)Mathf.Pow(2, layer);
                };
        }
    }


    protected void StartInterpolation(IEnumerator ienum)
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
        Vector3 addVector = Vector3.zero;
        float timeCheck = 0;
        float fPow;
        do
        {
            float delta = Time.deltaTime * speed;

            GetCamPosition(perAngle * delta, camMain.orthographicSize + (perSize * delta), addVector);
            SetCamDistance(GetCamDistanceFromWindow());

            timeCheck += delta;
            fPow = Mathf.Pow(timeCheck, 4);
            addVector = new Vector3(timeCheck * perVector.x, perVector.y * fPow, perVector.z * fPow);
            tickCamMove();
            yield return null;

        } while (timeCheck < 1);

        yield return null;

        Destroy(CamTuringWindow.transformObject);
        SetCamDistanceY(GetCamDistanceFromWindow(), Mathf.Tan(camMain.transform.eulerAngles.x * Mathf.Deg2Rad));

        delaySetCam();
        tickCamMove();
        delaySetCam = () => { };

    }
    int GetCamDistanceFromWindow()
    {
        if (isWindowOpen)
            return 30;

        return 10;
    }
    void SetCamDistance(float fDistance)
    {
        float radAngle = Mathf.Deg2Rad * camMain.transform.eulerAngles.x;
        float tanAngle = Mathf.Tan(radAngle);
        float newDistance = camMain.orthographicSize * 2.1f;

        if (newDistance < camMain.transform.localPosition.y)
        {
            SetCamDistanceY(newDistance, tanAngle);
        }
        else
        {
            SetCamDistanceZ(-newDistance, tanAngle);
        }
    }
    void SetCamDistanceY(float fDistance, float fTan)
    {
        float offsetY = (fDistance - camMain.transform.position.y) / fTan;
        camMain.transform.position = new Vector3(camMain.transform.position.x, fDistance, camMain.transform.position.z - offsetY);
    }
    void SetCamDistanceZ(float fDistance, float fTan)
    {
        float offsetY = (fDistance - camMain.transform.position.z) * fTan;
        camMain.transform.position = new Vector3(camMain.transform.position.x, camMain.transform.position.y - offsetY, fDistance);
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

    public void ChangeWindow()
    {
        if (!isWindowOpen)
            return;

        isWindowOpen = false;
        SetColliderActive(true);

        camTurningWindow.Collider_UIActive(!isWindowOpen);

        camMain.cullingMask += 32;
        camMain.cullingMask += 4096;
        foreach (int layer in cullingLayers)
            camMain.cullingMask += (int)Mathf.Pow(2, layer);

        SetWindowOpen();
    }
    public void SetColliderActive(bool onoff)
    {
        capsuleCollider.enabled = onoff;
    }
}
