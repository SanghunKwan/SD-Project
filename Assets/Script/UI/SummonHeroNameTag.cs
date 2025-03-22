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
        //��Ŭ�� �� ��� ����.


        //��Ŭ�� �� �̵�
        handImage.gameObject.SetActive(true);
        //handImage.SetText(hero);    
        //copyImage ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        //��Ŭ�� �� ��� ����.  
        //��Ŭ�� �� copyImage�̵�
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //��Ŭ�� �� ��ǥ �̵�
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //��Ŭ����
        infoWindow.gameObject.SetActive(true);
        infoWindow.VilligeWindowOpen(this);
        infoWindow.SetCam(true, summonHeroWindow.GetHeroExample(heroData.unitData.objectData.cur_status.type));


        //���� ������â ����.
        //��Ŭ����
        //"=" �δ�� �߰�.
    }

}
