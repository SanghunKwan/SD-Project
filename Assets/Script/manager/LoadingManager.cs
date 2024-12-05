using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class LoadingManager : StageChildManager
{
    AsyncOperation loading;
    protected override void SceneReveal()
    {
    }
    // Update is called once per frame
    #region ¾À
    public void SceneLoad(int SceneIndex, Action action)
    {
        SceneLoadActionAdd(action);
        SceneLoadActionAdd(DataSuccession);
        SceneLoadActionAdd(SceneEventClear);

        loading = SceneManager.LoadSceneAsync(SceneIndex);
        loading.allowSceneActivation = false;

        WaitforSeconds(500);
    }

    async void WaitforSeconds(int milisecond)
    {
        await Task.Delay(milisecond);
        loading.allowSceneActivation = true;
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
