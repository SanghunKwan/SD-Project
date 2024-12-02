using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class LoadingManager : StageManager
{
    protected override void Awake()
    {
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
        GameManager.manager.ReadytoSceneLoad();
        SceneManager.LoadSceneAsync(SceneIndex, LoadSceneMode.Additive);
    }
    public void LastSceneUnload(Action eventAction)
    {
        foreach (var obj in unloadActionList)
        {
            SceneManager.sceneUnloaded -= obj;
        }
        unloadActionList.Clear();

        unloadActionList.Add((sc) => eventAction());
        SceneManager.sceneUnloaded += unloadActionList[^1];
        SceneManager.UnloadSceneAsync(GetIndexScene());
    }
    public void LoadTargetScene(Action action)
    {
        SceneLoad(targetSceneNum, action);
    }
    #endregion

}
