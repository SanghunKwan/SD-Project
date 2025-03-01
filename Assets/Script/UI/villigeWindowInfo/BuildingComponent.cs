using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponent : ClickCamTurningComponent
{
    BuildingSetWindow buildingWindow;

    [SerializeField] AddressableManager.BuildingImage upgradeType;
    [SerializeField] string infoText;


    public villigeInteract[] saveVilligeInteract { get; private set; } = new villigeInteract[3];


    protected override void VirtualAwake()
    {
        buildingWindow = camTurningWindow as BuildingSetWindow;
        //�Ǽ� �Ϸ� ���� �ʿ�.
        //�Ǽ� �� ���� �ð� / ���� day �ʿ�
        //�̹� �Ǽ��� building �ε� �� ��� �Ϸ�.
        GameManager.manager.onVilligeBuildingCompleteConstruction.eventAction?.Invoke((int)type, transform.position);
    }

    protected override void SetWindowOpen()
    {
        buildingWindow.SetOpen(this, isWindowOpen, type, upgradeType, infoText, saveVilligeInteract);
    }

    protected override void SetMemory()
    {
        if (type != AddressableManager.BuildingImage.Tomb)
            saveVilligeInteract[0] = null;
    }

    public void SaveData(villigeInteract vil_interact, int index)
    {
        if (!IsDataNull(index, out villigeInteract saveVillige))
        {
            //���� ���� �Ҵ� ���
            saveVillige.DeleteWorkPlace();
        }

        saveVilligeInteract[index] = vil_interact;
        GameManager.manager.onVilligeBuildingHeroAllocation.eventAction?.Invoke((int)type, transform.position);
    }
    public void ResetData(int index)
    {
        saveVilligeInteract[index].DeleteWorkPlace();
        saveVilligeInteract[index] = null;
    }
    public bool IsDataNull(int index, out villigeInteract saveVillige)
    {
        saveVillige = saveVilligeInteract[index];
        return saveVillige == null;
    }
}
