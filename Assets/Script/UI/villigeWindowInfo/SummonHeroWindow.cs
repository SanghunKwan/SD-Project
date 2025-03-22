using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonHeroWindow : CamTuringWindow
{
    Transform contentsTransform;
    [Header("SummonWindow 오브젝트")]
    [SerializeField] Transform heroExample;
    [Space]
    [SerializeField] SummonHeroNameTag summonHeroNameTag;
    [SerializeField] HandImage handImage;
    [SerializeField] WindowInfo windowInfo;

    public override void Init()
    {
        contentsTransform = transform.Find("NewHeroList").GetChild(0).GetChild(0).GetChild(0);
        CheckCanSummonHeroData();
    }
    public void SetOpen(bool onoff)
    {
        gameObject.SetActive(onoff);
    }
    public void SetHeroList(int maxHeroSummonCount, System.Action<int, SummonHeroNameTag> tagAction)
    {
        SummonHeroNameTag tempTag;
        for (int i = 0; i < maxHeroSummonCount; i++)
        {
            tempTag = Instantiate(summonHeroNameTag, contentsTransform);
            tempTag.Init(handImage, windowInfo);
            tagAction(i, tempTag);
        }
    }
    public Transform GetHeroExample(Unit.TypeNum type)
    {
        return heroExample.GetChild((int)type);
    }
    void CheckCanSummonHeroData()
    {
        SaveData.PlayInfo info = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;
        if (info.canSummonHero == null)
        {
            info.canSummonHero = new SaveData.HeroData[info.canSummonHeroCount];
            SetHeroList(info.canSummonHeroCount, (index, tag) =>
            {
                tag.SetNewHeroData(2);
                tag.SaveNameTag(index);
            });
            GameManager.manager.battleClearManager.OverrideSummonHeroList();
        }
        else
            SetHeroList(info.canSummonHero.Length, (index, tag) =>
            {
                tag.LoadHeroData(info.canSummonHero[index]);
            });
    }
}
