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


    [SerializeField] Hero[] Heros;
    public Hero hero { get; private set; }
    Animator anim;
    int animOnMouse;
    WindowInfo infoWindow;



    [SerializeField] HandImage onHand;
    Image copyHand;
    Vector2 offset;
    CharacterList characterList;

    TextMeshProUGUI textPro;

    BuildingComponent workingBuilding;
    int workIndex;



    public void HeroIniit(int index)
    {
        hero = Instantiate(Heros[index], Vector3.zero, Quaternion.identity, PlayerNavi.nav.transform);
        hero.MakeQuirk();
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
        if(workingBuilding == characterList.buildingSetWindow.ienumOwner)
        {
            characterList.buildingSetWindow.BuildSetCharactersCheck(workIndex, this);
        }
    }
    string HeroInfoText()
    {
        return "<mark=#00000055><size=60>" + hero.keycode + " </size></mark> " + hero.stat.Lv + " " + hero.stat.NAME;
    }
    protected override void Start()
    {
        base.Start();

        textPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        animOnMouse = Animator.StringToHash("onMouse");

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

            GameManager.manager.Unselect(GameManager.manager.playerCharacter);
            GameManager.manager.Unselect(GameManager.manager.objects);

            GameManager.manager.ScreenToPoint(hero.transform.position);

            hero.Selected(true);
            PlayerNavi.nav.HeroAdd(hero);
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
        if (!eventData.dragging)
            clicks[(int)eventData.button](eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (eventData.button == PointerEventData.InputButton.Left)
            return;



        offset = new Vector2(eventData.pressPosition.x - image.rectTransform.position.x,
                             eventData.pressPosition.y - image.rectTransform.position.y);
        copyHand = CreateHandImage();
        image.color = Color.clear;
        transform.GetChild(0).gameObject.SetActive(false);
        //글자 복사 추가.
        characterList.NoMove(gameObject);
    }
    Image CreateHandImage()
    {
        HandImage temphand = Instantiate(onHand, characterList.transform.parent);
        temphand.Init();
        temphand.SetText(HeroInfoText());

        characterList.buildingSetWindow.isDrag = true;
        characterList.buildingSetWindow.vill_Interact = this;
        return temphand.image;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
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
            characterList.NoMove(gameObject);

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
    }

    public void SaveWorkPlace(BuildingComponent place, int index)
    {
        workingBuilding = place;
        workIndex = index;
    }
    public bool isCanLoad(out BuildingComponent workingbuilding, out int workindex)
    {
        workingbuilding = workingBuilding;
        workindex = workIndex;

        if (workingBuilding == null)
            return false;



        return true;
    }
}
