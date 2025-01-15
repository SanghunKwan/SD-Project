using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuWindow : tempMenuWindow
{
    [SerializeField] SettingWindow setting;

    private void Awake()
    {
        setting.init();
    }
    public void CallWithChildWindow(MonoBehaviour window)
    {
        setting.OpenOneWindow(window);
        CallWindow(setting);
    }
    public void GotoMain()
    {
        StageManager.instance.CallLoadingScene(0);


    }
    public void Quit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
# else
        Application.Quit();

#endif
    }

}
