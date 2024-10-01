using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerWindow : CamTuringWindow
{
    TowerComponent towerComponent;
    Image buildingIcon;
    TextMeshProUGUI buildingName;

    [SerializeField] AddressableManager addressableManager;
    [SerializeField] FloorManager floorManager;

    BuildSetCharacter[] heros = new BuildSetCharacter[5];

    [SerializeField] Button nextButton;
    int nowFloor;
    [Serializable]
    class FloorTag
    {
        [SerializeField] Image floorTagImage;
        [SerializeField] float moveTime;
        float originYposition;
        Text floorTagTextFirstNum;
        Text floorTagTextLastNum;
        IEnumerator ienMove;
        TowerWindow window;

        public void Init(TowerWindow caller)
        {
            window = caller;
            floorTagTextFirstNum = floorTagImage.transform.GetChild(0).GetComponent<Text>();
            floorTagTextLastNum = floorTagImage.transform.GetChild(1).GetComponent<Text>();
            originYposition = floorTagImage.transform.position.y;
        }
        public void SetNum(int firstNum, int lastNum)
        {
            floorTagTextFirstNum.text = firstNum.ToString();
            floorTagTextLastNum.text = lastNum.ToString();
        }
        public void SetActive(bool onoff)
        {
            floorTagImage.gameObject.SetActive(onoff);
        }
        public void Move(float yValue)
        {
            if (ienMove != null)
                window.StopCoroutine(ienMove);

            ienMove = MoveCour(yValue);
            window.StartCoroutine(ienMove);
        }
        public void PositionReset()
        {
            Vector3 vec = floorTagImage.rectTransform.position;

            vec.y = originYposition;

            floorTagImage.rectTransform.position = vec;
        }
        IEnumerator MoveCour(float yValue)
        {
            Vector3 vec = floorTagImage.rectTransform.position;
            vec.y = yValue;
            float time = 0;
            while (time < moveTime)
            {
                floorTagImage.rectTransform.position = Vector3.Lerp(floorTagImage.rectTransform.position, vec, 0.2f);
                time += Time.deltaTime;
                yield return null;
            }

            floorTagImage.rectTransform.position = vec;
        }
    }
    [SerializeField] FloorTag floortagImage;

    [Serializable]
    class NowFloorTag
    {
        [SerializeField] Image image;
        Animator anim;
        [SerializeField] Text floorText;
        int[] hash = new int[2];
        public enum AnimatorType
        {
            FadeIn,
            FadeOut
        }
        public void Init(int floorNum)
        {
            anim = image.GetComponent<Animator>();
            floorText.text = floorNum.ToString();
            hash[0] = Animator.StringToHash("fadeIn");
            hash[1] = Animator.StringToHash("fadeOutQuick");
        }
        public void AnimatorActivate(AnimatorType type)
        {
            anim.SetTrigger(hash[(int)type]);
        }
        public void Reset()
        {
            anim.ResetTrigger(hash[1]);
        }
        public void ImageMove(float yPosition)
        {
            Vector3 vec = image.transform.position;
            vec.y = yPosition;
            image.transform.position = vec;
        }
    }
    [SerializeField] NowFloorTag nowFloortag;
    [SerializeField] GameObject[] window1Object;
    [SerializeField] GameObject window2Object;
    [SerializeField] GameObject camMoveEndObject;
    public override void Init()
    {

        buildingIcon = transform.Find("BuildingName").GetChild(0).GetComponent<Image>();
        buildingName = buildingIcon.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
        floortagImage.Init(this);

        PrintFloorData();
        //for (int i = 0; i < heros.Length; i++)
        //    heros[i] = new BuildSetCharacter(transform.Find("HeroTeam").Find("MainTeam").GetChild(i));
    }

    public void SetOpen(bool onoff, AddressableManager.BuildingImage type)
    {
        gameObject.SetActive(onoff);

        if (onoff)
        {
            addressableManager.GetData("Building", type, out Sprite sprite);
            buildingIcon.sprite = sprite;
        }
        else
        {
            ExitReset();
        }
    }
    void ExitReset()
    {
        floortagImage.SetActive(false);
        floortagImage.PositionReset();
        towerComponent.AssembleClickReset();
        nowFloortag.Reset();
        nextButton.interactable = false;
    }
    public void PopUpWindow(int floorNum)
    {
        floortagImage.SetActive(true);

        floortagImage.Move(GameManager.manager.pointerEventData.position.y);
        floortagImage.SetNum((floorNum * 10) - 9, floorNum * 10);
        nowFloortag.AnimatorActivate(NowFloorTag.AnimatorType.FadeOut);
        nextButton.interactable = true;
        nowFloor = floorNum - 1;
    }

    public override void GetTurningComponent(ClickCamTurningComponent getClickComponent)
    {
        if (towerComponent != null)
            return;

        base.GetTurningComponent(getClickComponent);
        towerComponent = getClickComponent as TowerComponent;

        floorManager.GetData(out FloorManager.FloorData data);

        int tempNum = data.nowFloor - 1;
        int floorToIndex = tempNum / 10;
        float ratioInFloor = (tempNum % 10) / 10f;

        towerComponent.tickCamMove += () =>
        nowFloortag.ImageMove(towerComponent.GetCanvasYfromFloor(floorToIndex, data.floorLooks[floorToIndex], ratioInFloor));
    }
    void PrintFloorData()
    {
        floorManager.GetData(out FloorManager.FloorData data);

        nowFloortag.Init(data.nowFloor);
    }
    public void NextStep()
    {
        floorManager.GetData(out FloorManager.FloorData data);
        towerComponent.ChangeAngle(nowFloor, data.floorLooks[nowFloor], 10);
        Window1Active(false);
        towerComponent.SetAssembleCollierActive(false);
        towerComponent.windowEnd += () => camMoveEndObject.SetActive(true);
    }
    public void Exit()
    {
        towerComponent.ChangeAngle(40);
        Window1Active(true);
        camMoveEndObject.SetActive(false);
    }
    void Window1Active(bool onoff)
    {
        foreach (var obj in window1Object)
            obj.SetActive(onoff);
        window2Object.SetActive(!onoff);
    }
    public void BackStep()
    {
        towerComponent.CamBack();
        Window1Active(true);
        ExitReset();
        towerComponent.SetAssembleCollierActive(true);
        camMoveEndObject.SetActive(false);
    }
}
