using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class KeyButtonInput : InitObject
{
    Button button;
    TextMeshProUGUI text;
    [SerializeField] KeyWindow keyWindow;

    [Serializable]
    public class BindingOrderActionNameInit
    {
        public ActionNameType actionName;
        public int multiply;
        public int bindingOrder { get; private set; }
        public int add = 2;

        public void init(in KeyButtonInput input)
        {
            bindingOrder = input.transform.GetSiblingIndex() * multiply + add;
        }
    }
    [Serializable]
    public class BindingOrderNumActionName
    {
        public ActionNameType actionName;
        public int bindingOrder;
    }
    [SerializeField] BindingOrderActionNameInit[] bindingOrderActionNameInits;
    [SerializeField] BindingOrderNumActionName[] bindingOrderNumActionNames;
    public enum ActionNameType
    {
        ArmySelect,
        ArmyAdd,
        ArmySet,
        DoubleArmySelect,
        FormationSelect,
        TimeDelay,
        SimpleAct,
        ActionAdd
    }
    enum KeyType
    {
        TeamCode,
        ASDVOrder,
        Ctrl,
        Shift,
        SpaceTimeStop,
        QWERFormation,
        Max
    }
    [SerializeField] KeyType keyType;
    Action<string, string>[] keyChangeActions = new Action<string, string>[(int)KeyType.Max];
    Func<string, string>[] keyRemoveActions = new Func<string, string>[(int)KeyType.Max];
    Action keyRemove = () => { };
    Action save = () => { };
    string cancelStr;
    string tempStr;

    public override void Init()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        keyWindow.keyInit += Initialize;
    }
    private void Awake()
    {
        button.onClick.AddListener(SetKeyReady);

        BindingOrderSet();
        SetActions();

        keyWindow.setBack += noSave;
        keyWindow.changekeyRemove += KeyRemove;
        keyWindow.changeSave += Save;

    }
    void BindingOrderSet()
    {
        for (int i = 0; i < bindingOrderActionNameInits.Length; i++)
        {
            bindingOrderActionNameInits[i].init(this);
        }
    }
    void SetActions()
    {
        keyChangeActions[0] = (value, str2) => { GameManager.manager.ConverterChange(tempStr, value); };
        keyChangeActions[1] = (value, str2) => { GameManager.manager.ConverterChange(tempStr, value); };
        keyChangeActions[2] = (value, str2) => { };
        keyChangeActions[3] = (value, str2) => { GameManager.manager.ChangeShift(str2); };
        keyChangeActions[4] = (value, str2) => { };
        keyChangeActions[5] = (value, str2) => { GameManager.manager.ConverterChange(tempStr, value); };

        keyRemoveActions[0] = (str) => { GameManager.manager.KeyConverterKeyDelete(str, out string value); return value; };
        keyRemoveActions[1] = (str) => { GameManager.manager.KeyConverterKeyDelete(str, out string value); return value; };
        keyRemoveActions[2] = (str) => { return string.Empty; };
        keyRemoveActions[3] = (str) => { return string.Empty; };
        keyRemoveActions[4] = (str) => { return string.Empty; };
        keyRemoveActions[5] = (str) => { GameManager.manager.KeyConverterKeyDelete(str, out string value); return value; };

        keyWindow.OnKeyTextChanged += TextKeyWindowChange;
    }


    void SetKeyOnoff(bool onoff)
    {
        button.interactable = onoff;
        text.gameObject.SetActive(onoff);

    }

    void SetKeyReady()
    {
        SetKeyOnoff(false);
        keyWindow.ScreenEffectStart(SetKeyCancel, SetSave, (str) => text.text = str);
    }
    public void SetKeyCancel()
    {
        SetKeyOnoff(true);
    }
    void SetKeyComplete(in string value, in string controlName, in string newdisplay)
    {
        keyChangeActions[(int)keyType](value, controlName);
    }
    void SetKeyDelete(out string value)
    {
        value = keyRemoveActions[(int)keyType](cancelStr);
    }
    private void noSave()
    {
        text.text = cancelStr;
        tempStr = cancelStr;
        save = () => { };
        keyRemove = () => { };
    }
    void Initialize()
    {
        cancelStr = text.text;
        tempStr = cancelStr;

        if (cancelStr.StartsWith("empty"))
        {
            text.text = " ";
        }
    }
    void Save()
    {
        save();
        save = () => { };
    }
    void KeyRemove()
    {
        keyRemove();
        keyRemove = () => { };
    }
    public void SetSave(InputControl control)
    {
        keyWindow.SomethingInput(control.displayName);
        tempStr = control.displayName;
        text.text = tempStr;

        string value = string.Empty;
        keyRemove = () => { SetKeyDelete(out value); };
        save = () => { SaveChange()(control, value); };
    }
    Action<InputControl, string> SaveChange()
    {
        return (control, value) =>
            {
                SetKeyComplete(value, control.name, control.displayName);
                keyWindow.GetKeyInfo(bindingOrderActionNameInits, bindingOrderNumActionNames)
                                                    ("<Keyboard>/" + control.displayName.ToLower());
                cancelStr = control.displayName;
            };
    }

    void TextKeyWindowChange(string str)
    {
        if (tempStr == str)
        {
            tempStr = GameManager.manager.GetEmptyKey();
            text.text = " ";

            string value = string.Empty;

            keyRemove = () => SetKeyDelete(out value);
            save = () => SaveChangeBlank()(tempStr, value);
        }
    }
    Action<string, string> SaveChangeBlank()
    {
        return (str, value) =>
        {
            SetKeyComplete(value, "할당실패", "할당실패");
            cancelStr = str;
            text.text = " ";
        };
    }
    private void OnDisable()
    {
        text.text = cancelStr;
    }

}
