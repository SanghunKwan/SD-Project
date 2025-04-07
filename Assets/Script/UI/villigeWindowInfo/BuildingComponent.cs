using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponent : ClickCamTurningComponent
{
    BuildingSetWindow buildingWindow;

    [SerializeField] AddressableManager.BuildingImage upgradeType;
    [SerializeField] string infoText;

    /// <summary>
    /// parameter : dayRemaining
    /// </summary>
    public Action<int> constructionAction { get; set; }

    public villigeInteract[] saveVilligeInteract { get; private set; } = new villigeInteract[3];


    protected override void VirtualAwake()
    {
        buildingWindow = camTurningWindow as BuildingSetWindow;
        ReadytoUse();
        constructionAction += (day) => ReadytoUse();
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
        if (index != 0 && !IsDataNull(index, out villigeInteract saveVillige))
        {
            //기존 영웅 할당 취소
            saveVillige.DeleteWorkPlace();
        }

        saveVilligeInteract[index] = vil_interact;
        //영웅 building 앞으로 이동 후 특정 애니메이션 재생.
        //vil_interact.hero.unitMove.MoveOrAttack(MoveType.Move, transform.position);
        QueueBuildingWork(vil_interact.hero.unitMove, index);
        GameManager.manager.onVilligeBuildingHeroAllocation.eventAction?.Invoke((int)type, transform.position);
    }
    async void QueueBuildingWork(UnitMove heroMove, int index)
    {
        heroMove.NowActorReserve(MoveType.Move, transform.position + Vector3.back * 5, OrderType.NowAct);

        await System.Threading.Tasks.Task.Delay(100);
        heroMove.QueueBuildingWork(type, index);
        heroMove.NowActorReserve(MoveType.Move, transform.position + Vector3.back * 2, OrderType.InQueue);
    }
    public void ResetData(int index)
    {
        if (index != 0)
            saveVilligeInteract[index].DeleteWorkPlace();
        saveVilligeInteract[index] = null;
    }
    public bool IsDataNull(int index, out villigeInteract saveVillige)
    {
        saveVillige = saveVilligeInteract[index];
        return saveVillige == null;
    }
    public int[] GetWorkHeroArray()
    {
        int length = saveVilligeInteract.Length;
        int[] tempHeroArray = new int[length];

        for (int i = 0; i < length; i++)
        {
            tempHeroArray[i] = saveVilligeInteract[i]?.GetCharacterListIndex() ?? -1;
        }

        return tempHeroArray;
    }

}
