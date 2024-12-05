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

    [SerializeField] Vector2[] m_stagePosition;
    public Vector2[] stagePosition { get { return m_stagePosition; } }

    [SerializeField] float[] m_planeSize;
    public float[] planeSize { get { return m_planeSize; } }

    StageFloorComponent[] stageFloorComponents;

    SaveDataInfo saveData;
    Action[] setStageByNextScene;

    [SerializeField] LoadSaveManager loadSaveManager;
    BattleClearPool battleClearPool;

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
        GameManager.manager.battleClearManager = this;

        loadSaveManager.LoadData(StageManager.instance.saveDataIndex, out saveData);
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
        int length = floors.Length;
        stageFloorComponents = new StageFloorComponent[length];
        stageFloorComponents[0] = battleClearPool.MakeStage(PoolStageIndex(floors[0]));
        StageFloorComponent.Direction2Array randomDirection;
        for (int i = 1; i < length; i++)
        {
            stageFloorComponents[i] = battleClearPool.MakeStage(PoolStageIndex(floors[i]));
            randomDirection = stageFloorComponents[i - 1].GetEmptyDirection();
            stageFloorComponents[i - 1].NewStage(stageFloorComponents[i], randomDirection);
            stageFloorComponents[i - 1].NewLink(battleClearPool.MakeLink(), randomDirection);
        }
    }
    int PoolStageIndex(int nowFloor)
    {
        if (nowFloor % 10 != 0)
            return nowFloor / 10;
        else
            return 10;
    }
}
