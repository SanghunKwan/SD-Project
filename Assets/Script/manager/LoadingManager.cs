using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadingManager : StageManager
{
    AsyncOperation loading;
    protected override void Awake()
    {
        if(instance != null)
        targetSceneNum = instance.targetSceneNum;
        base.Awake();
    }
    protected override void Start()
    {
    }
    // Update is called once per frame
    protected override void Update()
    {
    }
    #region ¾À
    public void SceneLoad(int SceneIndex, Action action)
    {
        foreach (var obj in loadActionList)
        {
            SceneManager.sceneLoaded -= obj;
        }
        loadActionList.Clear();

        loadActionList.Add((sc, mode) => action());
        SceneManager.sceneLoaded += loadActionList[^1];
        loading = SceneManager.LoadSceneAsync(SceneIndex);
    }
    public void LoadTargetScene(Action action)
    {
        SceneLoad(targetSceneNum, action);
    }
    public float GetLoadingStatus()
    {
        if (loading != null)
            return loading.progress;
        else
            return 0.0f;
    }
    #endregion

}
