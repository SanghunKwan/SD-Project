using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public abstract class ClickCamTurningComponent : MonoBehaviour
{
    protected CamTuringWindow camTurningWindow { get; private set; }
    protected bool isWindowOpen { get; set; }
    protected Camera camMain;
    static Vector3 camTuringStartPosition;
    CapsuleCollider capsuleCollider;
    IEnumerator moveIenum;

    [SerializeField] string windowName;
    [SerializeField] float fX;
    [SerializeField] float fZ;
    [SerializeField] float camSize;

    [SerializeField] protected AddressableManager.BuildingImage type;
    public AddressableManager.BuildingImage Type { get { return type; } }
    [SerializeField] float camAngle;
    [Tooltip("Tower 등 Layer 추가 가리기")]
    [SerializeField] protected int[] cullingLayers;
    //Tower의 경우 바닥도 투명하게 함.


    protected bool isUsable;

    Action delaySetCam = () => { };
    public Action tickCamMove { get; set; } = () => { };
    public CObject CObject { get; private set; }

    private void Awake()
    {
        camMain = Camera.main;
        camTurningWindow = GameObject.FindWithTag("UI").transform.Find(windowName).GetComponent<CamTuringWindow>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        CObject = GetComponent<CObject>();
        VirtualAwake();
    }
    protected abstract void VirtualAwake();

    private void OnMouseUpAsButton()
    {
        if (isUsable && GameManager.manager.pointerEventData != null &&
            GameManager.manager.pointerEventData.pointerCurrentRaycast.gameObject.name == "InputUI")
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
        {
            SetCam();
            GameManager.manager.onVilligeBuildingWindowOpen.eventAction?.Invoke((int)type, transform.position);
        }
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
            camTuringStartPosition = camMain.transform.position;
            StartInterpolation(RotateIenum(camAngle, camSize, fX, fZ));
            camMain.cullingMask -= 32;
            camMain.cullingMask -= 4096;
            foreach (int layer in cullingLayers)
                camMain.cullingMask -= (int)Mathf.Pow(2, layer);

            GameManager.manager.questManager.isBuildingUnderControl = true;
        }
        else
        {
            StartInterpolation(RotateIenum(40, 5, fX * 1.5f, 0));
            delaySetCam = () =>
                {
                    camMain.cullingMask += 32;
                    camMain.cullingMask += 4096;
                    foreach (int layer in cullingLayers)
                        camMain.cullingMask += (int)Mathf.Pow(2, layer);

                    GameManager.manager.PointMoveConversionToUI(camMain.transform.position - camTuringStartPosition);
                    GameManager.manager.questManager.isBuildingUnderControl = false;
                    GameManager.manager.questManager.onBuildingControlFinish?.Invoke();
                    GameManager.manager.questManager.onBuildingControlFinish = null;
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
        GetPerMove(fX, fZ, angle, out Vector3 perVector, out float tOffset, out float rotateDistance, out float lastDistance);

        float perAngle = angle - camMain.transform.eulerAngles.x;
        float perSize = size - camMain.orthographicSize;
        float perDistance = lastDistance - rotateDistance;

        CamTuringWindow.transformObject = new GameObject();
        CamTuringWindow.transformObject.transform.position = camMain.transform.position;
        CamTuringWindow.transformObject.transform.rotation = camMain.transform.rotation;

        Vector3 addVector = Vector3.zero;
        Vector3 centerPosition = transform.position + Vector3.up * tOffset;
        float originalRotation = camMain.transform.eulerAngles.x;
        float timeCheck = 0;
        float delta;
        float addDistance = rotateDistance;
        do
        {
            delta = Time.deltaTime * speed;
            timeCheck += delta;
            addDistance += perDistance * delta;

            addVector = perVector * timeCheck;

            GetCamPosition(perAngle * delta, perSize * delta, addDistance, addVector, centerPosition);
            tickCamMove();
            yield return null;

        } while (timeCheck < 1);

        yield return null;
        Destroy(CamTuringWindow.transformObject);

        delaySetCam();
        tickCamMove();
        delaySetCam = () => { };

    }
    int GetCamDistanceFromWindow(bool isWinOpen)
    {
        if (isWinOpen)
            return 30;

        return 10;
    }

    void GetPerMove(float fX, float fZ, float angle, out Vector3 perVector, out float tOffset, out float rotateDistance, out float lastDistance)
    {
        //회전 후 y값과 목표로 하는 y값을 빼서 perY 산출.
        //perY로 perZ 산출.
        float perX = transform.position.x - camMain.transform.position.x + fX;

        float nowAngle = camMain.transform.eulerAngles.x * Mathf.Deg2Rad;
        float lengthZ = transform.position.z - camMain.transform.position.z;
        float lengthY = lengthZ * Mathf.Tan(nowAngle);
        rotateDistance = Vector2.Distance(Vector2.zero, new Vector2(lengthY, lengthZ));
        tOffset = camMain.transform.position.y - transform.position.y - lengthY;
        float endAngle = Mathf.Deg2Rad * angle;
        float eSin = Mathf.Sin(endAngle);
        float eCos = Mathf.Cos(endAngle);

        lastDistance = Vector2.Distance(Vector2.zero,
                                        new Vector2(GetCamDistanceFromWindow(isWindowOpen),
                                                    -(GetCamDistanceFromWindow(isWindowOpen) * (eCos / eSin))));
        float perY = Mathf.Max(tOffset + (lastDistance * eSin) - GetCamDistanceFromWindow(isWindowOpen), 0);

        float perZ = fZ - ((tOffset - perY) * (eCos / eSin));
        perVector = new Vector3(perX, -perY, perZ);
    }
    void GetCamPosition(float angle, float size, float distance, in Vector3 addVector, in Vector3 centerPosition)
    {
        Transform TransformObjectTransform = CamTuringWindow.transformObject.transform;

        TransformObjectTransform.RotateAround(centerPosition, Vector3.right, angle);

        SetCamDistance(distance, addVector, centerPosition);
        float sin = Mathf.Sin(Mathf.Deg2Rad * TransformObjectTransform.rotation.eulerAngles.x);
        camMain.orthographicSize += size;
    }
    void SetCamDistance(float distance, in Vector3 addVector, in Vector3 centerPosition)
    {
        Vector3 newVector = CamTuringWindow.transformObject.transform.position - centerPosition;
        Vector2 newVector2 = new Vector2(newVector.y, newVector.z).normalized * distance;


        camMain.transform.position = new Vector3(CamTuringWindow.transformObject.transform.position.x,
            centerPosition.y + newVector2.x, centerPosition.z + newVector2.y) + addVector;

        camMain.transform.rotation = CamTuringWindow.transformObject.transform.rotation;
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
    public void ReadytoUse()
    {
        isUsable = true;
    }
}
