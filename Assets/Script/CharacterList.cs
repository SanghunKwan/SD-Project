using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterList : MonoBehaviour
{
    RectTransform viewPortTransform;
    RectTransform scrollTransform;
    [SerializeField] villigeViewPort TeamBackBoard;
    [SerializeField] villigeInteract CharacterBackBoard;

    public Dictionary<string, GameObject> keyToTeamsNum { get; private set; } = new Dictionary<string, GameObject>();
    List<string> keyCount = new List<string>();
    List<GameObject> timeArray = new List<GameObject>();

    public Dictionary<GameObject, villigeViewPort> trViewPort { get; private set; } = new Dictionary<GameObject, villigeViewPort>();

    [SerializeField] GameObject defObject;
    Transform defSaveTr;
    int defSaveInt;
    ARRAYTYPE arType = ARRAYTYPE.TIME;

    Action[] actions = new Action[(int)ARRAYTYPE.MAX];
    enum ARRAYTYPE
    {
        KEYBOARD,
        TIME,
        TRANSFORM,
        MAX
    }

    public BuildingSetWindow buildingSetWindow { get; private set; }
    [SerializeField] HandImage handImage;

    // Start is called before the first frame update
    void Start()
    {
        viewPortTransform = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        scrollTransform = transform.GetChild(0).GetComponent<RectTransform>();
        SetCharacterListLength();
        actions[(int)ARRAYTYPE.KEYBOARD] = CleanKeyBoard;
        actions[(int)ARRAYTYPE.TIME] = CleanTime;
        actions[(int)ARRAYTYPE.TRANSFORM] = CleanByTransform;
        CreateKeyCount();

        buildingSetWindow = transform.parent.Find("BuildingSetWindow").GetComponent<BuildingSetWindow>();
    }
    void CreateKeyCount()
    {
        keyCount.Add("`");
        keyCount.Add("1");
        keyCount.Add("2");
        keyCount.Add("3");
        keyCount.Add("4");
        keyCount.Add("5");
        keyCount.Add("6");
        keyCount.Add("7");
        keyCount.Add("8");
        keyCount.Add("9");
        keyCount.Add("0");
        keyCount.Add("-");
        keyCount.Add("=");
    }
    void SetCharacterListLength(int add = 0)
    {
        if (viewPortTransform.childCount <= 0)
            return;

        int viewPortLength = viewPortTransform.childCount * 100 + 100 * (GetHeroCount() + add);
        viewPortTransform.sizeDelta = new Vector2(viewPortTransform.sizeDelta.x, viewPortLength);

        scrollTransform.sizeDelta = new Vector2(scrollTransform.sizeDelta.x,
            Mathf.Min(viewPortTransform.sizeDelta.y, 650));
    }
    int GetHeroCount()
    {
        int count = 0;
        foreach (var item in trViewPort.Values)
        {
            count += item.characters.Count;
        }
        return count;
    }
    public void NewCharacterCall(Unit.TypeNum type, Vector3 vec)
    {
        SpawnVilligeInteract("=");

        villigeInteract nameTag = trViewPort[keyToTeamsNum["="]].characters[^1];
        nameTag.HeroIniit((int)type);
        VilligeHero vh = nameTag.hero.gameObject.AddComponent<VilligeHero>();
        nameTag.hero.isDefaultName = true;
        vh.Init(nameTag);
        ReArrage();

    }
    public void SpawnVilligeInteract(in string teamKeycode, SaveData.HeroData heroData = null)
    {
        if (!keyToTeamsNum.TryGetValue(teamKeycode, out GameObject portObject))
        {
            Image tempImage = BackBoard(out _, teamKeycode);
            tempImage.rectTransform.anchoredPosition = new Vector2(0, GetViewPortTransform());
            portObject = tempImage.gameObject;
            keyToTeamsNum.Add(teamKeycode, portObject);
            timeArray.Add(portObject);
        }

        villigeInteract nameTag = Instantiate(CharacterBackBoard, portObject.transform);
        nameTag.Init(handImage);
        trViewPort[portObject].characters.Add(nameTag);

        if (heroData != null)
            nameTag.SetNameTag(heroData);
    }
    public void MatchingHeroWithInteract(int heroIndex, Unit.Hero hero)
    {
        villigeInteract nametag = trViewPort[keyToTeamsNum[hero.keycode]].characters[heroIndex];
        nametag.MatchHero(hero);
        nametag.hero.ResetFieldEquip();
        VilligeHero vh = nametag.hero.gameObject.AddComponent<VilligeHero>();
        vh.Init(nametag);
    }

    int GetViewPortTransform()
    {
        int offset = 0;
        for (int i = 0; i < viewPortTransform.childCount; i++)
        {
            offset -= 100;
            offset -= viewPortTransform.GetChild(i).childCount * 100;
        }

        return offset;
    }

    public Image BackBoard(out villigeViewPort port, in string teamCode = "=")
    {
        villigeViewPort backboard = Instantiate(TeamBackBoard, viewPortTransform);
        trViewPort.Add(backboard.gameObject, backboard);
        port = backboard;
        port.ChangeTeamCode(teamCode);
        return backboard.image;
    }
    public void MoveBoard(GameObject raycastTarget, float eventDataPositionY)
    {
        defObject.SetActive(true);
        bool isDownCheck;
        int add;

        if (raycastTarget.transform.parent == viewPortTransform)
        {
            isDownCheck = raycastTarget.transform.position.y - trViewPort[raycastTarget.gameObject].image.raycastPadding.w - eventDataPositionY < 100;
            add = Convert.ToInt32(!isDownCheck);

            defSaveTr = raycastTarget.transform;
            defSaveInt = raycastTarget.transform.GetSiblingIndex() + add;

            SetPositionTeamBackBoard(defSaveInt);
            defObject.transform.position
                = new Vector2(defObject.transform.position.x,
                              raycastTarget.transform.position.y + 120
                              - (add * ((raycastTarget.transform.childCount - 1) * 100 + 300)));
        }
        else
        {
            isDownCheck = raycastTarget.transform.position.y - eventDataPositionY
                               - trViewPort[raycastTarget.transform.parent.gameObject]
                                 .characters[raycastTarget.transform.GetSiblingIndex() - 1].image.raycastPadding.w < 50;
            add = Convert.ToInt32(!isDownCheck);

            defSaveTr = raycastTarget.transform;
            defSaveInt = raycastTarget.transform.GetSiblingIndex() + add - 1;

            SetPositionCharacterBackBoard(raycastTarget.transform.parent.GetSiblingIndex(), defSaveInt);
            defObject.transform.position = new Vector2(defObject.transform.position.x, raycastTarget.transform.position.y + 100 - (add * 200));
        }
    }

    public void NoMove(GameObject gob, bool isVilligeInteract = true)
    {
        defObject.SetActive(false);
        defObject.transform.position = new Vector2(defObject.transform.position.x, gob.transform.position.y);

        defSaveTr = gob.transform;
        defSaveInt = gob.transform.GetSiblingIndex() - Convert.ToInt32(isVilligeInteract);

        ReArrage();

    }

    void SetPositionTeamBackBoard(int teamSiblingIndex)
    {
        ArrangeBoard(teamSiblingIndex);

        PushBoard(teamSiblingIndex, 200);
        SetCharacterListLength(200);
    }
    void ArrangeBoard(int DestinationIndex)
    {
        for (int i = 0; i < DestinationIndex; i++)
        {
            if (!GetViewPortByIndex(i).isMoved)
                continue;

            GetViewPortByIndex(i).ImagePaddingMove(0);
        }
    }
    void SetPositionCharacterBackBoard(int teamSiblingIndex, int characterSiblingIndex)
    {
        ArrangeBoard(teamSiblingIndex);

        GetViewPortByIndex(teamSiblingIndex).InterActMove(100, characterSiblingIndex);

        PushBoard(teamSiblingIndex + 1, 100);

        SetCharacterListLength(100);
    }
    void PushBoard(int teamSiblingIndex, int distance)
    {
        for (int i = teamSiblingIndex; i < viewPortTransform.childCount; i++)
        {
            GetViewPortByIndex(i).ImagePaddingMove(distance);
        }
    }
    public void EndDrag()
    {
        defObject.SetActive(false);
        ReArrage();
    }
    public void EndDrag(villigeInteract moveObjectTransform)
    {
        moveObjectTransform.transform.position = defObject.transform.position;

        string tempStr = moveObjectTransform.hero.keycode;
        string newString;
        GameObject parentObject = moveObjectTransform.transform.parent.gameObject;

        if (defSaveTr.parent == viewPortTransform)
        {
            trViewPort[parentObject].characters.RemoveAt(moveObjectTransform.transform.GetSiblingIndex() - 1);
            newString = PlayerNavi.nav.GetEmptyTeamString();

            Image image = BackBoard(out villigeViewPort trParent, newString);
            image.transform.position = defObject.transform.position + new Vector3(100, 10);
            image.transform.SetSiblingIndex(defSaveInt);

            
            //null값 반환 시 옮김 실패 추가
            moveObjectTransform.ChangeTeamKey(newString);

            moveObjectTransform.transform.SetParent(trParent.transform);
            trParent.characters.Add(moveObjectTransform);
            keyToTeamsNum.Add(newString, moveObjectTransform.transform.parent.gameObject);
            timeArray.Insert(defSaveInt, moveObjectTransform.transform.parent.gameObject);
        }
        else
        {
            int add = -Convert.ToInt32(parentObject.transform == moveObjectTransform.transform.parent
                                    && defSaveInt > moveObjectTransform.transform.GetSiblingIndex() - 1);

            newString = trViewPort[defSaveTr.parent.gameObject].characters[0].hero.keycode;
            VilligeInteractMove(moveObjectTransform, trViewPort[defSaveTr.parent.gameObject], defSaveInt + add);
        }
        PlayerNavi.nav.PlayerCharacter[tempStr].Remove(moveObjectTransform.hero.unitMove as Character);
        CheckCount(tempStr);
        EndDrag();
    }
    void VilligeInteractMove(villigeInteract moveObject, villigeViewPort targetViewPort, int index)
    {
        moveObject.ChangeTeamKey(targetViewPort.characters[0].hero.keycode);
        trViewPort[moveObject.transform.parent.gameObject].characters.RemoveAt(moveObject.transform.GetSiblingIndex() - 1);
        moveObject.transform.SetParent(targetViewPort.transform);
        moveObject.transform.SetSiblingIndex(index + 1);
        targetViewPort.characters.Insert(index, moveObject);
    }
    void CheckCount(in string tempStr)
    {
        GameObject portObject = trViewPort[keyToTeamsNum[tempStr]].gameObject;
        if (portObject.transform.childCount <= 1)
        {
            keyToTeamsNum.Remove(tempStr);
            trViewPort.Remove(portObject);
            timeArray.Remove(portObject);
            portObject.transform.SetParent(null);

            Destroy(portObject);
        }
    }
    void MovedClean()
    {
        actions[(int)arType]();
    }
    void CleanTime()
    {
        float fAccumulateY = 0;

        for (int i = 0; i < timeArray.Count; i++)
        {
            trViewPort[timeArray[i]].ArrangeReset();
            trViewPort[timeArray[i]].transform.localPosition = Vector3.up * fAccumulateY;
            fAccumulateY -= trViewPort[timeArray[i]].image.rectTransform.sizeDelta.y;
            trViewPort[timeArray[i]].transform.SetAsLastSibling();
        }
    }
    void CleanKeyBoard()
    {
        float fAccumulateY = 0;

        foreach (var item in keyCount)
        {
            if (!keyToTeamsNum.ContainsKey(item))
                continue;

            trViewPort[keyToTeamsNum[item]].ArrangeReset();
            trViewPort[keyToTeamsNum[item]].transform.localPosition = Vector3.up * fAccumulateY;
            fAccumulateY -= trViewPort[keyToTeamsNum[item]].image.rectTransform.sizeDelta.y;
            trViewPort[keyToTeamsNum[item]].transform.SetAsLastSibling();
        }
    }
    void CleanByTransform()
    {
        float fAccumulateY = 0;

        for (int i = 0; i < viewPortTransform.childCount; i++)
        {
            GetViewPortByIndex(i).ArrangeReset();
            GetViewPortByIndex(i).transform.localPosition = Vector3.up * fAccumulateY;
            fAccumulateY -= GetViewPortByIndex(i).image.rectTransform.sizeDelta.y;
        }
    }
    #region 버튼
    public void ArrayTime()
    {
        arType = ARRAYTYPE.TIME;
        MovedClean();

    }
    public void ArrayKeyBoard()
    {
        arType = ARRAYTYPE.KEYBOARD;
        MovedClean();
    }
    #endregion 버튼
    #region 키보드
    public void KeyboardSetTeam(VilligeHero vh, in string keycode)
    {
        //SetTeamMove(vh, );

        Transform trPort = vh.villigeInteract.transform.parent;
        string st = vh.hero.keycode;

        if (!keyToTeamsNum.ContainsKey(keycode))
        {
            BackBoard(out villigeViewPort port, keycode);

            keyToTeamsNum.Add(keycode, port.gameObject);
            timeArray.Add(port.gameObject);
        }

        vh.villigeInteract.transform.SetParent(trViewPort[keyToTeamsNum[keycode]].transform);
        trViewPort[keyToTeamsNum[keycode]].characters.Add(vh.villigeInteract);
        vh.villigeInteract.ChangeTeamKey(keycode);

        CheckCount(st);
        ReArrage();
    }
    public void ReArrage()
    {
        arType = ARRAYTYPE.TRANSFORM;
        SetCharacterListLength();
        MovedClean();
    }
    #endregion 키보드
    #region VilligeTeamCollider 상호작용
    public void EndDrag(villigeViewPort parentPort)
    {
        defObject.SetActive(false);

        bool isInteract = defSaveTr.parent != viewPortTransform.transform;
        bool isSiblingCountIncline = !isInteract && parentPort.transform.GetSiblingIndex() < defSaveInt;
        int add = -Convert.ToInt32(isSiblingCountIncline) + Convert.ToInt32(isInteract);

        parentPort.transform.SetParent(defSaveTr.parent);
        parentPort.transform.SetSiblingIndex(defSaveInt + add);

        if (isInteract)
        {
            VilligeInteractsMove(parentPort, trViewPort[defSaveTr.parent.gameObject], parentPort.transform.GetSiblingIndex());
            CheckCount(parentPort.characters[0].hero.keycode);
        }
        ReArrage();
    }
    void VilligeInteractsMove(villigeViewPort startViewPort, villigeViewPort arriveViewPort, int index)
    {
        villigeInteract tempInteract;
        int tempIndex = index + 1;
        int characterCount = startViewPort.characters.Count;
        for (int i = startViewPort.characters.Count - 1; i >= 0; i--)
        {
            tempInteract = startViewPort.characters[i];
            tempInteract.ChangeTeamKey(arriveViewPort.characters[0].hero.keycode);
            tempInteract.transform.SetParent(arriveViewPort.transform);
            tempInteract.transform.SetSiblingIndex(tempIndex);
        }

        arriveViewPort.characters.InsertRange(index, startViewPort.characters);
        startViewPort.characters.RemoveRange(0, characterCount);
    }
    public void CollidersSetInteractive(bool onoff)
    {
        foreach (var item in trViewPort.Values)
        {
            item.TeamColliderSetInteractive(onoff);
        }
    }
    #endregion

    #region 다른UI상호작용


    #endregion
    #region 저장된 영웅 목록
    public villigeViewPort GetViewPortByIndex(int index)
    {
        return trViewPort[viewPortTransform.GetChild(index).gameObject];
    }
    public int GetInteractIndex(villigeInteract interact)
    {
        GameObject villigeInteractObject = keyToTeamsNum[interact.hero.keycode];

        int portSiblingIndex = villigeInteractObject.transform.GetSiblingIndex();
        int beforePortCharacterCount = 0;

        for (int i = 0; i < portSiblingIndex; i++)
        {
            beforePortCharacterCount += GetViewPortByIndex(i).characters.Count;
        }

        return beforePortCharacterCount + trViewPort[villigeInteractObject].characters.BinarySearch(interact);
    }
    public villigeInteract GetInteractByIndex(int index)
    {
        if (index < 0)
            return null;

        int interactCount = 0;
        int viewPortLength = trViewPort.Count;
        int i = 0;

        do
        {
            interactCount += GetViewPortByIndex(i).characters.Count;
            i++;
            Debug.Log("index 번호" + i);
        } while (interactCount < index && i < viewPortLength);
        i--;

        return GetViewPortByIndex(i).characters[^(interactCount - index)];

    }
    #endregion
}
