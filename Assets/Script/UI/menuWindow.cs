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
    public void CallWithChildWindow(tempMenuWindow window)
    {
        setting.OpenOneWindow(window);
        CallWindow(setting);
    }
    public void GotoMain()
    {
        //���� �� Mainȭ������ �̵�
    }
    public void Quit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
# else

        //�����ϱ�
        Application.Quit();

#endif
    }

}
