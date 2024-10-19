using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionExplain : InitInterface
{
    public Animator anim { get; private set; }
    TextMeshProUGUI mission_name;
    TextMeshProUGUI mission_purpose;
    [SerializeField] Sprite[] rewardImages;

    class ImageFrame
    {
        Image image;
        public ImageFrame(Transform rewardTransform, int index)
        {
            image = rewardTransform.GetChild(index).transform.GetChild(0).GetComponent<Image>();
        }
        public void SetImage(in Sprite sprite)
        {
            image.sprite = sprite;
        }
        public void SetActive(bool onoff)
        {
            image.transform.parent.gameObject.SetActive(onoff);
        }
    }
    ImageFrame[] frames = new ImageFrame[6];

    public enum RewardImageType
    {
        Gray,
        Black,
        White
    }

    public override void Init()
    {
        anim = GetComponent<Animator>();
        mission_name = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        mission_purpose = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < frames.Length; i++)
            frames[i] = new ImageFrame(transform.GetChild(1).GetChild(2), i);
    }

    #region 애니매이션 이벤트
    public void AnimationEnd()
    {
        gameObject.SetActive(false);
    }
    public void AnimationStart()
    {
        Fade(false);
    }
    public void FadeInEnd()
    {
        Fade(true);
    }
    void Fade(bool onoff)
    {
        int count = transform.GetChild(1).childCount;
        for (int i = 0; i < count; i++)
            transform.GetChild(1).GetChild(i).gameObject.SetActive(onoff);
    }
    #endregion
    #region text 변경
    public void ChangeExplain(int floorNum)
    {
        StageManager.StageData data = StageManager.instance.saveData.data[floorNum];

        mission_name.text = data.stageName;
        mission_purpose.text = data.stagePurpose;
        for (int i = 0; i < data.stageRewardsImage.Length; i++)
        {
            SetoffImage(i, true);
            SetImage(i, (RewardImageType)data.stageRewardsImage[i]);
        }
        for (int i = data.stageRewardsImage.Length; i < frames.Length; i++)
            SetoffImage(i, false);
    }
    void SetImage(int frameIndex, RewardImageType type)
    {
        frames[frameIndex].SetImage(rewardImages[(int)type]);
    }
    void SetoffImage(int frameIndex, bool onoff)
    {
        frames[frameIndex].SetActive(onoff);
    }

    #endregion
}
