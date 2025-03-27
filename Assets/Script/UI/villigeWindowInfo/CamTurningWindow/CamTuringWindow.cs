using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CamTuringWindow : tempMenuWindow, InitInterface
{
    public ClickCamTurningComponent clickCamturningComponent { get; set; }
    public static ClickCamTurningComponent ienumOwner { get; set; }
    [Header("²°´ÙÄ×´Ù °ü·Ã ¿ÀºêÁ§Æ®")]
    [SerializeField] CamTuringWindow[] otherCloseWindow;

    [SerializeField] GameObject[] UIObjectToggle;
    [SerializeField] GameObject[] UIObjectClose;

    public static GameObject transformObject { get; set; }

    [SerializeField] ClickDrag UIClickDragToggle;

    [Space(20)]
    [SerializeField] protected CharacterList characterList;
    public void ToggleWindow()
    {
        clickCamturningComponent.ToggleWindow();
    }
    public override void OnOffWindow()
    {
        ToggleWindow();
    }
    public void Collider_UIActive(bool onoff)
    {
        foreach (var item in UIObjectToggle)
        {
            item.SetActive(onoff);
        }
        foreach (var item in UIObjectClose)
        {
            item.SetActive(false);
        }
        UIClickDragToggle.enabled = onoff;

        
        characterList.CollidersSetInteractive(onoff);
    }
    public virtual void GetTurningComponent(ClickCamTurningComponent getClickComponent)
    {
        if (clickCamturningComponent != null && clickCamturningComponent != getClickComponent)
            clickCamturningComponent.SetColliderActive(true);
        clickCamturningComponent = getClickComponent;
    }
    public void CloseOtherWindow()
    {
        foreach (var item in otherCloseWindow)
        {
            if (item.gameObject.activeSelf)
            {
                item.clickCamturningComponent.ChangeWindow();
            }
        }
    }
    public abstract void Init();
}
