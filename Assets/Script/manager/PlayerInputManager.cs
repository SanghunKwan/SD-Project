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
    public bool IsDefaultInput => input.defaultActionMap == KeymapType.PlayerInput.ToString();
    InputActionMap EditableMap => input.actions.FindActionMap(KeymapType.PlayerOwn.ToString());

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
        EditableMap[actionName].ApplyBindingOverride(index, keyboardName);
    }
    public void ResetOwnMap()
    {
        foreach (var action in input.actions.FindActionMap(KeymapType.PlayerInput.ToString()))
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                EditableMap[action.name].ApplyBindingOverride(i, action.bindings[i]);
            }
        }
    }


}