using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFloorComponent : InitObject
{
    StageFloorComponent[] nearComponent;
    Transform navMeshParent;

    [SerializeField] int stageIndex;
    int directionCount = (int)Direction2Array.Max;
    public enum Direction2Array
    {
        RightUP,
        RightDown,
        LeftDown,
        LeftUP,
        Max
    }
    public override void Init()
    {
        directionCount = (int)Direction2Array.Max;

        navMeshParent = transform.GetChild(0);
        nearComponent = new StageFloorComponent[directionCount];
    }
    public void NewLink(GameObject link, Direction2Array direction)
    {
        int iDirection = (int)direction;

        link.transform.SetParent(navMeshParent);
        link.transform.SetLocalPositionAndRotation(GetLinkPosition(direction), Quaternion.Euler(0, (iDirection % 2) * 90, 0));
        link.SetActive(true);
    }
    public void NewStage(StageFloorComponent newStageFloorComponent, Direction2Array direction)
    {
        int iDirection = (int)direction;

        nearComponent[iDirection] = newStageFloorComponent;
        nearComponent[iDirection].Init();
        nearComponent[iDirection].nearComponent[(iDirection + 2) % directionCount] = this;
        Debug.Log((iDirection + 2) % directionCount);
        nearComponent[iDirection].transform.localPosition = transform.localPosition + GetStagePosition(direction);
    }

    #region Vector3
    Vector3 GetLinkPosition(Direction2Array direction)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        int iDirection = (int)direction;
        float xValue = battleClearManager.linkPosition[iDirection] * battleClearManager.planeSize[stageIndex];
        float zValue = battleClearManager.linkPosition[(iDirection + 1) % directionCount] * battleClearManager.planeSize[stageIndex];

        return new Vector3(xValue, 0, zValue);
    }
    Vector3 GetStagePosition(Direction2Array direction)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        int iDirection = (int)direction;
        Vector2 value = battleClearManager.stagePosition[iDirection] * battleClearManager.planeSize[stageIndex] * 1.414f;

        return new Vector3(value.x, 0, value.y);
    }
    #endregion
    #region GetEmptyDirection
    public Direction2Array GetEmptyDirection()
    {
        GetEmptyIndex(out int[] emptyIndex);
        return (Direction2Array)emptyIndex[UnityEngine.Random.Range(0, emptyIndex.Length)];
    }
    void GetEmptyIndex(out int[] emptyIndex)
    {
        int bitSum = 0;
        BitArray bit = new BitArray(4);
        for (int i = 0; i < directionCount; i++)
        {
            bit[i] = nearComponent[i];
            bitSum += Convert.ToInt32(bit[i]);
        }
        emptyIndex = new int[bitSum];

    }


    #endregion
}
