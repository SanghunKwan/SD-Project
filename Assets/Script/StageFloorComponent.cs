using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;


public class StageFloorComponent : InitObject
{
    public const float localPlaneLength = 5;
    const float localLinkOffset = 0.45f;

    public float PlaneMagnification { get; private set; }

    StageFloorComponent[] nearComponent;
    Transform navMeshParent;

    [SerializeField] int stageIndex;


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
        navMeshParent = transform.GetChild(0);
        PlaneMagnification = navMeshParent.localScale.x;

        nearComponent = new StageFloorComponent[(int)DirectionFlags.Max];
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
        nearComponent[iDirection].nearComponent[(iDirection + 2) % (int)DirectionFlags.Max] = this;
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
        float zValue = battleClearManager.linkPosition[(iDirection + 1) % (int)DirectionFlags.Max] * linkOffset;

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

    DirectionFlags CheckUndirectlyLinkedStages(DirectionFlags unlinkedFlags)
    {
        //��Ʈ�� 1�� ���� ��ȸ�ؼ� �� �� �ִ��� Ȯ��.
        int intFlag = (int)unlinkedFlags;
        int length = (int)DirectionFlags.Max;
        for (int i = 0; i < length; i++)
        {
            //��Ʈ�� 1�� ���� Ȯ��.
            if (((intFlag >> i) & 1) == 0) continue;

            if (RecursiveCallStagesLink((Direction2Array)i))
            {
                intFlag &= ~(1 << i); //�� �� �ִ� ���� ��Ʈ ����.
                Debug.Log("��Ʈ ����");
            }
        }

        return (DirectionFlags)intFlag;
    }
    bool RecursiveCallStagesLink(Direction2Array direction)
    {
        //������ �����صξ��ٰ� �ݴ� �������� �̵����� �� ���������� ������ �̵� �Ұ�.
        //¦����ŭ ������ ���������� ��. �� 1*2�� ����(�̹� ������)
        //���ݸ�ŭ ������ ���������� �������� �� �̵� ������ ���� ������ ���� ���������� ������ �̵� �Ұ�.
        int pairCount = (transform.GetSiblingIndex() + 1) / 2;
        if (pairCount <= 1) return false;


        uint savedDirection = AddDirection(0u, direction);
        Debug.Log(direction);
        //��͸� �̿��� �߰� ���������� �ֺ� ���������� ���� �̵�.
        return RecursiveCall(this, pairCount - 1, 1,
                             direction, savedDirection);
    }
    uint AddDirection(uint savedDirection, Direction2Array earlierDirection)
    {
        int bitMove = (int)earlierDirection * 8; //�̵��� ������ ��Ʈ ��ġ�� ���.
        uint directionMask = (0xFFu << bitMove); //8��Ʈ ������ �����ϹǷ� 0xFF ���. 0b11111111
        if ((savedDirection & directionMask) == directionMask)
        {
            Debug.Log("�����÷ο찡 �߻��߽��ϴ�.");
            return uint.MinValue; //�����÷ο찡 �߻����� ���
        }


        return savedDirection + (1u << bitMove);
        //8��Ʈ ������ �����ϹǷ� �̵������ڿ� 8�� ����.
    }

    Direction2Array GetOppositeDirection(Direction2Array direction)
    {
        //return (Direction2Array)(((int)(direction + 2)) % ((int)Direction2Array.Max));
        return (Direction2Array)((int)direction ^ 2);
    }


