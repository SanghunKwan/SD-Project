using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class StageManager : JsonLoad
{
    public static StageManager instance;

    public bool Newstage { get; set; } = false;
    public bool isDone { get; set; } = false;
    [SerializeField] GameObject[] Stage;
    [SerializeField] GameObject[] objects;
    [SerializeField] Animator sceneFadeAnim;
    int triggerNamedNoDelay;
    protected List<UnityAction<Scene, LoadSceneMode>> loadActionList;
    public int targetSceneNum;
    protected (int, int, List<UnityAction<Scene, LoadSceneMode>>, List<UnityAction<Scene>>) stageManagerData;


    [Serializable]
    public class StageData
    {
        public int nowFloor;
        public int participatingTeamCount;
        public string stageName;
        public string stagePurpose;
        public int[] stageRewardsImage;

        public StageData()
        {
            nowFloor = 0;
            participatingTeamCount = 5;
            stageName = "";
            stagePurpose = "";
            stageRewardsImage = new int[4] { 0, 1, 2, 3 };

        }
        public StageData(int floor)
        {
            nowFloor = floor;
            participatingTeamCount = 10;
            stageName = "asdf";
            stagePurpose = "fdsa";
            stageRewardsImage = new int[4] { 0, 1, 2, 3 };
        }
    }
    public StageDatas saveData { get; private set; }
    [Serializable]
    public class StageDatas
    {
        public StageData[] data;

        public StageDatas()
        {
            data = new StageData[100];
            for (int i = 0; i < data.Length; i++)
                data[i] = new StageData(i);
        }
    }


    protected void Awake()
    {
        VirtualAwake();
        instance = this;
    }
    protected virtual void VirtualAwake()
    {
        loadActionList = new(3);
        saveData = LoadData<StageDatas>("StageData");
        triggerNamedNoDelay = Animator.StringToHash("fadeOutNoDelay");
    }
    protected virtual void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject folder = new GameObject(objects[i].name);
            folder.transform.SetParent(transform);
            for (int j = 0; j < 5; j++)
            {
                Instantiate(objects[i], folder.transform);
            }
        }
        SceneReveal();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //새로운 스테이지 생성. 수정 필요.
        if (Newstage && !isDone)
        {
            isDone = true;
            GameObject newPlane = Instantiate(Stage[0], Vector3.zero, Quaternion.identity);
            newPlane.transform.GetChild(0).GetChild(0).localPosition -= new Vector3(50.2f, 0, 0);
        }
    }
    #region 씬
    public void CallLoadingScene(int targetScene)
    {
        SceneLoadActionAdd(() =>
        {
            instance.loadActionList = loadActionList;
            instance.saveData = saveData;
            instance.triggerNamedNoDelay = triggerNamedNoDelay;

            instance.targetSceneNum = targetScene;
        });

        SceneLoadActionAdd(SceneEventClear);

        SceneBlackOut();
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
        });
    }
    protected void SceneReveal()
    {
        sceneFadeAnim.gameObject.SetActive(true);
        sceneFadeAnim.enabled = true;
        sceneFadeAnim.SetTrigger(triggerNamedNoDelay);

        OnAnimationEnd(() => sceneFadeAnim.gameObject.SetActive(false));
    }

    async void OnAnimationEnd(Action action)
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
        return saveData.data[stage].participatingTeamCount;
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    #endregion
}
