using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager manager;

    System.Action<bool>[] mouseToggleActions;
    InputAction[] inputActions;

    public PlayerInput input { get; private set; }


    #region enums
    public enum KeymapType
    {
        PlayerInput,
        PlayerOwn,
        NewKeyInput,
        BuildingOrder
    }
    public enum KeyMapActionFlags
    {
        None = 0,
        ArmySelect = 1 << 0,
        ArmyAdd = 1 << 1,
        ArmySet = 1 << 2,
        DoubleArmySelect = 1 << 3,
        FormationSelect = 1 << 4,
        TimeDelay = 1 << 5,
        SimpleAct = 1 << 6,
        ActionAdd = 1 << 7,
        Escape = 1 << 8,

        Max = 9,
        All = (1 << Max) - 1 //모든 키입력 활성화 플래그
    }

    public enum MouseInputEnableFlags
    {
        None = 0,
        FieldLeftClick = 1 << 0, //0:좌클릭
        FieldRightClick = 1 << 1, //1:우클릭
        FieldMiddleClick = 1 << 2, //2:중앙클릭
        InventoryLeftClick = 1 << 3, //3:인벤 좌클릭
        InventoryRightClick = 1 << 4, //4:인벤 우클릭
        InventoryMiddleClick = 1 << 5, //5:인벤 중앙클릭
        MinimapClick = 1 << 6, //6:미니맵 클릭
        ScreenMove = 1 << 7, //7:화면 이동

        Max = 8,
        All = (1 << Max) - 1 //모든 마우스 입력 활성화 플래그
    }
    #endregion enums

    public bool[] fieldInputEnable { get; private set; } //0:좌클릭, 1:우클릭, 2:중앙클릭
    public bool[] inventoryInputEnable { get; private set; } //0:좌클릭, 1:우클릭, 2:중앙클릭

    public bool minimapInputEnable { get; private set; }
    public bool screenMoveInputEnable { get; private set; }

    public bool IsDefaultInput => input.defaultActionMap == KeymapType.PlayerInput.ToString();
    InputActionMap EditableMap => input.actions.FindActionMap(KeymapType.PlayerOwn.ToString());



    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        manager = this;

        //초기값은 모두 활성화
        fieldInputEnable = new bool[3] { true, true, true };
        inventoryInputEnable = new bool[3] { true, true, true };

        minimapInputEnable = true;
        screenMoveInputEnable = true;

        SetActions();
        SetCurrentMapInputActions();
    }
    void SetActions()
    {
        mouseToggleActions = new System.Action<bool>[(int)MouseInputEnableFlags.Max];
        mouseToggleActions[0] = (isOn)
            => SetFieldMouseInputEnable(PointerEventData.InputButton.Left, isOn);
        mouseToggleActions[1] = (isOn)
            => SetFieldMouseInputEnable(PointerEventData.InputButton.Right, isOn);
        mouseToggleActions[2] = (isOn)
            => SetFieldMouseInputEnable(PointerEventData.InputButton.Middle, isOn);
        mouseToggleActions[3] = (isOn)
            => SetInventoryInputEnable(PointerEventData.InputButton.Left, isOn);
        mouseToggleActions[4] = (isOn)
            => SetInventoryInputEnable(PointerEventData.InputButton.Right, isOn);
        mouseToggleActions[5] = (isOn)
            => SetInventoryInputEnable(PointerEventData.InputButton.Middle, isOn);
        mouseToggleActions[6] = (isOn) => SetMinimapClickEnable(isOn);
        mouseToggleActions[7] = (isOn) => SetScreenMoveEnable(isOn);
    }
    void SetCurrentMapInputActions()
    {
        inputActions = new InputAction[(int)KeyMapActionFlags.Max];

        InputActionMap map = input.currentActionMap;

        int length = inputActions.Length;
        for (int i = 0; i < length; i++)
        {
            inputActions[i] = map.FindAction(((KeyMapActionFlags)(1 << i)).ToString());
        }
    }

    #region 키보드 관련
    #region 키맵 관련
    public void SetKeyMap(KeymapType keymap)
    {
        input.SwitchCurrentActionMap(keymap.ToString());
    }
    public void SetDefaultKeyMap()
    {
        input.SwitchCurrentActionMap(input.defaultActionMap);
        SetCurrentMapInputActions();
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
    #endregion 키맵 관련


    #region 다른 키 비활성화
    public void SetKeyMapEnable(KeyMapActionFlags flags)
    {
        int length = (int)KeyMapActionFlags.Max;
        KeyMapActionFlags compareFlag;
        for (int i = 0; i < length; i++)
        {
            compareFlag = (KeyMapActionFlags)(1 << i);
            SetMapActionUnsafe(inputActions[i], (flags & compareFlag) != KeyMapActionFlags.None);
        }
    }
    void SetMapActionUnsafe(InputAction map, bool isOn)
    {
        if (isOn)
            map.Enable();
        else
            map.Disable();
    }
    #endregion 다른 키 비활성화
    #endregion 키보드 관련


    #region 마우스 이벤트 관련 관리
    public void SetFieldMouseInputEnable(PointerEventData.InputButton button, bool isOn)
    {
        fieldInputEnable[(int)button] = isOn;
    }
    public void SetInventoryInputEnable(PointerEventData.InputButton button, bool isOn)
    {
        inventoryInputEnable[(int)button] = isOn;
    }

    public void SetScreenMoveEnable(bool isOn)
    {
        screenMoveInputEnable = isOn;
    }

    public void SetMinimapClickEnable(bool isOn)
    {
        minimapInputEnable = isOn;
    }

    public void SetMouseInputEnable(MouseInputEnableFlags flags)
    {
        Debug.Log((int)flags);
        int length = (int)MouseInputEnableFlags.Max;
        for (int i = 0; i < length; i++)
        {
            mouseToggleActions[i]((((int)flags >> i) & 1) == 1);
        }
    }

    #endregion 마우스 이벤트 관련 관리
}