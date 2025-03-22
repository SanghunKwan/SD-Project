using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unit;
using System;

public class SkillPreview : MonoBehaviour
{
    Image[] images = new Image[4];
    [SerializeField] AddressableManager addrMgr;
    public MouseOnImage interact { get; private set; }

    public Action<int>[] upgradeViewerUpdate { get; set; } = new Action<int>[4];

    public void Awake()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i).GetComponent<Image>();
        }
        interact = GetComponent<MouseOnImage>();
    }

    public void ActivateSkillPreview(Hero hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 4; i++)
        {
            SetImage(hero.Getnum, (AddressableManager.PreviewImage)i);
            upgradeViewerUpdate[i]?.Invoke(hero.SkillsNum[i]);
        }
        Debug.Log("specialMove 이미지 수정 예정");
    }
    public void SetImage(TypeNum type, AddressableManager.PreviewImage previewImage)
    {
        addrMgr.GetData(type.ToString() + "Skill", previewImage, out Sprite sprite);
        images[(int)previewImage].sprite = sprite;
    }
    public void ActivateSkillPreview(SaveData.HeroData hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 4; i++)
        {
            SetImage(hero.unitData.objectData.cur_status.type, (AddressableManager.PreviewImage)i);
            upgradeViewerUpdate[i]?.Invoke(hero.skillNum[i]);
        }
        Debug.Log("specialMove 이미지 수정 예정");
    }
}
