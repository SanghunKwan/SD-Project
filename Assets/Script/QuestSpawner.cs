using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestSpawner : MonoBehaviour
{
    QuestManager questManager;
    QuestSaveData questSaveData;

    Action<QuestManager.QuestType, int>[] makeExistingQuestActions;
    Action<QuestManager.QuestData>[] makeQuestActions;
    Action<Vector3, float>[] highLightActions;


    public QuestManager.QuestType[] types;
    [SerializeField] QuestUIViewer questUIViewer;
    [SerializeField] RectTransform backGroundTransform;
    QuestBackGround questBackGround;

    // Start is called before the first frame update
    private void Awake()
    {
        questBackGround = backGroundTransform.Find("QuestBackGround").GetComponent<QuestBackGround>();

        SetActions();
    }
    void SetActions()
    {
        makeQuestActions = new Action<QuestManager.QuestData>[(int)QuestManager.QuestTrigger.Max];
        makeQuestActions[(int)QuestManager.QuestTrigger.Location] = MakeLocationQuest;
        makeQuestActions[(int)QuestManager.QuestTrigger.UnitSpecificAct] = MakeActionQuest;

        makeExistingQuestActions = new Action<QuestManager.QuestType, int>[(int)QuestSaveData.SaveDataBit.Complete];
        makeExistingQuestActions[(int)QuestSaveData.SaveDataBit.Enable] = PrepareQuest;
        makeExistingQuestActions[(int)QuestSaveData.SaveDataBit.Doing] = MakeQuest;

        highLightActions = new Action<Vector3, float>[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Max];
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Vector] = HighLightVector;
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Hero] = HighLightVector;
        highLightActions[(int)QuestManager.QuestData.QuestHighLight.HighLightTarget.Object] = HighLightVector;
    }
    void Start()
    {
        questManager = GameManager.manager.questManager;
        questSaveData = GameManager.manager.battleClearManager.SaveDataInfo.questSaveData;
        CheckDataEmptyNInit(questSaveData);

        MakeExistingQuest();
    }
    void CheckDataEmptyNInit(QuestSaveData data)
    {
        if (!data.isLoaded)
        {
            data.SetDataSize();
        }
        data.Init();
    }

    void MakeExistingQuest()
    {
        if (!questSaveData.isLoaded)
            return;

        QuestSaveData.SaveDataBit data;
        int length;
        foreach (var item in types)
        {
            length = questManager.GetQuestCount(item);
            for (int i = 0; i < length; i++)
            {
                data = questSaveData.GetQuestState(item, i);
                makeExistingQuestActions[(int)data]?.Invoke(item, i);
            }
        }

    }

    void MakeQuest(QuestManager.QuestType type, int questNum)
    {
        QuestManager.QuestData data = questManager.GetQuestData(type, questNum);
        makeQuestActions[(int)data.trigger](data);
    }
    #region MakeQuest 호출 Action
    void MakeLocationQuest(QuestManager.QuestData data)
    {
        //퀘스트 설명 UI 생성
        questUIViewer.ShowNewQuest(out QuestUISlot slot);
        slot.SetQuestText(data.name, data.detail);

        //퀘스트 관련 backGround
        QuestHighLight(data.highLight);



        //퀘스트 클리어 조건 형성
    }
    void MakeActionQuest(QuestManager.QuestData data)
    {

    }
    #endregion
    #region 하이라이트 actions
    void QuestHighLight(QuestManager.QuestData.QuestHighLight highlight)
    {
        if (highlight.timeStop)
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            //timeDelay(spaceBar)가 작동하지 않도록 예외처리 필요
        }

        highLightActions[(int)highlight.highLight](highlight.highLightPosition, highlight.size);
    }
    void HighLightVector(Vector3 highLightPosition, float highLightSize)
    {
        questBackGround.SetHighLight(highLightPosition, highLightSize);
    }
    void HighLightHero(Vector3 highLightPosition, float highLightSize)
    {
        questBackGround.SetHighLight(highLightPosition, highLightSize);
    }
    void HighLightObject(Vector3 highLightPosition, float highLightSize)
    {
        questBackGround.SetHighLight(highLightPosition, highLightSize);
    }
    #endregion
    void PrepareQuest(QuestManager.QuestType type, int questNum)
    {

    }
    #region PrepareQuest 관련 action

    #endregion
}
