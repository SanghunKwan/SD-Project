using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager manager;

    public PlayerInput input;
    public enum KeymapType
    {
        PlayerInput,
        PlayerOwn,
        NewKeyInput,
        BuildingOrder
    }
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        manager = this;
    }

    public void SetKeyMap(KeymapType keymap)
    {
        input.SwitchCurrentActionMap(keymap.ToString());
    }
    public void SetDefaultKeyMap()
    {
        input.SwitchCurrentActionMap(input.defaultActionMap);
    }
    public void ChangeDefaultKey(KeymapType map)
    {
        input.defaultActionMap = map.ToString();
    }
    public void KeyActionChange(int index, in string actionName, in string keyboardName)
    {
        input.currentActionMap[actionName].ApplyBindingOverride(index, keyboardName);
    }
}