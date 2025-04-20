using SaveData;
using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class QuirkPreview : UpgradePreview
{
    [SerializeField] QuirkChange[] quirkChange;
    QuirkButtonController controller;


    public override void Awake()
    {
        foreach (var item in quirkChange)
        {
            item.Awake();
        }
        controller = GetComponent<QuirkButtonController>();
    }
    public override void ActivePreview(Hero hero)
    {
        controller.SetHero(hero);
        controller.ResetAction = () =>
        {
            quirkChange[0].SetQuirk(hero.quirks);
            quirkChange[1].SetQuirk(hero.disease);
        };
        controller.ResetAction();
    }

    public override void ActivePreview(HeroData hero)
    {
        Debug.Log("Data 업그레이드 불가.");
        throw new System.NotImplementedException();
    }


}
