using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unit;
using System;
using SaveData;

public class SkillPreview : UpgradePreview
{
    Image[] images = new Image[4];
    [SerializeField] AddressableManager addrMgr;
    public MouseOnImage interact { get; private set; }

    public Action<int>[] upgradeViewerUpdate { get; set; } = new Action<int>[4];

    public override void Awake()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i).GetComponent<Image>();
        }
        interact = GetComponent<MouseOnImage>();
    }

    public void SetImage(TypeNum type, AddressableManager.PreviewImage previewImage)
    {
        addrMgr.GetData(type.ToString() + "Skill", previewImage, out Sprite sprite);
        images[(int)previewImage].sprite = sprite;
    }

    public override void ActivePreview(Hero hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 4; i++)
        {
            SetImage(hero.Getnum, (AddressableManager.PreviewImage)i);
            upgradeViewerUpdate[i]?.Invoke(hero.SkillsNum[i]);
        }
        Debug.Log("specialMove �̹��� ���� ����");
    }

    public override void ActivePreview(HeroData hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 4; i++)
        {
            SetImage(hero.unitData.objectData.cur_status.type, (AddressableManager.PreviewImage)i);
            upgradeViewerUpdate[i]?.Invoke(hero.skillNum[i]);
        }
        Debug.Log("specialMove �̹��� ���� ����");
    }
}
