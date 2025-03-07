using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class villigeInteract : villigeBase, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Action<PointerEventData>[] clicks = new Action<PointerEventData>[2];
    public Action dragEndEvent { get; set; } = () => { };

    [SerializeField] Hero[] Heros;
    public Hero hero { get; private set; }

    Animator anim;
    int animOnMouse;
    int workIndex;

    WindowInfo infoWindow;

    [SerializeField] HandImage onHand;
    Image copyHand;
    Vector2 offset;
    CharacterList characterList;

    TextMeshProUGUI textPro;

    BuildingComponent workingBuilding;

    public static bool isDrag { get; private set; }
    public static villigeInteract now_villigeInteract { get; private set; }


    public NowTeamUI teamUIData { get; private set; } = new NowTeamUI();
    Image jobImage;

    #region temp
    public void HeroIniit(int index)
    {
        hero = Instantiate(Heros[index], Vector3.zero, Quaternion.identity, PlayerNavi.nav.transform);
        hero.MakeQuirk();
    }
    #endregion
    public void MatchHero(Hero getHero)
    {
        hero = getHero;
        hero.TeamChange(hero.keycode);
        CheckText();
    }

    public void ChangeTeamKey(in string newCode)
    {
        PlayerNavi.nav.SetTeam(hero.unitMove, newCode);
        hero.TeamChange(newCode);
        CheckText();

    }
    public void CheckText()
    {
        textPro.text = HeroInfoText();
        if (workingBuilding != null && workingBuilding == characterList.buildingSetWindow.buildingComponent)
            characterList.buildingSetWindow.BuildSetCharactersCheck(workIndex, this);
    }
    public string HeroInfoText()
    {
        return HeroInfoText(hero.keycode, hero.lv, hero.name);
    }
    public void SetText(SaveData.HeroData heroData)
    {
        if (textPro == null)
            textPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        textPro.text = HeroInfoText(heroData.keycode, heroData.lv, heroData.name);
    }
    string HeroInfoText(in string keycode, int lvText, in string nameText)
    {
        return "<mark=#00000055><size=60>" + keycode + " </size></mark> " + lvText + " " + nameText;
    }
    protected override void Start()
    {
        base.Start();

        if (textPro == null)
            textPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        animOnMouse = Animator.StringToHash("onMouse");
        jobImage = transform.Find("Image").GetComponent<Image>();

        infoWindow = transform.parent.parent.parent.parent.parent.parent.Find("CharacterInfo").gameObject.GetComponent<WindowInfo>();
        characterList = transform.parent.parent.parent.parent.parent.GetComponent<CharacterList>();


        clicks[(int)PointerEventData.InputButton.Left] = (eventdata) =>
        {
            bool off = infoWindow.gameObject.activeSelf && infoWindow.wStatus.targetObject == hero;

            infoWindow.gameObject.SetActive(!off);
            infoWindow.VilligeWindowOpen(hero);
            infoWindow.SetCam(!off, hero);

        };
        clicks[(int)PointerEventData.InputButton.Right] = (eventdata) =>
        {
            PlayerNavi.nav.HeroClear();

            GameManager.manager.Unselect(GameManager.manager.dicPlayerCharacter);
            GameManager.manager.Unselect(GameManager.manager.dicObjects);

            GameManager.manager.ScreenToPoint(hero.transform.position);

            hero.Selected(true);
        };

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool(animOnMouse, true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool(animOnMouse, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!eventData.dragging && hero != null)
            clicks[(int)eventData.button](eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragOffset(eventData, image);
    }
    public void DragEvent(PointerEventData eventData)
    {
        copyHand = CreateHandImage();
        image.color = Color.clear;
        transform.GetChild(0).gameObject.SetActive(false);
        //글자 복사 추가.
        NoMove();
        isDrag = true;
        now_villigeInteract = this;
    }
    Image CreateHandImage()
    {
        HandImage temphand = Instantiate(onHand, characterList.transform.parent);
        temphand.SetText(HeroInfoText());

        characterList.buildingSetWindow.isDrag = true;
        characterList.buildingSetWindow.vill_Interact = this;
        return temphand.image;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left
            || eventData.pointerCurrentRaycast.gameObject is null)
            return;


        copyHand.rectTransform.position = eventData.position - offset;
        //두 backboard 사이에 닿으면 새로 teamboard 생성.

        //teambackboard 중 다른 villigeinteract 혹은 그 사이에 닿으면 같은 파티로 소속.
        //teamBackBoard 확장.

        //다시 떼면 복구.

        GameObject dragObject = eventData.pointerCurrentRaycast.gameObject;


        if (dragObject.CompareTag("NameTag") && dragObject != gameObject)
            characterList.MoveBoard(dragObject, eventData.position.y);
        else if (dragObject == gameObject)
            NoMove();

    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
            return;

        Destroy(copyHand.gameObject);
        characterList.buildingSetWindow.SetisDragFalse();
        characterList.buildingSetWindow.vill_Interact = null;
        image.color = Color.white;
        transform.GetChild(0).gameObject.SetActive(true);

        characterList.EndDrag(this);
        isDrag = false;

        dragEndEvent();
        dragEndEvent = () => { };
    }

    public void SaveWorkPlace(BuildingComponent place, int index)
    {
        workingBuilding = place;
        workIndex = index;
    }
    public void DeleteWorkPlace()
    {
        GameManager.manager.onVilligeBuildingHeroCancellation.eventAction?.Invoke((int)workingBuilding.Type, workingBuilding.transform.position);
        SaveWorkPlace(null, 0);
        ChangeImage(AddressableManager.BuildingImage.Tomb, false);
    }
    public bool isCanLoad(out BuildingComponent workingbuilding, out int workindex)
    {
        workingbuilding = workingBuilding;
        workindex = workIndex;

        if (workingBuilding == null)
            return false;

        return workindex != 0;
    }
    public void NoMove()
    {
        characterList.NoMove(gameObject);
    }
    public void BeginDragOffset(PointerEventData eventData, Image rectImage)
    {
        base.OnBeginDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
            return;

        GameManager.manager.onVilligeHeroInteractDrag.eventAction?.Invoke(hero.lv,hero.transform.position);
        float subtractHeight = (eventData.pressPosition.y - rectImage.rectTransform.position.y)
                                / rectImage.rectTransform.sizeDelta.y * image.rectTransform.sizeDelta.y;
        offset = new Vector2(eventData.pressPosition.x - rectImage.rectTransform.position.x, subtractHeight);
        DragEvent(eventData);
    }
    public void ChangeImage(AddressableManager.BuildingImage imageNum, bool onoff = true)
    {
        jobImage.gameObject.SetActive(onoff);

        if (!onoff)
            return;

        characterList.buildingSetWindow.AddressableManager.GetData(AddressableManager.LabelName.Building, imageNum, out Sprite sprite);

        jobImage.sprite = sprite;
    }
    public void DragInvisible(bool onoff)
    {
        textPro.enabled = onoff;
        image.enabled = onoff;

        jobImage.color = Color.white * Convert.ToInt32(onoff);

    }
}
