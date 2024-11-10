using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class StageButtonSet : InitObject, IPointerDownHandler
{
    Button[] buttons;
    [SerializeField] Button stageButton;
    [SerializeField] Button stageSaveButton;
    public Button StageSaveButton { get { return stageSaveButton; } }
    TextMeshProUGUI[] text;
    RectTransform rectTransform;
    ScrollRect scrollRect;
    bool isCamMove;

    int firstIndex = 0;
    int lastindex;
    int moveLength;
    [SerializeField] int offset = 120;
    [SerializeField] int offsetCount;
    [SerializeField] int maxFloor;
    int m_biggestNum;
    int temporarilyAddFloor;
    float m_yValue;
    float m_yMin;
    float m_yMax;
    Vector2 lastVec = Vector2.zero;
    Camera camMain;
    [SerializeField] FloorManager floorManager;
    FloorManager.FloorData data;
    [SerializeField] ObjectAssembly objectAssembly;
    [SerializeField] HeroTeam heroTeam;
    public HeroTeam GetHeroTeam { get => heroTeam; }
    [SerializeField] MissionExplain missionExplain;
    [SerializeField] SaveStageView saveStageView;


    class ParticipatingTeamView
    {
        Text text;
        public ParticipatingTeamView(Transform buttonTransform)
        {
            text = buttonTransform.Find("image").GetChild(0).GetComponent<Text>();
        }
        public void SetStageNum(int stageNum)
        {
            text.text = StageManager.instance.GetStageTeamCount(stageNum).ToString();
        }
    }
    ParticipatingTeamView[] participatingTeamViews;

    public override void Init()
    {
        buttons = GetComponentsInChildren<Button>();
        text = GetComponentsInChildren<TextMeshProUGUI>();
        scrollRect = transform.parent.parent.GetComponent<ScrollRect>();
        rectTransform = GetComponent<RectTransform>();

        participatingTeamViews = new ParticipatingTeamView[buttons.Length];
        for (int i = 0; i < participatingTeamViews.Length; i++)
            participatingTeamViews[i] = new ParticipatingTeamView(buttons[i].transform);

        floorManager.GetData(out data);
        saveStageView.nowFloor = data.topFloor;

        lastindex = buttons.Length - 1;
        moveLength = offset * buttons.Length;

        m_biggestNum = maxFloor;
        ButtonTextSet(m_biggestNum);

        m_yValue = buttons[0].image.rectTransform.sizeDelta.y * 0.5f - buttons[0].image.rectTransform.anchoredPosition.y;
        m_yMin = m_yValue + 1;
        m_yMax = rectTransform.sizeDelta.y + buttons[buttons.Length - 1].image.rectTransform.anchoredPosition.y
                    - (buttons[0].image.rectTransform.sizeDelta.y * 0.5f);
        camMain = Camera.main;
    }
    void ButtonMove(int index, int moveLength)
    {
        buttons[index].transform.localPosition += Vector3.up * moveLength;
    }
    void ButtonUp()
    {
        ButtonMove(lastindex, moveLength);
        int bigNum = m_biggestNum;
        text[lastindex].text = bigNum.ToString();
        buttons[lastindex].interactable = data.topFloor + temporarilyAddFloor >= bigNum;
        ChangeButtonAction(lastindex, bigNum);
        participatingTeamViews[lastindex].SetStageNum(bigNum);
        SetButtonPosition(-1);
    }
    void ButtonDown()
    {
        ButtonMove(firstIndex, -moveLength);
        int tempNum = m_biggestNum - text.Length;
        text[firstIndex].text = tempNum.ToString();
        buttons[firstIndex].interactable = data.topFloor + temporarilyAddFloor >= tempNum;
        ChangeButtonAction(firstIndex, tempNum);
        participatingTeamViews[firstIndex].SetStageNum(tempNum);
        SetButtonPosition(1);
    }
    void ChangeButtonAction(int index, int setNum)
    {
        buttons[index].onClick.RemoveAllListeners();
        buttons[index].onClick.AddListener(() => SelectStage(setNum));
    }
    public void SetButtonPosition(int numChange)
    {
        firstIndex = (firstIndex + numChange + buttons.Length) % buttons.Length;
        lastindex = (lastindex + numChange + buttons.Length) % buttons.Length;
    }
    public void ButtonTextSet(int biggestFloor)
    {

        for (int i = firstIndex; i < text.Length; i++)
        {
            text[i].text = biggestFloor.ToString();
            buttons[i].interactable = data.topFloor >= biggestFloor;
            biggestFloor--;
        }
        for (int i = 0; i < firstIndex; i++)
        {
            text[i].text = biggestFloor.ToString();
            buttons[i].interactable = data.topFloor >= biggestFloor;
            biggestFloor--;
        }
    }
    public void ViewportMove(Vector2 vec)
    {
        float dragLength = lastVec.y - vec.y;

        if (isCamMove)
            camMain.transform.position -= dragLength * objectAssembly.towerHeight * Vector3.up;

        if (dragLength == 0)
            return;

        float yOffset = m_yValue - rectTransform.anchoredPosition.y;
        if (dragLength > 0)
        {
            //내려감
            if (yOffset < 0 && m_yValue < m_yMax)
            {
                ButtonDown();
                m_yValue += offset;
                m_biggestNum--;
            }
        }
        else
        {
            //올라감
            if (yOffset > 0 && m_yValue > m_yMin)
            {
                m_biggestNum++;
                ButtonUp();
                m_yValue -= offset;
            }
        }
        lastVec = vec;

    }


    void FindStage(int fromTopStageNum, int maxStage)
    {
        int stageNum = maxStage - fromTopStageNum;
        int loopTime = Mathf.Abs(stageNum - Mathf.RoundToInt(rectTransform.anchoredPosition.y / offset));

        float newRectPositionY = stageNum * offset;
        float subtractY = newRectPositionY - rectTransform.anchoredPosition.y;
        float fValue = subtractY / Mathf.Abs(subtractY) * 0.001f;


        rectTransform.anchoredPosition = new Vector3(0, newRectPositionY, 0);

        lastVec = fromTopStageNum / maxStage * Vector2.up;
        for (int i = 0; i < loopTime; i++)
        {
            ViewportMove((0.5f - (i * fValue)) * Vector2.up);
        }
    }
    public void ActiveDrag(int stageNum)
    {
        int tempOffsetCount = offsetCount;
        if (stageNum < 11)
        {
            tempOffsetCount *= -2;
        }
        isCamMove = false;
        FindStage(stageNum - tempOffsetCount, maxFloor);
        scrollRect.velocity = new Vector2(0, -tempOffsetCount / Mathf.Abs(tempOffsetCount) * offsetCount * offset * 3);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isCamMove = true;
    }
    #region 스테이지 선택
    public void SelectStage(int stageNum)
    {
        heroTeam.SetTeamActiveCount(StageManager.instance.GetStageTeamCount(stageNum));

        stageButton.interactable = data.topFloor >= stageNum - temporarilyAddFloor;
        stageSaveButton.interactable = stageButton.interactable && saveStageView.IsLastIndex;

        missionExplain.ChangeExplain(stageNum);
        saveStageView.NowNode.SelectStage(stageNum);
    }
    public void RemainStage(int stageNum)
    {
        missionExplain.ChangeExplain(stageNum);
    }

    #endregion
    #region 스테이지 선택 범위 변화
    public void StageRangeAdd(int offset)
    {
        temporarilyAddFloor += offset;
    }
    public void StageSetInteractive(int floor)
    {
        int range = m_biggestNum - floor;

        if (Mathf.Abs(range) > buttons.Length)
            return;

        int index = (firstIndex + range - 1) % buttons.Length;
        buttons[index].interactable = true;
        ChangeButtonAction(index, floor + 1);
    }
    public void StageSetInteractiveWhile(int floor)
    {
        int floorDecrease = floor - data.topFloor;

        if (floorDecrease < 0) return;

        int index = firstIndex;
        int time = m_biggestNum - floor;

        while (time > 0)
        {
            buttons[index].interactable = false;
            index = (index + 1) % buttons.Length;
            time--;
        }

    }
    private void OnDisable()
    {
        temporarilyAddFloor = 0;
        ButtonTextSet(m_biggestNum);
        stageSaveButton.gameObject.SetActive(false);
        stageButton.gameObject.SetActive(false);
        stageButton.interactable = false;
    }
    #endregion
    public void AllButtonSet(bool onoff)
    {
        stageButton.interactable = onoff;
        StageSaveButton.interactable = onoff;
    }
}
