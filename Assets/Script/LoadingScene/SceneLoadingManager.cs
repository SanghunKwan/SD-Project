using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingManager : MonoBehaviour
{
    [SerializeField] Image backGround;
    [SerializeField] Camera tempCamera;
    LoadingManager loadingManager;

    private void Start()
    {
        loadingManager = StageManager.instance as LoadingManager;
        WaitforSeconds(1500, () => loadingManager.LastSceneUnload(DelayLastSceneUnloaded));
    }
    async void WaitforSeconds(int milliseconds, Action action)
    {
        await Task.Delay(milliseconds);
        action();
    }
    void DelayLastSceneUnloaded()
    {
        TempObjectSetActive(true);
        WaitforSeconds(1500, () => loadingManager.LoadTargetScene(LoadingSceneFadeOut));
    }
    void TempObjectSetActive(bool onoff)
    {
        tempCamera.gameObject.SetActive(onoff);
        backGround.transform.GetChild(0).gameObject.SetActive(onoff);
    }
    void LoadingSceneFadeOut()
    {
        TempObjectSetActive(false);
        backGround.gameObject.SetActive(false);
        WaitforSeconds(1500, () => loadingManager.LastSceneUnload(StageManager.instance.BackGroundFadeOut));
    }



}
