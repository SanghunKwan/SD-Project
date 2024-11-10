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
        //저장 후 Main화면으로 이동
    }
    public void Quit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
# else

        //저장하기
        Application.Quit();

#endif
    }

}
