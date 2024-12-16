using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuestSpawner : MonoBehaviour
{
    QuestManager questManager;
    QuestSaveData questSaveData;

    Action<QuestManager.QuestData>[] makeQuestActions;

    // Start is called before the first frame update
    private void Awake()
    {
        makeQuestActions = new Action<QuestManager.QuestData>[(int)QuestManager.QuestTrigger.Max];
        makeQuestActions[(int)QuestManager.QuestTrigger.Location] = MakeLocationQuest;
        makeQuestActions[(int)QuestManager.QuestTrigger.UnitSpecificAct] = MakeActionQuest;
    }
    void Start()
    {
        questManager = GameManager.manager.questManager;
        questSaveData = GameManager.manager.battleClearManager.SaveDataInfo.questSaveData;
        CheckDataEmptyNInit(questSaveData);

    }
    void CheckDataEmptyNInit(QuestSaveData data)
    {
        if (!data.isLoaded)
        {
            data.SetDataSize();
        }
        data.Init();
    }

    void MakeQuest(QuestManager.QuestType type, int questNum)
    {
        QuestManager.QuestData data = questManager.GetQuestData(type, questNum);
        makeQuestActions[(int)data.trigger](data);
    }
    void MakeLocationQuest(QuestManager.QuestData data)
    {
        //����Ʈ ���� UI
        //����Ʈ ���� backGround
        //����Ʈ Ŭ���� ���� ����
    }
    void MakeActionQuest(QuestManager.QuestData data)
    {

    }
    void QuestHighLight(QuestManager.QuestData.QuestHighLight highlight)
    {
        if (highlight.timeStop)
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            //timeDelay�� �۵����� �ʵ��� ����ó�� �ʿ�
        }
        if (highlight.highLight)
        {

            //highlight.highLightPosition
            //���⿡ ���̶���Ʈ ǥ��.
        }
    }
}
