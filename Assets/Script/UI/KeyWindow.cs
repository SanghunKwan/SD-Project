using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class KeyWindow : InitObject, IWindowSet
{
    [SerializeField] BlackScreen blackScreen;

    SettingWindow settingWindow;

    Action<InputControl> inputSave;
    public Action<string> EventTextChanged { get; set; }
    public bool isResistered { get; private set; } = false;

    [SerializeField] InitObject[] keyButtonInputs;

    PlayerInputManager manager;

    HashSet<KeyButtonInput> changedKeys;
    HashSet<KeyButtonInput> changedButtons;


    public override void Init()
    {
        manager = PlayerInputManager.manager;
        settingWindow = transform.parent.GetComponent<SettingWindow>();

        changedKeys = new HashSet<KeyButtonInput>(10);
        for (int i = 0; i < keyButtonInputs.Length; i++)
        {
            ((KeyButtonInput)keyButtonInputs[i]).Init(i);
        }

        settingWindow.LoadSaveDataActions += LoadKeys;
    }
    void LoadKeys(SaveData.PlaySetting playInfo)
    {
        if (playInfo.keyWindowSet == null) return;

        changedButtons = new HashSet<KeyButtonInput>(playInfo.keyWindowSet.Length);
        KeyButtonInput tempObj;
        foreach (var item in playInfo.keyWindowSet)
        {
            tempObj = (KeyButtonInput)keyButtonInputs[item.index];
            tempObj.SetSave(item.keyboardName);
            changedButtons.Add(tempObj);
        }
        LoadComplete();
    }


    public void ScreenEffectStart(Action cancel, Action<InputControl> save)
    {
        inputSave = save;
        blackScreen.GetActionClick(() =>
        {
            manager.SetKeyMap(PlayerInputManager.KeymapType.BuildingOrder);
            cancel();
        });

        manager.SetKeyMap(PlayerInputManager.KeymapType.NewKeyInput);
    }

    public void ChangeKey(InputControl control)
    {
        if (manager.IsDefaultInput)
        {
            manager.ResetOwnMap();
            manager.ChangeDefaultKey(PlayerInputManager.KeymapType.PlayerOwn);
        }
        manager.SetDefaultKeyMap();

        blackScreen.Escape();

        inputSave(control);

    }

    public Action<string> GetKeyInfo(KeyButtonInput.BindingOrderActionNameInit[] byTransform,
                           KeyButtonInput.BindingOrderNumActionName[] byInt)
    {
        return (Keyboard) =>
        {
            manager.SetDefaultKeyMap();
            for (int i = 0; i < byTransform.Length; i++)
            {
                manager.KeyActionChange(byTransform[i].bindingOrder, byTransform[i].actionName.ToString(), Keyboard);
            }

            for (int i = 0; i < byInt.Length; i++)
            {
                manager.KeyActionChange(byInt[i].bindingOrder, byInt[i].actionName.ToString(), Keyboard);
            }
            manager.SetKeyMap(PlayerInputManager.KeymapType.PlayerInput);
        };
    }
    public void SomethingInput(in string str)
    {
        settingWindow.isChanged = true;
        EventTextChanged?.Invoke(str);
    }
    public void AddChangeKeys(KeyButtonInput keybuttonInput) => changedKeys.Add(keybuttonInput);


    void ApplyChanged()
    {
        foreach (var item in changedKeys)
        {
            item.DeleteOriginalKey();
        }
        foreach (var item in changedKeys)
        {
            item.OverrideValue();
        }
    }
    void LoadComplete()
    {
        ApplyChanged();
        TranslateKeys();
    }
    public void SaveValue()
    {
        //바뀐 키들을 순회하며 매니저에 저장.
        //playerInputManager 에 덮어씌우기.
        ApplyChanged();
        TranslateKeys();
        SaveKeys();
    }
    public void SaveKeys()
    {
        SaveData.PlaySetting setting = GameManager.manager.settingLoader.PlaySetting;
        setting.keyWindowSet = new SaveData.KeySet[changedButtons.Count];

        int index = 0;
        foreach (var item in changedButtons)
        {
            setting.keyWindowSet[index] = new SaveData.KeySet(item);
            index++;
        }
    }
    void TranslateKeys()
    {
        foreach (var item in changedKeys)
        {
            changedButtons.Add(item);
        }

        changedKeys.Clear();
    }
    public void RevertValue()
    {
        foreach (var item in changedKeys)
        {
            item.RevertValue();
        }
        changedKeys.Clear();
        //바뀐 키들을 순회하며 원래 키로 되돌림.
    }
}
