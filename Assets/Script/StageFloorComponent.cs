using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class StageFloorComponent : MonoBehaviour
{
    NavMeshLink[] navMeshLinks;
    [SerializeField] NavMeshLink linkPrefab;
    Transform navMeshParent;

    static readonly int[] linkPosition = new int[4] { 0, 1, 0, -1 };

    static readonly float[] planeSize = new float[1] { 25.5f };
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
        navMeshLinks = new NavMeshLink[directionCount];

    }
    public void NewLink(Direction2Array direction)
    {
        int iDirection = (int)direction;

        navMeshLinks[iDirection] = Instantiate(linkPrefab, navMeshParent);
        navMeshLinks[iDirection].transform.SetLocalPositionAndRotation(GetPosition(direction), Quaternion.Euler(0, (iDirection % 2) * 90, 0));
        navMeshLinks[iDirection].gameObject.SetActive(true);
    }
    Vector3 GetPosition(Direction2Array direction)
    {
        int iDirection = (int)direction;
        float xValue = linkPosition[iDirection] * planeSize[stageIndex];
        float zValue = linkPosition[(iDirection + 1) % (int)Direction2Array.Max] * planeSize[stageIndex];

        return new Vector3(xValue, 0, zValue);
    }
}
