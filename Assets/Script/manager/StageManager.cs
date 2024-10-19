using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unit;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{
    public static StageManager instance;


    public bool Newstage { get; set; } = false;
    public bool isDone { get; set; } = false;
    [SerializeField] GameObject[] Stage;
    [SerializeField] GameObject[] objects;



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


    private void Awake()
    {
        instance = this;

        string wnth = Path.Combine(Application.dataPath, "DataTable/StageData.json");

        saveData = (StageDatas)JsonUtility.FromJson(File.ReadAllText(wnth), typeof(StageDatas));
    }
    private void Start()
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
    void Update()
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
    public void SceneLoad(int SceneIndex)
    {
        DontDestroyOnLoad(this);
        DontdestoryList();
        SceneManager.LoadScene(SceneIndex);
    }
    void DontdestoryList()
    {
        DontDestroyOnLoad(Unit.Data.Instance);
        ObjectUIPool.pool.ReadytoSceneLoad();
        GameManager.manager.ReadytoSceneLoad();
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
    #endregion
    #region 스테이지 데이터
    [ContextMenu("json만들기")]
    public void SDF()
    {
        StageDatas asdf = new StageDatas();

        string wnth = Path.Combine(Application.dataPath, "DataTable/asdf.json");

        File.WriteAllText(wnth, JsonUtility.ToJson(asdf, true));

    }

    public int GetStageTeamCount(int stage)
    {
        return saveData.data[stage].participatingTeamCount;
    }

    #endregion
}
