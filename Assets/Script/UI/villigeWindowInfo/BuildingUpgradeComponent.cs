using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingUpgradeComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] OpenWithMousePosition infoWindow;
    [SerializeField] Image upgradeImage;
    [SerializeField] BuildingPool pool;
    SetBuildingMat buildingMatView;
    AddressableManager.BuildingImage _type;
    MaterialsData.NeedMaterials needMaterials;
    BuildingComponent _building;


    public void Init(AddressableManager manager, AddressableManager.BuildingImage type, BuildingComponent building)
    {
        _building = building;
        buildingMatView = infoWindow.GetComponent<SetBuildingMat>();
        _type = type;

        manager.GetData(AddressableManager.LabelName.Building, type, out Sprite sprite);
        upgradeImage.sprite = sprite;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        infoWindow.OpenOnMouse(eventData);
        needMaterials = buildingMatView.GetData((int)_type + 1, SetBuildingMat.MaterialsType.BuildingNeed);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseBox();
    }
    void CloseBox()
    {
        infoWindow.Close();
    }
    private void OnDisable()
    {
        CloseBox();
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
            _building.ToggleWindow();
            GameManager.manager.questManager.onBuildingControlFinish += () =>
            {
                pool.BackPooling(_building);

                BuildingComponent building = pool.PoolBuilding(_type, _building.transform.position).buildingComponent;
                building.constructionAction?.Invoke(needMaterials.turn);
                building.CObject.GetSelecting();

            };
        }
        else
            buildingMatView.HighLightNotEnoughMaterials((int)_type + 1);
    }
}
