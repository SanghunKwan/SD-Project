using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SummonHeroNameTag : HandImage, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    SummonHeroWindow summonHeroWindow;
    HandImage handImage;
    WindowInfo infoWindow;
    public SaveData.HeroData heroData { get; private set; }
    public void Init(HandImage getHandImage, WindowInfo windowInfo)
    {
        summonHeroWindow = transform.parent.parent.parent.parent.parent.GetComponent<SummonHeroWindow>();
        handImage = getHandImage;
        infoWindow = windowInfo;
    }
    public void LoadHeroData(SaveData.HeroData getHeroData)
    {
        heroData = getHeroData;
    }
    public void SetNewHeroData(int newQuirkCount)
    {
        int quirkCount = Random.Range(1, newQuirkCount + 1);
        DefaultNameManager.mananger.GetRandomName(out string name);
        heroData = new SaveData.HeroData(name, 1, Unit.Data.Instance.statusList[301 + Random.Range(0, 2)],
                                          new SaveData.QuirkSaveData(quirkCount, 5),
                                          new SaveData.QuirkDefaultData(quirkCount - 1, 4, QuirkData.manager.diseaseInfo));
    }
    public void SaveNameTag(int index)
    {
        SaveData.PlayInfo info = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;
        info.canSummonHero[index] = heroData;
    }
    private void Start()
    {
        CheckNameTag();
    }
    public void CheckNameTag()
    {
        SetNameTag(heroData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //좌클릭 시 기능 없음.


        //우클릭 시 이동
        handImage.gameObject.SetActive(true);
        //handImage.SetText(hero);    
        //copyImage 생성
    }

    public void OnDrag(PointerEventData eventData)
    {
        //좌클릭 시 기능 없음.  
        //우클릭 시 copyImage이동
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //우클릭 시 좌표 이동
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //좌클릭시
        infoWindow.gameObject.SetActive(true);
        infoWindow.VilligeWindowOpen(this);
        infoWindow.SetCam(true, summonHeroWindow.GetHeroExample(heroData.unitData.objectData.cur_status.type));


        //영웅 데이터창 오픈.
        //우클릭시
        //"=" 부대로 추가.
    }

}
