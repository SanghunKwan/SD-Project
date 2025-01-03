using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using SaveData;

[RequireComponent(typeof(BattleClearPool))]
public class BattleClearManager : MonoBehaviour
{
    [SerializeField] int[] m_linkPosition;
    public int[] linkPosition { get { return m_linkPosition; } }

    public int nowFloorIndex { get; private set; }
    int lastFloorIndex;

    [SerializeField] Vector2[] m_stagePosition;
    public Vector2[] stagePosition { get { return m_stagePosition; } }

    [SerializeField] float[] m_planeSize;
    public float[] planeSize { get { return m_planeSize; } }

    public StageFloorComponent[] stageFloorComponents { get; private set; }

    SaveDataInfo saveData;
    public SaveDataInfo SaveDataInfo { get { return saveData; } }

    [SerializeField] LoadSaveManager loadSaveManager;
    BattleClearPool battleClearPool;

    [SerializeField] UnityEngine.UI.Button StageEndButton;


    Action[] setStageByNextScene;
    public Action<int> SetActiveNewStageObjects { get; set; }
    public Action onStageChanged { get; set; }

    public enum OBJECTNUM
    {
        BONEWALL,
    }
    private void Awake()
    {
        battleClearPool = GetComponent<BattleClearPool>();

        SetAction();
    }
    void SetAction()
    {
        setStageByNextScene = new Action[3];
        setStageByNextScene[0] = () => { };
        setStageByNextScene[1] = () => CallStages(saveData.stageData.floors);
        setStageByNextScene[2] = () => { /*건물 가져오기, tower 설정하기 */};
    }
    private void Start()
    {
        loadSaveManager.LoadData(StageManager.instance.saveDataIndex, out saveData);
        GameManager.manager.SetBattleClearManager(this);
        GameManager.manager.onPlayerEnterStage.eventAction += (num, vec) => { if (num == -1) nowFloorIndex++; };
        SetStage(saveData.nextScene);

    }

    public CObject CallObject(OBJECTNUM num, Transform trans)
    {
        CObject obj = transform.GetChild((int)num).GetChild(0).GetComponent<CObject>();
        obj.gameObject.SetActive(true);
        obj.transform.position = trans.position;
        obj.transform.SetParent(transform);

        return obj;
    }
    void SetStage(int nextScene)
    {
        setStageByNextScene[nextScene]();
    }
    void CallStages(in int[] floors)
    {
        lastFloorIndex = floors.Length;
        stageFloorComponents = new StageFloorComponent[lastFloorIndex];

        stageFloorComponents[0] = battleClearPool.MakeStage(PoolStageIndex(floors[0]));
        stageFloorComponents[0].gameObject.SetActive(true);
        StageFloorComponent.Direction2Array randomDirection;
        for (int i = 1; i < lastFloorIndex; i++)
        {
            stageFloorComponents[i] = battleClearPool.MakeStage(PoolStageIndex(floors[i]));
            randomDirection = stageFloorComponents[i - 1].GetEmptyDirection();
            stageFloorComponents[i - 1].NewStage(stageFloorComponents[i], randomDirection);
            stageFloorComponents[i - 1].NewLink(battleClearPool.MakeLink(), randomDirection);
        }
    }
    public int PoolStageIndex(int nowFloor)
    {
        return (nowFloor / 10) + (Convert.ToInt32(nowFloor % 10 == 0) * 10);
    }

    public void ActivateNextFloor(in QuestSpawner questSpawner)
    {
        if (nowFloorIndex + 1 < lastFloorIndex)
        {
            GetStageComponent(1).gameObject.SetActive(true);
            questSpawner.PrepareQuest(QuestManager.QuestType.FloorQuest, 1);
            onStageChanged?.Invoke();
        }
        else
        {
            Debug.Log("마지막 스테이지 클리어!");
            StageEndButton.gameObject.SetActive(true);
        }
    }
    public void ActiveStageObject()
    {
        SetActiveNewStageObjects(nowFloorIndex);
    }
    public StageFloorComponent GetStageComponent(int offsetIndex)
    {
        return stageFloorComponents[nowFloorIndex + offsetIndex];
    }
}
