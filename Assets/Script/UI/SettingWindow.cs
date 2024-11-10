using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : tempMenuWindow
{
    MonoBehaviour[] childWindows;
    Button[] ChangeButtons;
    public System.Action setBack { get; set; }
    public System.Action beforeApply { get; set; } = () => { };
    public System.Action setApply { get; set; }

    [SerializeField] CheckUICall[] windowMoveUI;
    [SerializeField] CheckUICall[] windowQuitUI;
    public bool isChanged { get; set; }
    Dictionary<bool, System.Action<MonoBehaviour>> isChangedAction = new Dictionary<bool, System.Action<MonoBehaviour>>();
    Dictionary<bool, System.Action<int>> isChangedQuitAction = new Dictionary<bool, System.Action<int>>();

    public enum ChildWindow
    {
        soundWindow,
        KeyWindow,
        DescriptionWindow,
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
        setApply = () => { };

        isChangedAction.Add(false, OpenOneWindow);
        isChangedAction.Add(true, OpenCheckUI);

        isChangedQuitAction.Add(false, (num) => CloseAll());
        isChangedQuitAction.Add(true, OpenCheckUI);
    }
    public void CloseAll()
    {
        m_openWindow.gameObject.SetActive(!m_openWindow.gameObject.activeSelf);
        base.OnOffWindow();
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
        }
    }
    public void SetBack()
    {
        setBack();
        setBack = () => { };
    }
    public void SaveandClose()
    {
        beforeApply();
        Debug.Log("before End");
        setApply();
        setApply = () => { };
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
        windowMoveUI[window.transform.GetSiblingIndex()].CallUIOnce();
    }
    public void ClickWindowQuitButton(int i)
    {
        isChangedQuitAction[isChanged](i);
    }
    void OpenCheckUI(int i)
    {
        windowQuitUI[i].CallUIOnce();
    }

}
