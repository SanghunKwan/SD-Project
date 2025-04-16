using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveStageView : InitObject
{
    SaveStageViewNode[] nodes;
    int nowIndex;
    int tempAddFloor;
    public int nowFloor { private get; set; }
    int nodeLengthMinusOne;
    [SerializeField] Button button;
    [SerializeField] StageButtonSet stageButtonSet;
    List<int> nodesToRemove = new List<int>();

    public bool IsLastIndex { get { return nowIndex < nodeLengthMinusOne; } }
    [SerializeField] HighLightImage highLightImage;

    public override void Init()
    {
        nodes = GetComponentsInChildren<SaveStageViewNode>();
        foreach (var node in nodes)
            node.Init();
        nodeLengthMinusOne = nodes.Length - 1;
        nodesToRemove.Capacity = nodeLengthMinusOne;
    }
    public SaveStageViewNode NowNode { get { return nodes[nowIndex]; } }

    public void SaveStage()
    {
        MaxFloorIncrease();

        NowNode.MakeLine(true);
        nodes[nowIndex + 1].SelectStage(NowNode.saveStage);
        nodes[nowIndex].AnimRepeatStop();

        do
        {
            nowIndex++;

        } while (!NowNode.IsInteractive);

        button.interactable = IsLastIndex;
        GameManager.manager.onVilligeButton.eventAction?.Invoke((int)GameManager.ActionButtonNum.VilligeExpeditionProlongBtn, Vector3.zero);
    }

    void MaxFloorIncrease()
    {
        if (NowNode.saveStage != nowFloor + tempAddFloor)
            return;

        stageButtonSet.StageRangeAdd(1);
        stageButtonSet.StageSetInteractive(NowNode.saveStage);
        tempAddFloor++;
    }
    private void OnDisable()
    {
        foreach (var item in nodes)
            item.StageReset();

        nowIndex = 0;
        tempAddFloor = 0;
    }
    #region 버튼입력 이벤트
    public void NodeButtonPress(int nodeIndex)
    {
        int tempFloor = nodes[nodeIndex].saveStage;

        if (nodes[nodeIndex].saveStage >= nowFloor && !AddFloorCheck(nodeIndex))
            nodes[nodeIndex].exitAction = () =>
            {
                RemoveFromList(nodeIndex);
                MaxFloorDecrease(tempFloor);
                nodesToRemove.Clear();
              
            };
        else
        {
            nodes[nodeIndex].exitAction = () =>
            {
                NodeShift(nodeIndex);
            };
        }

        NowNode.AnimRepeatStop();

        nodes[nodeIndex].NodeReset();
        GameManager.manager.onVilligeExpeditionFloorDelete.eventAction?.Invoke(nodeIndex, Vector3.zero);
    }
    bool SearchNum(int tempFloor, int nodeIndex)
    {
        bool found = false;

        for (int i = 0; i < nodeIndex; i++)
        {
            found |= nodes[i].saveStage == tempFloor;
        }
        return found;
    }

    void NodeShift(int nodeIndex)
    {
        int i = nodeIndex;
        int j = i + 1;
        for (; i < nodeLengthMinusOne && nodes[j].IsInteractive; i++)
        {
            nodes[i].LoadStage(nodes[j].saveStage);
            j++;
        }
        nodes[i].StageReset();

        LastNodeSet(i);
    }
    bool AddFloorCheck(int nodeIndex)
    {
        bool defaultBool = false;
        bool latelyFind = false;
        for (int i = 0; i < nodeIndex && !defaultBool; i++)
        {
            defaultBool |= nodes[i].saveStage == nodes[nodeIndex].saveStage;
        }

        for (int i = nodeIndex + 1; i < nodes.Length && !defaultBool
                                                     && nodes[i].saveStage <= nodes[nodeIndex].saveStage; i++)
        {
            defaultBool |= nodes[i].saveStage == nodes[nodeIndex].saveStage;
        }

        for (int i = nodeIndex + 1; i < nodes.Length && !latelyFind; i++)
        {
            latelyFind |= nodes[i].saveStage == nodes[nodeIndex].saveStage;
            if (nodes[i].saveStage > nodes[nodeIndex].saveStage)
                nodesToRemove.Add(i);
        }
        return defaultBool;
    }
    void RemoveFromList(int nodeIndex)
    {
        int listIndex = 0;
        int nextIndex = nodeIndex + 1;
        while (nextIndex < nodes.Length && nodes[nextIndex].saveStage != 0)
        {
            while (listIndex < nodesToRemove.Count && nodesToRemove[listIndex] == nextIndex)
            {
                nextIndex++;
                listIndex++;
            }

            if (nextIndex < nodes.Length && nodes[nextIndex].saveStage != 0)
            {
                nodes[nodeIndex].LoadStage(nodes[nextIndex].saveStage);
                nodeIndex++;
                nextIndex++;
            }
        }
        LastNodeSet(nodeIndex);

        for (; nodeIndex < nodes.Length; nodeIndex++)
        {
            nodes[nodeIndex].StageReset();
        }
    }
    void LastNodeSet(int nodeIndex)
    {
        if (nodeIndex == 0)
        {
            stageButtonSet.AllButtonSet(false);
            highLightImage.CallFloor(-10);
        }
        else
        {
            nodeIndex--;
            highLightImage.CallFloor(nodes[nodeIndex].saveStage);
            nodes[nodeIndex].NewLastNode();
            stageButtonSet.AllButtonSet(true);
        }
        stageButtonSet.RemainStage(nodes[nodeIndex].saveStage);
        nowIndex = nodeIndex;

        bool isLastBiggest = true;
        for (int i = 0; i < nodeIndex; i++)
        {
            isLastBiggest &= nodes[nodeIndex].saveStage > nodes[i].saveStage;
        }
        if (isLastBiggest)
        {
            stageButtonSet.StageSetInteractiveWhile(nodes[nodeIndex].saveStage);
            stageButtonSet.StageRangeAdd(-1);
            tempAddFloor -= 1;
        }
    }
    void MaxFloorDecrease(int eraseStage)
    {
        int containNow = ContainNowFloor(eraseStage);

        int offset = eraseStage - nowFloor + containNow;
        stageButtonSet.StageSetInteractiveWhile(eraseStage + containNow);
        stageButtonSet.StageRangeAdd(offset - tempAddFloor);
        tempAddFloor = offset;
    }
    int ContainNowFloor(int tempFloor)
    {
        bool defaultFalse = false;
        for (int i = 0; i < nowIndex; i++)
        {
            defaultFalse |= nodes[i].saveStage == tempFloor - 1;
        }
        if (defaultFalse || tempFloor == nowFloor)
            return 0;

        return -1;
    }
    #endregion

    public int[] GetStageFloorsData()
    {
        int length = nowIndex + 1;
        int[] data = new int[length];
        for (int i = 0; i < length; i++)
            data[i] = nodes[i].saveStage;

        return data;
    }
}
