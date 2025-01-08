using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementCharacter : MonoBehaviour
{
    [SerializeField] SettleNamePlate settleNamePlate;


    [SerializeField] GameObject IsHeroDeadObject;




    public void SetCharcterNameTag(HeroData heroData)
    {

        settleNamePlate.SetTexts(heroData.keycode, heroData.lv, heroData.name);
        gameObject.SetActive(true);

        IsHeroDeadObject.SetActive(heroData.isDead);
    }
    void SetQuirkData(QuirkData quirkData)
    {

    }

}
