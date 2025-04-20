using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuirkOrderButton : InitObject
{
    [SerializeField] NeedAddressable needAddressable;
    [SerializeField] Button button;

    public override void Init()
    {
        needAddressable.Init();
    }

    public void SetButtonPosition(Vector3 vec)
    {
        gameObject.SetActive(true);
        button.image.rectTransform.position = vec;
    }
    public void SetButtonAction(System.Action action)
    {
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() => action());
    }
    private void OnDisable()
    {
        ButtonInActive();
    }
    public void ButtonInActive()
    {
        button.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
