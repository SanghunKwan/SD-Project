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
        RightUP,                    //00
        RightDown,                  //01
        LeftDown,                   //10
        LeftUP,                     //11

        Max
    }
    enum DirectionFlags
    {
        None = 0,

        RightUP = 1 << 0,           //0001
        RightDown = 1 << 1,         //0010
        LeftDown = 1 << 2,          //0100
        LeftUP = 1 << 3,            //1000

        Max = 4,

        All = (1 << Max) - 1
    }



    public override void Init()
    {
        directionCount = (int)DirectionFlags.Max;

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

        //�Ÿ� �缳�� �ʿ�
        //�������� ���� �Ÿ� : planeSize + planeSize + linkLength
        //��ũ ��ġ : planeSize + 0.5f * linkLength


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
        DirectionFlags unlinkedFlags = DirectionFlags.All ^ CurrentLinkedDirection();

        //unlinkedFlags���� �� ���� �� ����.
        unlinkedFlags = CheckUndirectlyLinkedStages(unlinkedFlags);

        return GetRandomDirectionByReservoir(unlinkedFlags);
    }
    Direction2Array GetRandomDirectionByReservoir(DirectionFlags flags)
    {
        //reservoir sampling �˰����� ����Ͽ� flags���� �������� Ȱ��ȭ�� ������ ��ȯ.

        //�⺻������ ���� ���� ��Ʈ �� Ȱ��ȭ.
        int intFlags = (int)flags;
        int selectedDirection = intFlags & (-intFlags); // flags�� ������ ��Ʈ���� ����.

        // ���� ������ ��ȸ��.
        int reservoirSeen = 1; //�⺻�� ī��Ʈ
        int length = (int)DirectionFlags.All; // length = (1 << DirectionFlags.Max)
        for (int i = selectedDirection; i < length; i <<= 1)
        {
            if ((i & intFlags) != 0)
            {
                reservoirSeen++;
                if (Random.Range(0, reservoirSeen) != 0) continue;

                selectedDirection = i; // ���� ������ ����.
            }
        }

        return (Direction2Array)GetSmallestBitIndex(selectedDirection); // ���õ� ������ �ε����� ��ȯ.
    }
    int GetSmallestBitIndex(int bit)
    {
        int index = 0;
        while ((bit & 1) == 0)
        {
            bit >>= 1;
            index++;
        }

        return index;
    }
    DirectionFlags CheckUndirectlyLinkedStages(DirectionFlags unlinkedFlags)
    {
        //��Ʈ�� 1�� ���� ��ȸ�ؼ� �� �� �ִ��� Ȯ��.
        int intFlag = (int)unlinkedFlags;
        int length = (int)DirectionFlags.Max;
        for (int i = 0; i < length; i++)
        {
            //��Ʈ�� 1�� ���� Ȯ��.
            if (((intFlag >> i) & 1) == 0) continue;

            RecursiveCallStagesLink((Direction2Array)i);
        }

        return DirectionFlags.None;
    }
    void RecursiveCallStagesLink(Direction2Array direction)
    {
        //������ �����صξ��ٰ� �ݴ� �������� �̵����� �� ���������� ������ �̵� �Ұ�.

        //stack�� ������ �����ϰ�, ���ÿ��� ������ Ȯ���ϱ�.
        //�� ������ ��������� ����.

        Stack<Direction2Array> directionStack = new Stack<Direction2Array>();
        directionStack.Push(GetOppositeDirection(direction));
        StageFloorComponent tempFloorComponent = this;

        RecursiveCall(ref tempFloorComponent, 0, directionStack);

        if (IsDrawBackStageExist(directionStack, tempFloorComponent))
        {
            //���������� �����ϴ� ���, �̵� �Ұ�.
        }
    }
    Direction2Array GetOppositeDirection(Direction2Array direction)
    {
        //return (Direction2Array)(((int)(direction + 2)) % ((int)Direction2Array.Max));
        return (Direction2Array)((int)direction ^ 2);
    }

    void RecursiveCall(ref StageFloorComponent stageFloorComponent, int depth,
                       Stack<Direction2Array> directionStack)
    {
        if (depth <= 0) return;

        //stageFloorComponent���� �̵��� �� �ִ� ���� üũ.
        //recursiveCall�� ȣ������ �� �̵��ؿ� ������ ����.
        DirectionFlags canRecursiveStage = GetRecursiveStages(CurrentLinkedDirection(),
                                                              directionStack.Peek());
        
        //�̵��� �� �ִ� �������� ��� ȣ��.
        



    }

    bool IsDrawBackStageExist(IReadOnlyCollection<Direction2Array> directionStack,
                              StageFloorComponent tempFloorComponent)
    {
        int intDirection;
        foreach (var item in directionStack)
        {
            intDirection = (int)item;
            if (tempFloorComponent.nearComponent[intDirection] == null)
                return false;
            else
                tempFloorComponent = nearComponent[intDirection];
        }
        return true;
    }


    DirectionFlags CurrentLinkedDirection()
    {
        //1�� ��Ʈ�� �����.
        int flag = (int)DirectionFlags.None;

        int count = (int)DirectionFlags.Max;
        for (int i = 0; i < count; i++)
        {
            if (nearComponent[i] == null) continue;

            flag |= (1 << i);
        }
        return (DirectionFlags)flag;
    }
    DirectionFlags GetRecursiveStages(DirectionFlags currentLinkedDirections,
                                      Direction2Array earlierDirection)
    {
        //currentLinkedDirections���� earlierDirection ���� ����.
        int disableFlag = 1 << ((int)earlierDirection);

        return currentLinkedDirections & (~(DirectionFlags)disableFlag);
    }

    #endregion
}
