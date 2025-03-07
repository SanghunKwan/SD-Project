using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuildingComponent), typeof(Animator))]
public class BuildingConstructDelay : MonoBehaviour
{
    public BuildingComponent buildingComponent { get; private set; }
    public VilligeBuildingConstructing constructingUI { get; private set; }
    public Animator anim { get; private set; }
    public int dayRemaining { get; private set; }

    static int constructStateName = Animator.StringToHash("construct");

    private void Awake()
    {
        buildingComponent = GetComponent<BuildingComponent>();
        anim = GetComponent<Animator>();

        buildingComponent.constructionAction += StartConstruction;
        GameManager.manager.battleClearManager.NewBuilding(this);
    }
    void StartConstruction(int newDayRemaining)
    {
        SetConstructData(newDayRemaining, 0);
    }
    void SetConstructData(int newDayRemaining, float constructionProgress)
    {
        anim.Play(constructStateName, 0, constructionProgress);
        constructingUI = ObjectUIPool.pool.Call(ObjectUIPool.Folder.VilligeConstructingUI,
                                                ObjectUIPool.UICanvasType.UpperCanvas).
                                                GetComponent<VilligeBuildingConstructing>();
        dayRemaining = newDayRemaining;
        constructingUI.SetTargetBuilding(this, dayRemaining, constructionProgress);
    }
    public void LoadConstructionData(BuildingData data)
    {
        SetConstructData(data.dayRemaining, data.timeNormalized);
    }
    public void OnAnimConstructionComplete()
    {
        GameManager.manager.onVilligeBuildingCompleteConstruction.eventAction?.
            Invoke((int)buildingComponent.Type, transform.position);

        buildingComponent.ReadytoUse();
    }
    private void OnDestroy()
    {
        if (buildingComponent != null)
            buildingComponent.constructionAction -= StartConstruction;

        if (constructingUI != null)
            constructingUI.enabled = false;
    }


}
