using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class StageFloorComponent : InitObject
{
    const float localPlaneLength = 5;
    const float localLinkOffset = 0.45f;

    float PlaneMagnification;

    StageFloorComponent[] nearComponent;
    Transform navMeshParent;

    [SerializeField] int stageIndex;
    int directionCount;


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
        PlaneMagnification = navMeshParent.localScale.x;

        nearComponent = new StageFloorComponent[directionCount];
    }
    public void NewLink(NavMeshLink link, StageFloorComponent newStageFloorComponent, Direction2Array direction)
    {
        int iDirection = (int)direction;

        link.transform.SetParent(navMeshParent);
        link.transform.SetLocalPositionAndRotation(GetLinkPosition(direction), Quaternion.Euler(0, (iDirection % 2) * 90, 0));
        link.gameObject.SetActive(true);
        link.width = GetShorterPlaneLength(newStageFloorComponent.stageIndex) * 4;
    }
    float GetShorterPlaneLength(int compareStageIndex)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        return Mathf.Min(battleClearManager.planeSize[compareStageIndex], battleClearManager.planeSize[stageIndex]);
    }
    public void NewStage(StageFloorComponent newStageFloorComponent, Direction2Array direction)
    {
        int iDirection = (int)direction;

        nearComponent[iDirection] = newStageFloorComponent;
        nearComponent[iDirection].nearComponent[(iDirection + 2) % directionCount] = this;
        nearComponent[iDirection].transform.localPosition = transform.localPosition + GetStagePosition(direction, newStageFloorComponent.stageIndex);
    }

    #region Vector3
    Vector3 GetLinkPosition(Direction2Array direction)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        int iDirection = (int)direction;

        //거리 재설정 필요
        //스테이지 사이 거리 : planeSize + planeSize + linkLength
        //링크 위치 : planeSize + 0.5f * linkLength


        float linkOffset = localPlaneLength + (localLinkOffset / PlaneMagnification);
        float xValue = battleClearManager.linkPosition[iDirection] * linkOffset;
        float zValue = battleClearManager.linkPosition[(iDirection + 1) % directionCount] * linkOffset;

        return new Vector3(xValue, 0, zValue);
    }
    public int GetStageIndex(int stageIndex)
    {
        return GameManager.manager.battleClearManager.PoolStageIndex(stageIndex);
    }
    Vector3 GetStagePosition(Direction2Array direction, int nextStageIndex)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        int iDirection = (int)direction;
        float offset = (1.41421356f * (battleClearManager.planeSize[stageIndex] + battleClearManager.planeSize[nextStageIndex] + localLinkOffset));
        Vector2 value = offset * battleClearManager.stagePosition[iDirection];

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
