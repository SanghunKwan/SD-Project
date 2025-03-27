using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandImage : MonoBehaviour
{
    [SerializeField] Image weapon_image;
    [SerializeField] Image work_image;
    [SerializeField] Image tag_image;
    public Image image { get { return tag_image; } }
    [SerializeField] TextMeshProUGUI lv_Text;
    [SerializeField] TextMeshProUGUI name_Text;

    public void SetText(Unit.Hero hero)
    {
        lv_Text.text = hero.lv.ToString();
        name_Text.text = hero.name;

        SetImage(hero);
    }

    void SetImage(Unit.Hero hero)
    {
        AddressableManager.manager.GetData(GetWeaponLabelName(hero), AddressableManager.EquipsImage.Weapon, out Sprite weaponSprite);
        weapon_image.sprite = weaponSprite;

        work_image.enabled = hero.VilligeAction == ActionAlert.ActionType.buildingWork;
        if (work_image.enabled)
        {
            AddressableManager.manager.GetData(AddressableManager.LabelName.Building, hero.BuildingAction, out Sprite buildingSprite);
            work_image.sprite = buildingSprite;
        }
        else
            work_image.sprite = null;
    }
    string GetWeaponLabelName(Unit.Hero hero)
    {
        return hero.Getnum.ToString() + ((AddressableManager.ItemQuality)hero.EquipsNum[0]).ToString();
    }


    public void SetNameTag(SaveData.HeroData heroData)
    {
        lv_Text.text = heroData.lv.ToString();
        name_Text.text = heroData.name;

        SetImage(heroData);
    }
    void SetImage(SaveData.HeroData heroData)
    {
        AddressableManager.manager.GetData(GetWeaponLabelName(heroData), AddressableManager.EquipsImage.Weapon, out Sprite weaponSprite);
        weapon_image.sprite = weaponSprite;

        work_image.enabled = heroData.villigeAction == 3;
        if (work_image.enabled)
        {
            AddressableManager.manager.GetData(AddressableManager.LabelName.Building, (AddressableManager.BuildingImage)heroData.workBuilding, out Sprite buildingSprite);
            work_image.sprite = buildingSprite;
        }
        else
            work_image.sprite = null;
    }
    string GetWeaponLabelName(SaveData.HeroData heroData)
    {
        return heroData.unitData.objectData.cur_status.type.ToString() + ((AddressableManager.ItemQuality)heroData.equipNum[0]).ToString();
    }
}