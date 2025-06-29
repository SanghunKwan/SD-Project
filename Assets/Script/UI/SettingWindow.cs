using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : tempMenuWindow
{
    MonoBehaviour[] childWindows;
    Button[] ChangeButtons;

    public System.Action<SaveData.PlaySetting> LoadSaveDataActions { get; set; }

    [SerializeField] CheckUICall[] windowMoveUI;
    [SerializeField] CheckUICall[] windowQuitUI;
    public bool isChanged { get; set; }
    Dictionary<bool, System.Action<MonoBehaviour>> isChangedAction = new Dictionary<bool, System.Action<MonoBehaviour>>();
    Dictionary<bool, System.Action<int>> isChangedQuitAction = new Dictionary<bool, System.Action<int>>();
    HashSet<ChildWindow> changedWindows = new HashSet<ChildWindow>((int)ChildWindow.MAX);

    public enum ChildWindow
    {
        soundWindow,
        KeyWindow,
        DescriptionWindow,
        ViewWindow,
        MAX
    }

    MonoBehaviour m_openWindow;

    public void init()
    {
        int maxCount = (int)ChildWindow.MAX;

        childWindows = new MonoBehaviour[maxCount];
        ChangeButtons = new Button[maxCount];
        for (int i = 0; i < maxCount; i++)
        {
            childWindows[i] = transform.GetChild(i).GetComponent<MonoBehaviour>();
            ChangeButtons[i] = transform.GetChild(maxCount).GetChild(0).GetChild(i).GetComponent<Button>();
        }
        SetActions();
    }
    void SetActions()
    {
        isChangedAction.Add(false, OpenOneWindow);
        isChangedAction.Add(true, OpenCheckUI);

        isChangedQuitAction.Add(false, (num) => CloseAll());
        isChangedQuitAction.Add(true, OpenCheckUI);
    }
    public void LoadSettingData(SaveData.PlaySetting setting)
    {
        LoadSaveDataActions(setting);
    }

    public void CloseAll()
    {
        m_openWindow.gameObject.SetActive(!m_openWindow.gameObject.activeSelf);
        base.OnOffWindow();
        changedWindows.Clear();
    }
    public void OpenOneWindow(MonoBehaviour window)
    {
        m_openWindow = window;
        isChanged = false;

        for (int i = 0; i < childWindows.Length; i++)
        {
            bool onoff = childWindows[i].Equals(window);
            childWindows[i].gameObject.SetActive(onoff);

            ChangeButtons[i].interactable = !onoff;
            if (onoff)
            {
                changedWindows.Add((ChildWindow)i);
            }
        }
    }
    public void SetBack()
    {
        //수정 전으로 값 되돌리기.
        foreach (var window in changedWindows)
        {
            ((IWindowSet)childWindows[(int)window]).RevertValue();
        }
    }
    public void SaveWindow()
    {
        //수정 값 저장하기.
        foreach (var window in changedWindows)
        {
            ((IWindowSet)childWindows[(int)window]).SaveValue();
        }

        GameManager.manager.settingLoader.SaveSetting();
    }
    public override void OnOffWindow()
    {
        GameManager.manager.windowManager.windowStack.Push(this);
        ClickWindowQuitButton(0);

    }
    public void ClickWindowButton(MonoBehaviour window)
    {
        isChangedAction[isChanged](window);
    }
    void OpenCheckUI(MonoBehaviour window)
    {
        windowMoveUI[window.transform.GetSiblingIndex()].CallUIOnce(false);
    }
    public void ClickWindowQuitButton(int i)
    {
        isChangedQuitAction[isChanged](i);
    }
    void OpenCheckUI(int i)
    {
        windowQuitUI[i].CallUIOnce(false);
    }

}
