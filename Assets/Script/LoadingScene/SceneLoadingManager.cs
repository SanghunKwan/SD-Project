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
    [SerializeField] GameObject loadingObj;

    private void Start()
    {
        loadingManager = StageManager.instance as LoadingManager;
        WaitforSeconds(2000, NewSceneLoad);
    }
    private void FixedUpdate()
    {
        SetAnimation(loadingManager.GetLoadingStatus());
    }
    void SetAnimation(float result)
    {
        Debug.Log(result);
        loadingObj.transform.localRotation = Quaternion.Euler(0, 0, -360 * result);
    }
    async void WaitforSeconds(int milliseconds, Action action)
    {
        await Task.Delay(milliseconds);
        action();
    }
    void NewSceneLoad()
    {
        loadingManager.LoadTargetScene(() => { });
    }

}
