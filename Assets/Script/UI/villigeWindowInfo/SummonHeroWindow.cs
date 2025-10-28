using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonHeroWindow : CamTuringWindow
{
    Transform contentsTransform;
    ScrollRect scrollRect;
    SaveData.PlayInfo playInfo;
    CharacterPopulation characterPopulation;
    [Header("SummonWindow 오브젝트")]
    [SerializeField] Transform heroExample;
    [Space]
    [SerializeField] SummonHeroNameTag summonHeroNameTag;
    [SerializeField] HandImage handImage;
    [Space]
    bool isDragtoSummon;
    [SerializeField] WindowInfo windowInfo;
    [SerializeField] VilligeUnitSpawner villigeUnitSpawner;

    public override void Init()
    {
        scrollRect = transform.Find("NewHeroList").GetChild(0).GetComponent<ScrollRect>();
        characterPopulation = characterList.GetComponent<CharacterPopulation>();
        contentsTransform = scrollRect.transform.GetChild(0).GetChild(0);

        if (GameManager.manager.battleClearManager != null)
        {
            playInfo = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;
            CheckCanSummonHeroData();
        }
        else
            GameManager.manager.onBattleClearManagerRegistered += () =>
            {
                playInfo = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;
                CheckCanSummonHeroData();
            };

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
            tempTag.Init(handImage, windowInfo, scrollRect);
            tagAction(i, tempTag);
        }
    }
    public Transform GetHeroExample(Unit.TypeNum type)
    {
        return heroExample.GetChild((int)type);
    }
    void CheckCanSummonHeroData()
    {
        if (playInfo.canSummonHero.Length == 0)
        {
            playInfo.canSummonHero = new SaveData.SummonHeroData[playInfo.canSummonHeroCount];
            SetHeroList(playInfo.canSummonHeroCount, (index, tag) =>
            {
                tag.SetNewHeroData(index, 2);
                tag.SaveNameTag(index);
            });
            GameManager.manager.battleClearManager.OverrideSummonHeroList();
        }
        else
            SetHeroList(playInfo.canSummonHero.Length, (index, tag) =>
            {
                tag.LoadHeroData(playInfo.canSummonHero[index]);
            });
    }

    public void DragToCharacterList(GameObject dragObject, float yPosition)
    {
        characterList.MoveBoard(dragObject, yPosition);
        isDragtoSummon = true;
    }
    public void NoMove(GameObject dragObject)
    {
        characterList.NoMove(dragObject, false);
        isDragtoSummon = false;
    }
    public void DragEnd(SummonHeroNameTag summonNameTag)
    {
        if (isDragtoSummon && characterPopulation.CanAddHero(1))
        {
            villigeInteract nameTag = villigeUnitSpawner.SummonHeroFromHeroData(summonNameTag.heroData);
            characterList.EndDrag(nameTag);
            HeroSpawnEvent(summonNameTag, nameTag.hero);
        }
        else
        {
            characterList.EndDrag();
            summonNameTag.SetView(true);
        }
    }
    public void ClickSpawn(SummonHeroNameTag summonNameTag)
    {
        if (!characterPopulation.CanAddHero(1))
            return;

        Unit.Hero hero = villigeUnitSpawner.SummonHeroFromHeroData(summonNameTag.heroData).hero;
        summonNameTag.SetView(false);
        characterList.ReArrage();
        HeroSpawnEvent(summonNameTag, hero);
    }
    void HeroSpawnEvent(SummonHeroNameTag summonNameTag, Unit.Hero hero)
    {
        SaveData.HeroData heroData = summonNameTag.heroData;
        SetTagInteractableFalse(summonNameTag);
        GameManager.manager.onVilligeHeroSummon.eventAction?.Invoke(
            (int)heroData.unitData.objectData.cur_status.type, heroData.unitData.objectData.position);
        characterPopulation.AddHero();

        HeroRallyPoint(hero);
    }
    void HeroRallyPoint(Unit.Hero hero)
    {
        hero.unitMove.Navi_Destination(clickCamturningComponent.transform.position + Vector3.back * 3);
    }
    void SetTagInteractableFalse(SummonHeroNameTag summonNameTag)
    {
        summonNameTag.SetInteractableFalse();
        playInfo.canSummonHero[summonNameTag.transform.GetSiblingIndex()].isSummoned = true;
    }


    public void OnClickExitButton()
    {
        bool[] bools = PlayerInputManager.manager.windowInputEnable;

        if (!(bools[0] && bools[1] && bools[2])) return;

        ToggleWindow();
    }
}
