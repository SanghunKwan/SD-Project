using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class StageManager : JsonLoad
{
    public static StageManager instance;

    [SerializeField] protected Animator sceneFadeAnim;
    int triggerNamedNoDelay;
    public int targetSceneNum;
    public int saveDataIndex { get; set; }
    protected List<UnityAction<Scene, LoadSceneMode>> loadActionList;
    public bool buildingConstructingReady { get; private set; }
    public LinkedList<VilligeBuildingConstructing> constructingUIList;

    [Serializable]
    public class StageData
    {
        public int nowFloor;
        public int participatingTeamCount;
        public string stageName;
        public string stagePurpose;
        public int[] stageRewardsImage;

    }
    public StageDatas stageData { get; private set; }
    [Serializable]
    public class StageDatas
    {
        public StageData[] data;

    }


    protected void Awake()
    {
        VirtualAwake();
        instance = this;
        constructingUIList = new LinkedList<VilligeBuildingConstructing>();
    }
    protected virtual void VirtualAwake()
    {
        loadActionList = new(3);
        stageData = LoadData<StageDatas>("StageData");
        triggerNamedNoDelay = Animator.StringToHash("fadeOutNoDelay");
    }
    void Start()
    {
        SceneReveal();
    }

    #region 씬
    public void CallLoadingScene(int nextScene)
    {
        SceneLoadActionAdd(() =>
        {
            Time.timeScale = 1;
            DataSuccession();
            instance.targetSceneNum = nextScene;
        });

        SceneLoadActionAdd(SceneEventClear);

        SceneBlackOut();
    }
    protected void DataSuccession()
    {
        instance.loadActionList = loadActionList;
        instance.stageData = stageData;
        instance.triggerNamedNoDelay = triggerNamedNoDelay;
        instance.saveDataIndex = saveDataIndex;
    }
    protected void SceneLoadActionAdd(Action action)
    {
        loadActionList.Add((sc, mode) => action());
        SceneManager.sceneLoaded += loadActionList[^1];
    }
    protected void SceneEventClear()
    {
        foreach (var obj in loadActionList)
        {
            SceneManager.sceneLoaded -= obj;
        }
        loadActionList.Clear();
    }
    protected void SceneBlackOut()
    {
        sceneFadeAnim.gameObject.SetActive(true);
        sceneFadeAnim.enabled = true;

        OnAnimationEnd(() =>
        {
            SceneManager.LoadSceneAsync(3);
            GameManager.manager.ReadytoSceneLoad();

            if (SceneManager.GetActiveScene().buildIndex != 0)
                ObjectUIPool.pool.ReadytoSceneLoad();
        });
    }
    protected virtual void SceneReveal()
    {
        sceneFadeAnim.gameObject.SetActive(true);
        sceneFadeAnim.enabled = true;
        sceneFadeAnim.SetTrigger(triggerNamedNoDelay);

        OnAnimationEnd(() =>
        {
            sceneFadeAnim.gameObject.SetActive(false);
            buildingConstructingReady = true;

            foreach (var item in constructingUIList)
            {
                item.StartConstruct();
            }
            constructingUIList.Clear();
            constructingUIList = null;
        });
    }

    protected async void OnAnimationEnd(Action action)
    {
        await Task.Delay(2000);
        action();
    }
    public int GetIndexScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    #endregion

    [ContextMenu("json만들기")]
    public void SDF()
    {
        SDF<StageDatas>();
    }
    #region 스테이지 데이터
    public int GetStageTeamCount(int stage)
    {
        return stageData.data[stage].participatingTeamCount;
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    #endregion
}
