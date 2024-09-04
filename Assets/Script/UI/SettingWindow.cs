using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : tempMenuWindow
{
    public tempMenuWindow[] childWindows { get; private set; }
    Button[] ChangeButtons;
    public System.Action setBack;
    public System.Action setApply;

    public enum ChildWindow
    {
        soundWindow,
        KeyWindow,
        DescriptionWindow,
        MAX
    }

    tempMenuWindow m_openWindow;

    public void init()
    {
        int maxCount = (int)ChildWindow.MAX;

        childWindows = new tempMenuWindow[maxCount];
        ChangeButtons = new Button[maxCount];
        for (int i = 0; i < maxCount; i++)
        {
            childWindows[i] = transform.GetChild(i).GetComponent<tempMenuWindow>();
            ChangeButtons[i] = transform.GetChild(maxCount).GetChild(0).GetChild(i).GetComponent<Button>();
        }
        setApply = () => { };
    }
    public void CloseAll()
    {
        m_openWindow.OnOffWindow();
        OnOffWindow();
    }
    public void OpenOneWindow(tempMenuWindow window)
    {
        m_openWindow = window;

        for (int i = 0; i < childWindows.Length; i++)
        {
            bool onoff = childWindows[i].Equals(window);
            childWindows[i].gameObject.SetActive(onoff);

            ChangeButtons[i].interactable = !onoff;

        }
    }
    public void SetBack()
    {
        setBack();
        setBack = null;

        CloseAll();
    }
    public void SaveandClose()
    {
        setApply();

        CloseAll();
    }
}
