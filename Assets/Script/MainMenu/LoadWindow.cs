using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadWindow : MonoBehaviour
{
    [SerializeField] LoadSaveManager loadSaveManager;
    [SerializeField] ButtonInteractableHighlighted[] loadButtons;
    [SerializeField] Button[] trashButtons;
    [SerializeField] Text[] texts;
    [SerializeField] string emptytext;
    [SerializeField] SaveName saveText;
    [Serializable]
    class SaveName
    {
        public string floorText;
        public string dayText;
        public string blank;
    }
    #region �̸��ҷ�����
    private void Start()
    {
        bool tempValid;
        for (int i = 0; i < loadButtons.Length; i++)
        {
            tempValid = loadSaveManager.IsValidData(i);
            ButtonsSet(i, tempValid);
            NeedFloorData(tempValid, i);
        }
    }
    void ButtonsSet(int index, bool onoff)
    {
        trashButtons[index].image.raycastTarget = onoff;
        loadButtons[index].Set = !onoff;
    }

    void NeedFloorData(bool isValid, int index)
    {
        if (!isValid)
            return;

        loadSaveManager.DataNowFloor(index, out int floor, out int day);
        texts[index].text = SaveDataInfo(floor, day);

    }
    string SaveDataInfo(int floor, int day)
    {
        return saveText.floorText + floor.ToString() + saveText.blank + day.ToString() + saveText.dayText;
    }
    #endregion
    #region �ε� ��ư Ŭ��
    public void OnButtonClick(int index)
    {
        if (loadButtons[index].Set)
        {
            CreateSaveFile(index);
        }
        else
        {
            loadSaveManager.LoadData(index, out LoadSaveManager.SaveDataInfo info);

            //���� ����
            StageManager.instance.FromMainMenu(1);

        }

        //loadButtons[index].Set

        //true�� �� �� ���� false �� �� �̾��ϱ�
    }
    #endregion
    #region �����ϱ�
    public void CreateSaveFile(int index)
    {
        loadSaveManager.CreateSaveFile(index);
        NeedFloorData(true, index);
        ButtonsSet(index, true);
    }
    #endregion
    #region �����ϱ�
    public void DeleteSaveFile(int index)
    {
        loadSaveManager.DeleteSaveFile(index);
        ButtonsSet(index, false);
        ResetSlot(index);
    }
    void ResetSlot(int index)
    {
        texts[index].text = emptytext;
    }
    #endregion
}
