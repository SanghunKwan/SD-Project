using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class KeyButtonInput : MonoBehaviour
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
    Action<string, string>[] actions = new Action<string, string>[(int)KeyType.Max];
    Action save = () => { };
    string cancelStr;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(SetKeyReady);

        BindingOrderSet();
        SetActions();

        keyWindow.setBack += noSave;
        keyWindow.changeSave += Save;
        keyWindow.keyInit += Init;
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
        actions[0] = (str, str2) => { GameManager.manager.ConverterChange(cancelStr, str); };
        actions[1] = (str, str2) => { GameManager.manager.ConverterChange(cancelStr, str); };
        actions[2] = (str, str2) => { };
        actions[3] = (str, str2) => { GameManager.manager.ChangeShift(str2); };
        actions[4] = (str, str2) => { };
        actions[5] = (str, str2) => { GameManager.manager.ConverterChange(cancelStr, str); };
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
    void SetKeyComplete(string newdisplay, string name)
    {
        actions[(int)keyType](newdisplay, name);
    }
    private void noSave()
    {
        text.text = cancelStr;
        save = () => { };
    }
    void Init()
    {
        cancelStr = text.text;
    }
    void Save()
    {
        save();
        save = () => { };
    }
    public void SetSave(InputControl control)
    {
        Action<InputControl> action = (control) =>
        {
            SetKeyComplete(control.displayName, control.name);
            keyWindow.GetKeyInfo(bindingOrderActionNameInits, bindingOrderNumActionNames)("<Keyboard>/" + control.displayName.ToLower());
        };

        save = () => action(control);
    }
}
