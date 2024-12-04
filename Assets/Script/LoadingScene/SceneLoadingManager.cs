using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingManager : MonoBehaviour
{
    [SerializeField] Image backGround;
    [SerializeField] Camera tempCamera;
    LoadingManager loadingManager;
    [SerializeField] GameObject loadingObj;
    float turningAngle;

    private void Start()
    {
        loadingManager = StageManager.instance as LoadingManager;
        NewSceneLoad();
    }
    private void Update()
    {
        SetAnimation(loadingManager.GetLoadingStatus());
    }
    void SetAnimation(float result)
    {
        float angleLast = -result * 360;
        Debug.Log(result);
        if (turningAngle > angleLast)
        {
            turningAngle -= 30;
        }
        loadingObj.transform.localRotation = Quaternion.Euler(0, 0, turningAngle);
    }
    void NewSceneLoad()
    {
        loadingManager.LoadTargetScene(() => { });
    }

}
