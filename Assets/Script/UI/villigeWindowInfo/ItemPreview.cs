using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.UI;

public class ItemPreview : UpgradePreview
{
    Image[] images = new Image[3];
    [SerializeField] AddressableManager addrMgr;

    float[] floats = new float[(int)AddressableManager.ItemQuality.MAX];
    public MouseOnImage interact { get; private set; }
    public Action<int>[] upgradeViewerUpdate { get; set; } = new Action<int>[3];


    public override void Awake()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i).GetChild(0).GetComponent<Image>();
        }

        floats[1] = 0.5f;
        for (int i = 2; i < floats.Length; i++)
        {
            floats[i] = 1;
        }
        interact = GetComponent<MouseOnImage>();
    }

    public void SetImage(TypeNum type, AddressableManager.ItemQuality itemQuality, AddressableManager.EquipsImage equipsImage)
    {
        string dicKey = type.ToString() + itemQuality.ToString();
        int ImageIndex = (int)equipsImage;

        addrMgr.GetData(dicKey, equipsImage, out Sprite sprite);
        images[ImageIndex].sprite = sprite;

        images[ImageIndex].color = Color.white * floats[(int)itemQuality];
    }

    public override void ActivePreview(Hero hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 3; i++)
        {
            SetImage(hero.Getnum, (AddressableManager.ItemQuality)hero.EquipsNum[i], (AddressableManager.EquipsImage)i);
            upgradeViewerUpdate[i]?.Invoke(hero.EquipsNum[i]);
        }
    }

    public override void ActivePreview(HeroData hero)
    {
        interact.SetHero(hero);
        for (int i = 0; i < 3; i++)
        {
            SetImage(hero.unitData.objectData.cur_status.type, (AddressableManager.ItemQuality)hero.equipNum[i], (AddressableManager.EquipsImage)i);
            upgradeViewerUpdate[i]?.Invoke(hero.equipNum[i]);
        }
    }
}
