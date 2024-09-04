using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class KeyWindow : tempMenuWindow
{
    [SerializeField] PlayerInput input;
    [SerializeField] BlackScreen blackScreen;
    [SerializeField] CharacterList characterList;

    Action<string> textChange;
    SettingWindow settingWindow;

    public Action setBack;
    public Action changeSave;
    public Action keyInit;

    Action<InputControl> inputSave;
    public bool isResistered { get; private set; } = false;

    enum KeymapType
    {
        PlayerInput,
        PlayerOwn,
        NewKeyInput
    }

    private void Awake()
    {
        settingWindow = transform.parent.GetComponent<SettingWindow>();
        changeSave += () => isResistered = false;
        setBack += () => isResistered = false;

    }
    private void SetKeyMap(KeymapType keymap)
    {
        input.SwitchCurrentActionMap(keymap.ToString());
    }

    void SetDefaultKeyMap()
    {
        input.SwitchCurrentActionMap(input.defaultActionMap);
    }

    public void ScreenEffectStart(Action cancel, Action<InputControl> save, Action<string> strChange)
    {
        textChange = strChange;
        inputSave = save;
        blackScreen.GetActionClick(() =>
        {
            SetDefaultKeyMap();
            cancel();
        });
        SetKeyMap(KeymapType.NewKeyInput);
    }

    public void ChangeKey(InputControl control)
    {
        input.defaultActionMap = KeymapType.PlayerOwn.ToString();

        blackScreen.Escape();

        textChange(control.displayName);
        inputSave(control);

    }
    public Action<string> GetKeyInfo(KeyButtonInput.BindingOrderActionNameInit[] byTransform,
                           KeyButtonInput.BindingOrderNumActionName[] byInt)
    {
        return (Keyboard) =>
        {
            for (int i = 0; i < byTransform.Length; i++)
            {
                input.currentActionMap[byTransform[i].actionName.ToString()].ApplyBindingOverride(byTransform[i].bindingOrder, Keyboard);
            }

            for (int i = 0; i < byInt.Length; i++)
            {
                input.currentActionMap[byInt[i].actionName.ToString()].ApplyBindingOverride(byInt[i].bindingOrder, Keyboard);
            }
        };
    }
    private void OnEnable()
    {
        if (isResistered)
            return;
        settingWindow.setBack += setBack;
        settingWindow.setApply += changeSave;
        keyInit();
        isResistered = true;
    }
}