    bool RecursiveCall(StageFloorComponent stageFloorComponent, int pairCount, int directionCount,
                       Direction2Array earlierDirection, uint savedDirection)
    {
        //stageFloorComponent���� �̵��� �� ���� ����.
        DirectionFlags movableDirectionFlags = RemoveDirection(stageFloorComponent.CurrentLinkedDirection(),
                                                               earlierDirection);
        //Debug.Log(stageFloorComponent.CurrentLinkedDirection() + ":" + movableDirectionFlags);
        int intFlag = (int)movableDirectionFlags;
        int length = (int)DirectionFlags.Max;

        Direction2Array oppositeDirection;
        for (int i = 0; i < length; i++)
        {
            //��Ʈ�� 1�� ���� Ȯ��.
            if (((intFlag >> i) & 1) == 0) continue;


            if (directionCount > 1)
            {
                Debug.Log("savedDirection :" + savedDirection);
                //directionCount�� 1�� ���� �̹� ����ó���� ��.
                if (CompareStageLinks(stageFloorComponent, directionCount, earlierDirection, savedDirection))
                    return true;
            }
            //Debug.Log(pairCount);
            if (pairCount > 0)
            {
                oppositeDirection = GetOppositeDirection((Direction2Array)i);

                savedDirection = AddDirection(savedDirection, oppositeDirection);

                if (RecursiveCall(stageFloorComponent.nearComponent[i], pairCount - 1, directionCount + 1,
                              oppositeDirection, savedDirection))
                    return true;
            }
        }

        return false;

        //�̵��� �� �ִ� �������� ��� ȣ��.
    }
    bool CompareStageLinks(StageFloorComponent stageFloorComponent, int directionCount,
                           Direction2Array earlierDirection, uint savedDirection)
    {
        DirectionFlags movableDirectionFlags = RemoveDirection(stageFloorComponent.CurrentLinkedDirection(),
                                               earlierDirection);
        //Debug.Log(movableDirectionFlags + ":" + earlierDirection + ":" + directionCount + ":" + stageFloorComponent.transform.GetSiblingIndex());
        int intFlag = (int)movableDirectionFlags;
        int length = (int)DirectionFlags.Max;
        int nextDirectionCount = directionCount - 1;
        uint savedDirectionCopy;//���纻�� ���� ���.
        Direction2Array nextDirection;

        for (int i = 0; i < length; i++)
        {
            savedDirectionCopy = savedDirection;
            //��Ʈ�� 1�� ���� Ȯ��.
            if (((intFlag >> i) & 1) == 0) continue;

            nextDirection = (Direction2Array)i;

            if (!SubtractDirection(ref savedDirectionCopy, nextDirection)) continue;

            if (savedDirectionCopy == 0)
            {
                //��� ����� ���� ���.
                return true;
            }
            else
            {
                if (CompareStageLinks(stageFloorComponent.nearComponent[i], nextDirectionCount,
                                  GetOppositeDirection(nextDirection), savedDirectionCopy))
                    return true;
            }
        }
        return false;
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
    DirectionFlags RemoveDirection(DirectionFlags currentLinkedDirections,
                                      Direction2Array earlierDirection)
    {
        //currentLinkedDirections���� earlierDirection ���� ����.
        return currentLinkedDirections & (~(DirectionFlags)(1 << ((int)earlierDirection)));
    }
    bool SubtractDirection(ref uint savedDirection, Direction2Array earlierDirection)
    {
        int bitMove = (int)earlierDirection * 8; //�̵��� ������ ��Ʈ ��ġ�� ���.
        uint directionMask = (0xFFu << bitMove); //8��Ʈ ������ �����ϹǷ� 0xFF ���. 0b11111111
        //�̵��� ������ ����.


        if ((savedDirection & directionMask) == 0)
        {
            //Debug.Log("����" + earlierDirection);
            return false; //����� ������ ���� ���
        }
        //Debug.Log("����" + earlierDirection);
        savedDirection -= (1u << bitMove); //���� 1 ����
        return true;
    }
    Direction2Array GetRandomDirectionByReservoir(DirectionFlags flags)
    {
        Debug.Log("�� �� �ִ� flag  :" + flags);
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

    #endregion
    #region �ܺ� ȣ��
    public Direction2Array GetDirectionToNear(StageFloorComponent stageFloorComponent)
    {
        int index = System.Array.IndexOf(nearComponent, stageFloorComponent);

        return (index == -1) ? Direction2Array.Max : (Direction2Array)index;
    }

    #endregion
}
