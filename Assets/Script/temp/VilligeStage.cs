using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilligeStage : MonoBehaviour
{
    [SerializeField] BattleClearManager battleClearManager;
    StageFloorComponent stageFloorComponent;
    private void Awake()
    {
        stageFloorComponent = GetComponent<StageFloorComponent>();
        battleClearManager.onVilligeFloorComponentSet += () => stageFloorComponent;
    }
}
