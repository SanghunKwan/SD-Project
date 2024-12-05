using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFloorComponent : MonoBehaviour
{
    StageFloorComponent[] nearComponent;
    Transform navMeshParent;

    [SerializeField] int stageIndex;

    public enum Direction2Array
    {
        RightUP,
        RightDown,
        LeftDown,
        LeftUP,
        Max
    }

    private void Awake()
    {
        int directionCount = (int)Direction2Array.Max;

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
        nearComponent[iDirection].nearComponent[(iDirection + 2) % (int)Direction2Array.Max] = this;
        nearComponent[iDirection].transform.localPosition = GetStagePosition(direction);
    }

    #region Vector3
    Vector3 GetLinkPosition(Direction2Array direction)
    {
        BattleClearManager battleClearManager = GameManager.manager.battleClearManager;

        int iDirection = (int)direction;
        float xValue = battleClearManager.linkPosition[iDirection] * battleClearManager.planeSize[stageIndex];
        float zValue = battleClearManager.linkPosition[(iDirection + 1) % (int)Direction2Array.Max] * battleClearManager.planeSize[stageIndex];

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
        return (Direction2Array)UnityEngine.Random.Range(0, emptyIndex.Length);
    }
    void GetEmptyIndex(out int[] emptyIndex)
    {
        int length = (int)Direction2Array.Max;
        int index = 0;
        int[] tempArray = new int[length];

        for (int i = 0; i < length; i++)
        {
            if (nearComponent[i] != null)
                continue;

            tempArray[index++] = i;
        }
        emptyIndex = new int[index];
        Array.Copy(tempArray, emptyIndex, index);
    }
    #endregion
}
