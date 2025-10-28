using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingImg : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    [SerializeField] BuildingClickDrag clickdrag;
    [SerializeField] AddressableManager addMgr;
    [SerializeField] AddressableManager.BuildingImage type;
    ScrollRect scrollRect;
    [SerializeField] ConstructionUI onoffButton;
    BuildingScrollView scrollViewPort;
    List<Action<PointerEventData>> actions = new List<Action<PointerEventData>>();
    [SerializeField] Transform preivewParent;
    BuildingArrange previewObject;

    OpenWithMousePosition infoWindow;
    SetBuildingMat buildingMatView;

    int actionNum;

    private void Awake()
    {
        scrollRect = transform.parent.parent.parent.transform.GetComponent<ScrollRect>();
        image = GetComponent<Image>();
        addMgr.GetData(AddressableManager.LabelName.Building, type, out Sprite sprite);
        image.sprite = sprite;
        scrollViewPort = scrollRect.transform.GetChild(0).GetComponent<BuildingScrollView>();
        actions.Add(InScrollDrag);
        actions.Add(OutScrollDrag);
        actions.Add((eventData) => { });
        scrollViewPort.AddAction(() => actionNum = 1);

        previewObject = preivewParent.Find(type.ToString()).GetComponent<BuildingArrange>();
        infoWindow = scrollRect.transform.parent.Find("PricenfoBox").GetComponent<OpenWithMousePosition>();
        buildingMatView = infoWindow.GetComponent<SetBuildingMat>();


    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        actionNum = 0;
        scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        actions[actionNum](eventData);
    }

    void InScrollDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
    }
    void OutScrollDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        actionNum++;

        InputBuildingConstruct();
        SelectBuilding();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        clickdrag.OnPointerUp(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging || !PlayerInputManager.manager.windowInputEnable[(int)eventData.button]) return;

        InputBuildingConstruct();
    }
    void InputBuildingConstruct()
    {
        infoWindow.Close();

        if (buildingMatView.isBuildable)
        {
            clickdrag.Activate(previewObject, type);
            SelectBuilding();
        }
        else
            buildingMatView.HighLightNotEnoughMaterials((int)type + 1);
    }

    void SelectBuilding()
    {
        scrollRect.gameObject.SetActive(false);
        onoffButton.ButtonColorReset();
    }
    
    void CloseBox()
    {
        infoWindow.Close();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoWindow.OpenOnMouse(eventData);
        buildingMatView.GetData((int)type + 1, SetBuildingMat.MaterialsType.BuildingNeed);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CloseBox();
    }
    
    public void OnDisable()
    {
        CloseBox();
    }
}
