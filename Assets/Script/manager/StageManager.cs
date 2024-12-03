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
    [SerializeField] Animator backGround;
    protected List<UnityAction<Scene, LoadSceneMode>> loadActionList;
    protected List<UnityAction<Scene>> unloadActionList;
    public int targetSceneNum;


    public enum OBJECTNUM
    {
        BONEWALL,
    }
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


    protected virtual void Awake()
    {
        if (instance != null)
        {
            loadActionList = instance.loadActionList;
            unloadActionList = instance.unloadActionList;
            saveData = instance.saveData;
        }
        else
        {
            loadActionList = new(3);
            unloadActionList = new(3);
            saveData = LoadData<StageDatas>("StageData");
        }
        instance = this;
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
        foreach (var obj in loadActionList)
        {
            SceneManager.sceneLoaded -= obj;
        }
        loadActionList.Clear();

        SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
        GameManager.manager.ReadytoSceneLoad();
        targetSceneNum = targetScene;
    }
    public CObject CallObject(OBJECTNUM num, Transform trans)
    {
        CObject obj = transform.GetChild((int)num).GetChild(0).GetComponent<CObject>();
        obj.gameObject.SetActive(true);
        obj.transform.position = trans.position;
        obj.transform.SetParent(transform);

        return obj;
    }
    public int GetIndexScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public void BackGroundFadeOut()
    {
        backGround.enabled = true;
        backGround.SetTrigger("fadeOutNoDelay");
        BackGroundSetActiveFalse();
    }
    async void BackGroundSetActiveFalse()
    {
        await Task.Delay(1000);
        backGround.gameObject.SetActive(false);
    }
    #endregion
    #region 스테이지 데이터
    [ContextMenu("json만들기")]
    public void SDF()
    {
        SDF<StageDatas>();
    }

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
