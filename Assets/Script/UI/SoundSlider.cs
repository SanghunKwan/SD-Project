using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : InitObject
{
    public Slider slider { get; private set; }
    InputField inputField;
    SoundWindow soundWindow;
    [SerializeField] SoundWindow.SoundType type;
    float BaseValue;


    public override void Init()
    {
        slider = GetComponent<Slider>();
        inputField = transform.GetChild(4).GetComponent<InputField>();
        soundWindow = transform.parent.GetComponent<SoundWindow>();

        ComponentInit();
    }

    void ComponentInit()
    {
        slider.onValueChanged.AddListener((fNum) => SliderValueChanged(fNum));

        inputField.onSubmit.AddListener((str) => InputSubmit(str));
        inputField.onEndEdit.AddListener((str) => SynchronizeInputField());

        BaseValue = slider.value;
        SynchronizeInputField();
    }
    public void InputValue(float value)
    {
        slider.value = value;
    }

    private void SliderValueChanged(float fNum)
    {
        int nNum = (int)fNum;
        inputField.text = nNum.ToString();
        soundWindow.ValueChanged(type, nNum);
    }
    void SynchronizeInputField()
    {
        inputField.text = slider.value.ToString();
    }
    public void InputSubmit(string str)
    {
        InputValue(float.Parse(str));
    }
    public void NoSaveRevertValue()
    {
        slider.value = BaseValue;
        SynchronizeInputField();
    }
    public void SaveValue(SaveData.PlaySetting setting)
    {
        BaseValue = slider.value;
        setting.soundWindowSet[(int)type] = BaseValue;
    }

}
