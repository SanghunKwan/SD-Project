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
    static int animOnMouse = Animator.StringToHash("onMouse");
    int workIndex;

    WindowInfo infoWindow;

    HandImage onHand;
    Vector2 offset;
    CharacterList characterList;

    TextMeshProUGUI lvText;
    TextMeshProUGUI nameText;

    BuildingComponent workingBuilding;

    public static bool isDrag { get; private set; }
    public static villigeInteract now_villigeInteract { get; private set; }


    public NowTeamUI teamUIData { get; private set; } = new NowTeamUI();
    Image jobImage;
    Image weaponImage;

    Image copyHand;
    GameObject childComponent;


    #region temp
    public void HeroIniit(int index)
    {
        hero = Instantiate(Heros[index], Vector3.zero, Quaternion.identity, PlayerNavi.nav.transform);
        hero.MakeQuirk();
    }
    public void Init(HandImage handImage)
    {
        onHand = handImage;
    }
    #endregion
    protected override void VirtualAwake()
    {
        childComponent = transform.GetChild(0).gameObject;

        lvText = childComponent.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText = childComponent.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        infoWindow = transform.parent.parent.parent.parent.parent.parent.Find("CharacterInfo").gameObject.GetComponent<WindowInfo>();
        characterList = transform.parent.parent.parent.parent.parent.GetComponent<CharacterList>();

        anim = GetComponent<Animator>();
        weaponImage = childComponent.transform.Find("weapon").GetComponent<Image>();
        jobImage = weaponImage.transform.GetChild(0).GetComponent<Image>();

    }
    public void MatchHero(Hero getHero)
    {
        hero = getHero;
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
        lvText.text = hero.lv.ToString();
        nameText.text = hero.name;

        if (workingBuilding != null && workingBuilding == characterList.buildingSetWindow.buildingComponent)
            characterList.buildingSetWindow.BuildSetCharactersCheck(workIndex, this);
    }
    public void SetNameTag(SaveData.HeroData heroData)
    {
        lvText.text = heroData.lv.ToString();
        nameText.text = heroData.name;

        AddressableManager.manager.DelayUntilLoadingComplete(() =>
        {
            AddressableManager.manager.GetData(WeaponLabelName(heroData), AddressableManager.EquipsImage.Weapon, out Sprite weaponSprite);
            weaponImage.sprite = weaponSprite;
        });

    }
    string WeaponLabelName(SaveData.HeroData heroData)
    {
        TypeNum getType = heroData.unitData.objectData.cur_status.type;
        AddressableManager.ItemQuality quality = (AddressableManager.ItemQuality)heroData.equipNum[0];

        return getType.ToString() + quality.ToString();
    }


    protected override void Start()
    {
        base.Start();

        clicks[(int)PointerEventData.InputButton.Left] = (eventdata) =>
        {
            bool off = infoWindow.gameObject.activeSelf && infoWindow.wStatus.targetObject == hero;

            infoWindow.gameObject.SetActive(!off);
            infoWindow.SetCam(!off, hero);
            if (!off)
                infoWindow.VilligeWindowOpen(hero);
        };
        clicks[(int)PointerEventData.InputButton.Right] = (eventdata) =>
        {
            PlayerNavi.nav.HeroClear();

            GameManager.manager.DragFalse(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.Hero]);
            GameManager.manager.DragFalse(GameManager.manager.objectManager.ObjectList[(int)ObjectManager.CObjectType.FieldObject]);

            if (characterList.NameTagInteractable)
                GameManager.manager.ScreenToPoint(hero.transform.position);

            hero.Selected(true);

            GameManager.manager.onHeroSelect.eventAction?.Invoke(1, hero.transform.position);
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
        if (!eventData.dragging && hero != null && PlayerInputManager.manager.windowInputEnable[(int)eventData.button])
        {
            int buttonIndex = (int)eventData.button;
            clicks[buttonIndex](eventData);
            GameManager.manager.onVilligeHeroInteractClick.eventAction?.Invoke(buttonIndex, Vector3.zero);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragOffset(eventData, image);
    }
    public void DragEvent(PointerEventData eventData)
    {
        copyHand = GetHandImage();
        image.color = Color.clear;
        childComponent.gameObject.SetActive(false);
        //글자 복사 추가.
        NoMove();
        isDrag = true;
        now_villigeInteract = this;
    }
    Image GetHandImage()
    {
        onHand.SetText(hero);
        onHand.gameObject.SetActive(true);

        characterList.buildingSetWindow.isDrag = true;
        characterList.buildingSetWindow.vill_Interact = this;
        return onHand.image;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left
            || eventData.pointerCurrentRaycast.gameObject is null
            || hero == null
            || (!PlayerInputManager.manager.windowInputEnable[(int)eventData.button]))
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
        if (eventData.button == PointerEventData.InputButton.Left || hero == null
            || (!PlayerInputManager.manager.windowInputEnable[(int)eventData.button]))
            return;

        onHand.gameObject.SetActive(false);
        characterList.buildingSetWindow.SetisDragFalse();
        characterList.buildingSetWindow.vill_Interact = null;
        image.color = Color.white;
        childComponent.SetActive(true);

        characterList.EndDrag(this);
        isDrag = false;

        dragEndEvent();
        dragEndEvent = () => { };
    }
    public void LoadWorkPlace(BuildingComponent place, int index)
    {
        SaveWorkPlace(place, index);
        ChangeImage(place.Type);
        hero.alloBuilding(place.Type);
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
        hero.unitMove.BuildingWorkEnd();
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
        if (eventData.button == PointerEventData.InputButton.Left || hero == null
             || (!PlayerInputManager.manager.windowInputEnable[(int)eventData.button]))
            return;

        GameManager.manager.onVilligeHeroInteractDrag.eventAction?.Invoke(0, hero.transform.position);
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
        lvText.enabled = onoff;
        nameText.enabled = onoff;
        image.enabled = onoff;
        weaponImage.enabled = onoff;

        jobImage.color = Color.white * Convert.ToInt32(onoff);
    }

    public int GetCharacterListIndex()
    {
        return characterList.GetInteractIndex(this);
    }
}
