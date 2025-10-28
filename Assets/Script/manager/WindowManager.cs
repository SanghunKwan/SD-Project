using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public Stack<tempMenuWindow> windowStack { get; private set; } = new Stack<tempMenuWindow>();
    private void Start()
    {
        GameManager.manager.windowManager = this;
    }
    public void CloseWindow()
    {
        FindOpenWindow(out tempMenuWindow window);
        window.OnOffWindow();
    }
    void FindOpenWindow(out tempMenuWindow window)
    {
        tempMenuWindow tempWindow = windowStack.Pop();
        while (!tempWindow.gameObject.activeSelf)
        {
            tempWindow = windowStack.Pop();
        }
        window = tempWindow;
    }
    public void CheckCloseWindow()
    {
        tempMenuWindow window;
        while (windowStack.TryPop(out window) && !CheckWindowActive(window))
        {
        }
        if (windowStack.Count <= 0)
            PlayerInputManager.manager.SetDefaultKeyMap();
    }
    bool CheckWindowActive(tempMenuWindow window)
    {
        if (window.gameObject.activeSelf)
        {
            windowStack.Push(window);
            return true;
        }
        return false;
    }

}
