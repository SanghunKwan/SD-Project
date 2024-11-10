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
    #region 미리불러오기
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
    #region 로드 버튼 클릭
    public void OnButtonClick(int index)
    {
        if (loadButtons[index].Set)
        {
            CreateSaveFile(index);
        }
        else
        {
            loadSaveManager.LoadData(index, out LoadSaveManager.SaveDataInfo info);

            //게임 시작
            StageManager.instance.FromMainMenu(1);

        }

        //loadButtons[index].Set

        //true일 때 새 게임 false 일 때 이어하기
    }
    #endregion
    #region 저장하기
    public void CreateSaveFile(int index)
    {
        loadSaveManager.CreateSaveFile(index);
        NeedFloorData(true, index);
        ButtonsSet(index, true);
    }
    #endregion
    #region 삭제하기
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
