using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingClickDrag : InitInterface, IPointerMoveHandler, IPointerUpHandler, IPointerDownHandler
{
    BuildingArrange obj;
    Camera mainCam;

    Action[] actions = new Action[3];
    [SerializeField] BuildingPool pool;
    Vector3 lastPoint;

    AddressableManager.BuildingImage buildType;
    
    public void Activate(BuildingArrange preview, in AddressableManager.BuildingImage type)
    {
        gameObject.SetActive(true);
        obj = preview;
        obj.gameObject.SetActive(true);
        buildType = type;

        if (CheckRaycast(UnityEngine.InputSystem.Mouse.current.position.value, out RaycastHit hit))
        {
            lastPoint = hit.point;
            obj.transform.position = lastPoint;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {

        if (CheckRaycast(eventData.position, out RaycastHit hit))
        {
            lastPoint = hit.point;
            obj.transform.position = lastPoint;
        }
    }
    bool CheckRaycast(in Vector2 eventData, out RaycastHit hit)
    {
        Ray ray = mainCam.ScreenPointToRay(eventData);

        return Physics.Raycast(ray, out hit, 100, 0x100);

    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        actions[(int)eventData.button]();
    }
    void LeftButton()
    {
        if (!gameObject.activeSelf || obj.isChanged)
            return;

        //생성
        pool.PoolObject(buildType).transform.position = lastPoint;
        obj.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    void RightButton()
    {
        //취소
        if(obj == null) return;
        obj.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void Init()
    {
        mainCam = Camera.main;

        actions[0] = LeftButton;
        actions[1] = RightButton;

        actions[2] = () => { };
        gameObject.SetActive(false);
    }
}
