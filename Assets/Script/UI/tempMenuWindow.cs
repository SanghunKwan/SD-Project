using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class tempMenuWindow : MonoBehaviour
{
    public virtual void OnOffWindow()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void CallWindow(MonoBehaviour otherWindow)
    {
        otherWindow.gameObject.SetActive(!otherWindow.gameObject.activeSelf);
    }
    private void OnEnable()
    {
        GameManager.manager.windowManager.windowStack.Push(this);

        PlayerInputManager.manager.SetKeyMap(PlayerInputManager.KeymapType.BuildingOrder);
    }
    protected void OnDisable()
    {
        GameManager.manager.windowManager.CheckCloseWindow();
    }

}
