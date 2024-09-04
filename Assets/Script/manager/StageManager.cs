using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{
    public static StageManager instance;


    public bool Newstage = false;
    public bool isDone = false;
    [SerializeField] GameObject[] Stage;
    [SerializeField] GameObject[] objects;



    public enum OBJECTNUM
    {
        BONEWALL,
    }


    private void Awake()
    {
        instance = this;
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
            Debug.Log("asdf");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Newstage && !isDone)
        {
            isDone = true;
            GameObject newPlane = Instantiate(Stage[0], Vector3.zero, Quaternion.identity);
            newPlane.transform.GetChild(0).GetChild(0).localPosition -= new Vector3(50.2f, 0, 0);
        }
    }

    public void SceneLoad()
    {
        DontDestroyOnLoad(this);
        DontdestoryList();
        SceneManager.LoadScene(1);
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
}
