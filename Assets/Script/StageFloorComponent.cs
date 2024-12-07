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
        Vector2 value = 1.414f * battleClearManager.planeSize[stageIndex] * battleClearManager.stagePosition[iDirection];

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
        int bit = 0;
        int count = 0;
        int index = 0;
        for (int i = 0; i < directionCount; i++)
        {
            if (nearComponent[i] != null)
                continue;

            bit += (1 << i);
            count++;
        }
        emptyIndex = new int[count];
        for (int i = 0; i < directionCount; i++)
        {
            if ((bit & 1) == 0)
            {
                bit >>= 1;
                continue;
            }

            emptyIndex[index] = i;
            bit >>= 1;
            index++;
        }
    }


    #endregion
}
