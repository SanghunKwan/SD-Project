using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StagePrefabsCaller : MonoBehaviour
{
    [SerializeField] protected StagePoolManager stagePoolManager;

    public abstract GameObject PlacePrefab(StageFloorComponent floor, int floorNum);

}
