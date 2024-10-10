using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StageButtonSet : InitInterface, IPointerDownHandler
{
    Button[] buttons;
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
    float m_yValue;
    float m_yMin;
    float m_yMax;
    Vector2 lastVec = Vector2.zero;
    Camera camMain;
    [SerializeField] FloorManager floorManager;
    FloorManager.FloorData data;
    [SerializeField] ObjectAssembly objectAssembly;

    public override void Init()
    {
        buttons = GetComponentsInChildren<Button>();
        text = GetComponentsInChildren<TextMeshProUGUI>();
        scrollRect = transform.parent.parent.GetComponent<ScrollRect>();
        floorManager.GetData(out data);

        lastindex = buttons.Length - 1;
        moveLength = offset * buttons.Length;

        m_biggestNum = maxFloor;
        ButtonTextSet(m_biggestNum);
        ButtonSetInteractive(m_biggestNum);

        rectTransform = GetComponent<RectTransform>();

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
        text[lastindex].text = m_biggestNum.ToString();
        buttons[lastindex].interactable = data.nowFloor + 1 >= m_biggestNum;
        SetButtonPosition(-1);
    }
    void ButtonDown()
    {
        ButtonMove(firstIndex, -moveLength);
        int tempNum = m_biggestNum - text.Length;
        text[firstIndex].text = tempNum.ToString();
        buttons[firstIndex].interactable = data.nowFloor + 1 >= tempNum ;
        SetButtonPosition(1);
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
            biggestFloor--;
        }
        for (int i = 0; i < firstIndex; i++)
        {
            text[i].text = biggestFloor.ToString();
            biggestFloor--;
        }
    }
    public void ButtonSetInteractive(int biggestFloor)
    {
        int enableFloor = data.nowFloor + 1;
        for (int i = firstIndex; i < buttons.Length; i++)
        {
            buttons[i].interactable = enableFloor >= biggestFloor;
            biggestFloor--;
        }
        for (int i = 0; i < firstIndex; i++)
        {
            buttons[i].interactable = enableFloor >= biggestFloor;
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
}
