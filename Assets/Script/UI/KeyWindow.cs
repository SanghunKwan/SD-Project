using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class KeyWindow : InitObject
{
    [SerializeField] BlackScreen blackScreen;

    Action<string> textChange;
    SettingWindow settingWindow;

    public Action changekeyRemove { get; set; }
    public Action setBack { get; set; }
    public Action changeSave { get; set; }
    public Action keyInit { get; set; }

    Action<InputControl> inputSave;
    Action<string> eventTextChanged = (str) => { };
    public bool isResistered { get; private set; } = false;

    [SerializeField] InitObject[] keyButtonInputs;

    private void Awake()
    {
        settingWindow = transform.parent.GetComponent<SettingWindow>();
        changeSave += () => isResistered = false;
        setBack += () => isResistered = false;

    }
    public void ScreenEffectStart(Action cancel, Action<InputControl> save, Action<string> strChange)
    {
        textChange = strChange;
        inputSave = save;
        blackScreen.GetActionClick(() =>
        {
            PlayerInputManager.manager.SetKeyMap(PlayerInputManager.KeymapType.BuildingOrder);
            cancel();
        });

        PlayerInputManager.manager.SetKeyMap(PlayerInputManager.KeymapType.NewKeyInput);
    }

    public void ChangeKey(InputControl control)
    {
        PlayerInputManager.manager.ChangeDefaultKey(PlayerInputManager.KeymapType.PlayerOwn);

        blackScreen.Escape();

        textChange(control.displayName);
        inputSave(control);

    }

    public Action<string> GetKeyInfo(KeyButtonInput.BindingOrderActionNameInit[] byTransform,
                           KeyButtonInput.BindingOrderNumActionName[] byInt)
    {
        return (Keyboard) =>
        {
            PlayerInputManager.manager.SetDefaultKeyMap();
            for (int i = 0; i < byTransform.Length; i++)
            {
                PlayerInputManager.manager.KeyActionChange(byTransform[i].bindingOrder, byTransform[i].actionName.ToString(), Keyboard);
            }

            for (int i = 0; i < byInt.Length; i++)
            {
                PlayerInputManager.manager.KeyActionChange(byInt[i].bindingOrder, byInt[i].actionName.ToString(), Keyboard);
            }
            PlayerInputManager.manager.SetKeyMap(PlayerInputManager.KeymapType.BuildingOrder);
        };
    }
    public event Action<string> OnKeyTextChanged
    {
        add
        {
            eventTextChanged += value;
        }
        remove
        {
            eventTextChanged -= value;
        }
    }
    public void SomethingInput(in string str)
    {
        settingWindow.isChanged = true;
        eventTextChanged.Invoke(str);
    }
    private void OnEnable()
    {
        if (isResistered)
            return;
        settingWindow.setBack += setBack;
        settingWindow.beforeApply += changekeyRemove;
        settingWindow.setApply += changeSave;
        keyInit();
        isResistered = true;
    }

    public override void Init()
    {
        foreach (var item in keyButtonInputs)
        {
            item.Init();
        }
    }
}
