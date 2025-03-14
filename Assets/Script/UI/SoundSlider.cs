using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : InitObject
{
    public Slider slider { get; private set; }
    InputField inputField;
    SoundWindow soundWindow;
    [SerializeField] SoundWindow.SoundType type;
    float BaseValue;

    void SliderSetting()
    {
        slider.onValueChanged.AddListener((fNum) => SliderValueChanged(fNum));
    }
    void InputFieldSetting()
    {
        inputField.onSubmit.AddListener((str) => InputSubmit(str));
        inputField.onEndEdit.AddListener((str) => inputField.text = slider.value.ToString());
    }


    void InitInWindow()
    {
        ((Text)inputField.placeholder).text = slider.value.ToString();
        BaseValue = slider.value;
    }

    private void SliderValueChanged(float fNum)
    {
        int nNum = (int)fNum;
        inputField.text = nNum.ToString();
        soundWindow.ValueChanged(type, nNum);
    }
    public void InputSubmit(string str)
    {
        slider.value = int.Parse(str);
    }
    void NoSave()
    {
        slider.value = BaseValue;
    }

    public override void Init()
    {
        slider = GetComponent<Slider>();
        inputField = transform.GetChild(4).GetComponent<InputField>();
        soundWindow = transform.parent.GetComponent<SoundWindow>();
        SliderSetting();
        InputFieldSetting();
        soundWindow.noSave += NoSave;
        soundWindow.Sliderinit += InitInWindow;
    }
}
