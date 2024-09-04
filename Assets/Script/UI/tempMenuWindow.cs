using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class tempMenuWindow : MonoBehaviour
{
    public void OnOffWindow()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void CallWindow(tempMenuWindow otherWindow)
    {
        otherWindow.OnOffWindow();
    }


}
