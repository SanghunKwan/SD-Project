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

    [SerializeField] BindingOrderActionNameInit[] bindingOrderActionNameInits;
    [SerializeField] BindingOrderNumActionName[] bindingOrderNumActionNames;

    [SerializeField] KeyType keyType;

    static Action<GameManager, string>[] gameManagerKeyDeleteActions;
    static Action<GameManager, string, string>[] gameManagerKeyAddActions;

    public int buttonIndex { get; private set; }

    public string originalStr { get; private set; }
    string dictionaryValue;

    public bool IsChange2Original => dictionaryValue == text.text;

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

    public void Init(int index)
    {
        buttonIndex = index;
        Init();
    }
    public override void Init()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        button.onClick.AddListener(SetKeyReady);

        BindingOrderSet();
        SetActions();

        originalStr = dictionaryValue = text.text;
        keyWindow.EventTextChanged += CheckSameText2Blank;
    }
    private void OnEnable()
    {
        originalStr = text.text;
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
        if (gameManagerKeyDeleteActions != null) return;

        gameManagerKeyDeleteActions = new Action<GameManager, string>[(int)KeyType.Max];
        gameManagerKeyAddActions = new Action<GameManager, string, string>[(int)KeyType.Max];

        gameManagerKeyDeleteActions[0] = (gameManager, originalStr) => { gameManager.ConverterKeyDelete(originalStr); };
        gameManagerKeyDeleteActions[1] = (gameManager, originalStr) => { gameManager.ConverterKeyDelete(originalStr); };
        gameManagerKeyDeleteActions[2] = (gameManager, originalStr) => { };
        gameManagerKeyDeleteActions[3] = (gameManager, originalStr) => { };
        gameManagerKeyDeleteActions[4] = (gameManager, originalStr) => { };
        gameManagerKeyDeleteActions[5] = (gameManager, originalStr) => { gameManager.ConverterKeyDelete(originalStr); };

        gameManagerKeyAddActions[0] = (gameManager, text, dictionaryValue) => { gameManager.ConverterKeyAdd(text, dictionaryValue); };
        gameManagerKeyAddActions[1] = (gameManager, text, dictionaryValue) => { gameManager.ConverterKeyAdd(text, dictionaryValue); };
        gameManagerKeyAddActions[2] = (gameManager, text, dictionaryValue) => { gameManager.ChangeModifier(text.ToLower(), GameManager.ModifiersNum.Ctrl); };
        gameManagerKeyAddActions[3] = (gameManager, text, dictionaryValue) => { gameManager.ChangeModifier(text.ToLower(), GameManager.ModifiersNum.Shift); };
        gameManagerKeyAddActions[4] = (gameManager, text, dictionaryValue) => { gameManager.ChangeModifier(text.ToLower(), GameManager.ModifiersNum.Space); };
        gameManagerKeyAddActions[5] = (gameManager, text, dictionaryValue) => { gameManager.ConverterKeyAdd(text, dictionaryValue); };

    }

    void SetKeyReady()
    {
        SetKeyOnoff(false);
        keyWindow.ScreenEffectStart(SetKeyCancel, SetSave);
    }
    void SetKeyOnoff(bool onoff)
    {
        button.interactable = onoff;
        text.gameObject.SetActive(onoff);

    }
    void SetKeyCancel()
    {
        SetKeyOnoff(true);
    }
    public void SetSave(InputControl control)
    {
        keyWindow.SomethingInput(control.displayName);
        keyWindow.AddChangeKeys(this);

        text.text = control.displayName;
    }
    public void SetSave(in string controlDisplayName)
    {
        keyWindow.SomethingInput(controlDisplayName);
        keyWindow.AddChangeKeys(this);

        text.text = controlDisplayName;
    }
    void CheckSameText2Blank(string str)
    {
        if (text.text != str) return;

        text.text = " ";
        keyWindow.AddChangeKeys(this);
    }

    public void RevertValue()
    {
        text.text = originalStr;
    }
    public void DeleteOriginalKey()
    {
        //기존 키값 삭제.
        gameManagerKeyDeleteActions[(int)keyType](GameManager.manager, originalStr);
    }
    public void OverrideValue()
    {
        //dictionary 새로운 key 값 저장.
        gameManagerKeyAddActions[(int)keyType](GameManager.manager, text.text, dictionaryValue);

        keyWindow.GetKeyInfo(bindingOrderActionNameInits, bindingOrderNumActionNames)
                                                    ("<Keyboard>/" + text.text.ToLower());
        originalStr = text.text;
    }
}
