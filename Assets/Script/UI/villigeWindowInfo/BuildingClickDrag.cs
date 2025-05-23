using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingClickDrag : InitObject, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{
    BuildingArrange obj;
    Camera mainCam;

    Action[] actions = new Action[3];
    [SerializeField] BuildingPool pool;
    [SerializeField] MaterialsData materialsData;
    Vector3 lastPoint;

    AddressableManager.BuildingImage buildType;
    [SerializeField] StorageComponent storageComponent;

    public void Activate(BuildingArrange preview, in AddressableManager.BuildingImage type)
    {
        GameManager.manager.onVilligeBuildingChoosed.eventAction?.Invoke((int)type, Vector3.zero);
        gameObject.SetActive(true);
        obj = preview;
        obj.gameObject.SetActive(true);
        buildType = type;
        if (CheckRaycast(UnityEngine.InputSystem.Mouse.current.position.value, out RaycastHit hit))
        {
            RoundPoint(hit);
        }
    }
    void RoundPoint(in RaycastHit hit)
    {
        Vector3 vec = new Vector3();

        vec.x = Mathf.RoundToInt(hit.point.x);
        vec.y = Mathf.RoundToInt(hit.point.y);
        vec.z = Mathf.RoundToInt(hit.point.z);

        lastPoint = vec;
        obj.transform.position = lastPoint;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (CheckRaycast(eventData.position, out RaycastHit hit))
        {
            RoundPoint(hit);
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

        //����
        GameManager.manager.onVilligeBuildingStartConstruction.eventAction?.Invoke((int)buildType, lastPoint);
        MaterialsData.NeedMaterials needMaterial = materialsData.data.Needs[(int)buildType + 1];
        pool.PoolBuilding(buildType, lastPoint).buildingComponent
            .constructionAction?.Invoke(needMaterial.turn);

        storageComponent.CalculateMaterials(needMaterial);
        obj.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    public void RightButton()
    {
        //���
        if (obj == null) return;
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
