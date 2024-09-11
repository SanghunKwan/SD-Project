using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponent : ClickCamTurningComponent
{
    BuildingSetWindow buildingWindow;
    
    [SerializeField] AddressableManager.BuildingImage upgradeType;
    [SerializeField] string infoText;


    villigeInteract[] saveVilligeInteract = new villigeInteract[3];


    protected override void Awake()
    {
        base.Awake();
        buildingWindow = camTurningWindow as BuildingSetWindow;
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
