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

        //거리 재설정 필요
        //스테이지 사이 거리 : planeSize + planeSize + linkLength
        //링크 위치 : planeSize + 0.5f * linkLength


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

        //unlinkedFlags에서 못 가는 곳 제외.
        unlinkedFlags = CheckUndirectlyLinkedStages(unlinkedFlags);

        return GetRandomDirectionByReservoir(unlinkedFlags);
    }

    DirectionFlags CheckUndirectlyLinkedStages(DirectionFlags unlinkedFlags)
    {
        //비트가 1인 곳은 순회해서 갈 수 있는지 확인.
        int intFlag = (int)unlinkedFlags;
        int length = (int)DirectionFlags.Max;
        for (int i = 0; i < length; i++)
        {
            //비트가 1인 곳만 확인.
            if (((intFlag >> i) & 1) == 0) continue;

            if (RecursiveCallStagesLink((Direction2Array)i))
            {
                intFlag &= ~(1 << i); //갈 수 있는 곳은 비트 제거.
                Debug.Log("비트 제거");
            }
        }

        return (DirectionFlags)intFlag;
    }
    bool RecursiveCallStagesLink(Direction2Array direction)
    {
        //방향을 저장해두었다가 반대 방향으로 이동했을 때 스테이지가 있으면 이동 불가.
        //짝수만큼 떨어진 스테이지와 비교. 단 1*2는 예외(이미 연산함)
        //절반만큼 떨어진 스테이지를 기준으로 각 이동 방향의 합이 동일한 곳에 스테이지가 있으면 이동 불가.
        int pairCount = (transform.GetSiblingIndex() + 1) / 2;
        if (pairCount <= 1) return false;


        uint savedDirection = AddDirection(0u, direction);
        Debug.Log(direction);
        //재귀를 이용해 중간 스테이지와 주변 스테이지를 따라 이동.
        return RecursiveCall(this, pairCount - 1, 1,
                             direction, savedDirection);
    }
    uint AddDirection(uint savedDirection, Direction2Array earlierDirection)
    {
        int bitMove = (int)earlierDirection * 8; //이동할 방향의 비트 위치를 계산.
        uint directionMask = (0xFFu << bitMove); //8비트 단위로 저장하므로 0xFF 사용. 0b11111111
        if ((savedDirection & directionMask) == directionMask)
        {
            Debug.Log("오버플로우가 발생했습니다.");
            return uint.MinValue; //오버플로우가 발생했을 경우
        }


        return savedDirection + (1u << bitMove);
        //8비트 단위로 저장하므로 이동연산자에 8을 곱함.
    }

    Direction2Array GetOppositeDirection(Direction2Array direction)
    {
        //return (Direction2Array)(((int)(direction + 2)) % ((int)Direction2Array.Max));
        return (Direction2Array)((int)direction ^ 2);
    }


    bool RecursiveCall(StageFloorComponent stageFloorComponent, int pairCount, int directionCount,
                       Direction2Array earlierDirection, uint savedDirection)
    {
        //stageFloorComponent에서 이동해 온 방향 제거.
        DirectionFlags movableDirectionFlags = RemoveDirection(stageFloorComponent.CurrentLinkedDirection(),
                                                               earlierDirection);
        //Debug.Log(stageFloorComponent.CurrentLinkedDirection() + ":" + movableDirectionFlags);
        int intFlag = (int)movableDirectionFlags;
        int length = (int)DirectionFlags.Max;

        Direction2Array oppositeDirection;
        for (int i = 0; i < length; i++)
        {
            //비트가 1인 곳만 확인.
            if (((intFlag >> i) & 1) == 0) continue;


            if (directionCount > 1)
            {
                Debug.Log("savedDirection :" + savedDirection);
                //directionCount가 1인 경우는 이미 예외처리가 됨.
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

        //이동할 수 있는 방향으로 재귀 호출.
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
        uint savedDirectionCopy;//복사본을 만들어서 사용.
        Direction2Array nextDirection;

        for (int i = 0; i < length; i++)
        {
            savedDirectionCopy = savedDirection;
            //비트가 1인 곳만 확인.
            if (((intFlag >> i) & 1) == 0) continue;

            nextDirection = (Direction2Array)i;

            if (!SubtractDirection(ref savedDirectionCopy, nextDirection)) continue;

            if (savedDirectionCopy == 0)
            {
                //모든 저장된 방향 사용.
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
        //1인 비트는 연결됨.
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
        //currentLinkedDirections에서 earlierDirection 방향 제외.
        return currentLinkedDirections & (~(DirectionFlags)(1 << ((int)earlierDirection)));
    }
    bool SubtractDirection(ref uint savedDirection, Direction2Array earlierDirection)
    {
        int bitMove = (int)earlierDirection * 8; //이동할 방향의 비트 위치를 계산.
        uint directionMask = (0xFFu << bitMove); //8비트 단위로 저장하므로 0xFF 사용. 0b11111111
        //이동한 방향을 제거.


        if ((savedDirection & directionMask) == 0)
        {
            //Debug.Log("없음" + earlierDirection);
            return false; //저장된 방향이 없을 경우
        }
        //Debug.Log("있음" + earlierDirection);
        savedDirection -= (1u << bitMove); //방향 1 감소
        return true;
    }
    Direction2Array GetRandomDirectionByReservoir(DirectionFlags flags)
    {
        Debug.Log("갈 수 있는 flag  :" + flags);
        //reservoir sampling 알고리즘을 사용하여 flags에서 랜덤으로 활성화된 방향을 반환.

        //기본값으로 가장 낮은 비트 값 활성화.
        int intFlags = (int)flags;
        int selectedDirection = intFlags & (-intFlags); // flags의 최하위 비트값을 선택.

        // 다음 값들을 순회함.
        int reservoirSeen = 1; //기본값 카운트
        int length = (int)DirectionFlags.All; // length = (1 << DirectionFlags.Max)
        for (int i = selectedDirection; i < length; i <<= 1)
        {
            if ((i & intFlags) != 0)
            {
                reservoirSeen++;
                if (Random.Range(0, reservoirSeen) != 0) continue;

                selectedDirection = i; // 현재 방향을 선택.
            }
        }

        return (Direction2Array)GetSmallestBitIndex(selectedDirection); // 선택된 방향의 인덱스를 반환.
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
    #region 외부 호출
    public Direction2Array GetDirectionToNear(StageFloorComponent stageFloorComponent)
    {
        int index = System.Array.IndexOf(nearComponent, stageFloorComponent);

        return (index == -1) ? Direction2Array.Max : (Direction2Array)index;
    }

    #endregion
}
