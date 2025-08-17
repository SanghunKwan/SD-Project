using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SummonHeroNameTag : HandImage, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    SummonHeroWindow summonHeroWindow;
    HandImage handImage;
    WindowInfo infoWindow;
    ScrollRect scrollRect;
    public SaveData.HeroData heroData { get; private set; }
    static Vector2 offset;
    RectTransform rectTransform;

    public void Init(HandImage getHandImage, WindowInfo windowInfo, ScrollRect getScrollRect)
    {
        summonHeroWindow = transform.parent.parent.parent.parent.parent.GetComponent<SummonHeroWindow>();
        rectTransform = GetComponent<RectTransform>();
        offset = new Vector2();

        handImage = getHandImage;
        infoWindow = windowInfo;
        scrollRect = getScrollRect;
    }
    public void LoadHeroData(SaveData.SummonHeroData getHeroData)
    {
        heroData = getHeroData.heroData;

        if (getHeroData.isSummoned)
        {
            SetView(false);
            SetInteractableFalse();
        }
    }
    public void SetNewHeroData(int index, int newQuirkCount)
    {
        int quirkCount = Random.Range(1, newQuirkCount + 1);
        int typeAddNum = Random.Range(0, 2);
        if (GameManager.manager.battleClearManager.SaveDataInfo.day == 1)
            typeAddNum = index;
        //ù���� ������ Ȱ�� ������ ����.

        DefaultNameManager.mananger.GetRandomName(out string name);
        heroData = new SaveData.HeroData(name, 1, Unit.Data.Instance.statusList[301 + typeAddNum],
                                          new SaveData.QuirkSaveData(quirkCount, 5),
                                          new SaveData.QuirkDefaultData(quirkCount - 1, 4, QuirkData.manager.diseaseInfo));
        heroData.unitData.objectData.cur_status.curHP = heroData.unitData.objectData.cur_status.HP;
    }
    public void SaveNameTag(int index)
    {
        SaveData.PlayInfo info = GameManager.manager.battleClearManager.SaveDataInfo.playInfo;
        info.canSummonHero[index] = new SaveData.SummonHeroData(heroData);
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
        scrollRect.OnBeginDrag(eventData);
        //��Ŭ�� �� ��� ����.
        if (eventData.button == PointerEventData.InputButton.Left
            || !PlayerInputManager.manager.windowInputEnable[(int)eventData.button])
            return;

        //��Ŭ�� �� �̵�
        offset.x = eventData.pressPosition.x - rectTransform.position.x;
        offset.y = (eventData.pressPosition.y - rectTransform.position.y)
                                / rectTransform.sizeDelta.y * image.rectTransform.sizeDelta.y;

        handImage.gameObject.SetActive(true);
        handImage.SetNameTag(heroData);

        SetView(false);
        summonHeroWindow.NoMove(gameObject);
        GameManager.manager.onVilligeSummonInteract.eventAction?.Invoke((int)heroData.unitData.objectData.cur_status.type, Vector3.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.OnDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left
            || !PlayerInputManager.manager.windowInputEnable[(int)eventData.button])
            return;

        GameObject dragObject = eventData.pointerCurrentRaycast.gameObject;

        //��Ŭ�� �� copyImage�̵�
        handImage.image.rectTransform.position = eventData.position - offset;

        if (dragObject.CompareTag("NameTag"))
            summonHeroWindow.DragToCharacterList(dragObject, eventData.position.y);
        else
            summonHeroWindow.NoMove(gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left
            || !PlayerInputManager.manager.windowInputEnable[(int)eventData.button])
            return;

        //��Ŭ�� �� ��ǥ �̵�
        handImage.gameObject.SetActive(false);
        summonHeroWindow.DragEnd(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging || !PlayerInputManager.manager.windowInputEnable[(int)eventData.button])
            return;

        GameManager.manager.onVilligeSummonInteract.eventAction?.Invoke((int)heroData.unitData.objectData.cur_status.type, Vector3.zero);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //��Ŭ���� ���� ������â ����.
            infoWindow.gameObject.SetActive(true);
            infoWindow.VilligeWindowOpen(this);
            infoWindow.SetCam(true, summonHeroWindow.GetHeroExample(heroData.unitData.objectData.cur_status.type));
        }
        else
            //��Ŭ����
            //"=" �δ�� �߰�.
            summonHeroWindow.ClickSpawn(this);
    }
    public void SetView(bool onoff)
    {
        image.color = Color.white * System.Convert.ToInt32(onoff);
        transform.GetChild(0).gameObject.SetActive(onoff);
    }
    public void SetInteractableFalse()
    {
        image.raycastTarget = false;
    }
}
