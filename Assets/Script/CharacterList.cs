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
    Image newCharacterTempBelong;
    [SerializeField] villigeInteract CharacterBackBoard;

    public Dictionary<string, Transform> keyToTeamsNum = new Dictionary<string, Transform>();
    List<string> keyCount = new List<string>();
    List<Transform> timeArray = new List<Transform>();

    public Dictionary<Transform, villigeViewPort> trViewPort = new Dictionary<Transform, villigeViewPort>();

    [SerializeField] GameObject defObject;
    Transform defSaveTr;
    int defSaveInt;
    ARRAYTYPE arType = ARRAYTYPE.TIME;

    Action[] actions = new Action[(int)ARRAYTYPE.MAX];
    enum ARRAYTYPE
    {
        KEYBOARD,
        TIME,
        MAX
    }

    public BuildingSetWindow buildingSetWindow {  get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        viewPortTransform = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        scrollTransform = transform.GetChild(0).GetComponent<RectTransform>();
        SetCharacterListLength();
        actions[(int)ARRAYTYPE.KEYBOARD] = CleanKeyBoard;
        actions[(int)ARRAYTYPE.TIME] = CleanTime;
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


        int length = viewPortTransform.childCount * 20 + 100 * (GameManager.manager.playerCharacter.Count + add);
        viewPortTransform.sizeDelta = new Vector2(viewPortTransform.sizeDelta.x,
            length);


        scrollTransform.sizeDelta = new Vector2(scrollTransform.sizeDelta.x,
            Mathf.Min(viewPortTransform.sizeDelta.y, 650));
    }
    public void NewCharacterCall(Unit.TypeNum type, Vector3 vec, SoundManager soundManager)
    {
        int offset = 0;
        for (int i = 0; i < viewPortTransform.childCount; i++)
        {
            offset -= 20;
            offset -= viewPortTransform.GetChild(i).childCount * 100;
        }


        if (newCharacterTempBelong == null)
        {
            newCharacterTempBelong = BackBoard(out _);
            newCharacterTempBelong.rectTransform.anchoredPosition = new Vector2(0, offset);
            keyToTeamsNum.Add("=", newCharacterTempBelong.transform);
            timeArray.Add(newCharacterTempBelong.transform);
        }

        villigeInteract nameTag = Instantiate(CharacterBackBoard, newCharacterTempBelong.transform);
        nameTag.HeroIniit((int)type);
        VilligeHero vh = nameTag.hero.gameObject.AddComponent<VilligeHero>();
        vh.hero.unitMove.SetSound(soundManager);
        nameTag.hero.isDefaultName = true;

        trViewPort[keyToTeamsNum["="]].characters.Add(nameTag);
        vh.Init(nameTag);

        ReArrage();
    }
    public Image BackBoard(out villigeViewPort port)
    {
        villigeViewPort backboard = Instantiate(TeamBackBoard, viewPortTransform.transform);
        trViewPort.Add(backboard.transform, backboard);
        port = backboard;
        return backboard.image;
    }
    public void MoveBoard(GameObject raycastTarget, float eventDataPositionY)
    {
        defObject.SetActive(true);
        bool upDownCheck;
        int add;

        if (raycastTarget.transform.parent == viewPortTransform)
        {
            upDownCheck = raycastTarget.transform.position.y - trViewPort[raycastTarget.transform].image.raycastPadding.w - eventDataPositionY < 60;
            add = Convert.ToInt32(!upDownCheck);

            //gameobejct가 teambackboard
            defSaveTr = raycastTarget.transform;
            defSaveInt = raycastTarget.transform.GetSiblingIndex() + add;

            SetPositionTeamBackBoard(defSaveInt);
            defObject.transform.position = new Vector2(defObject.transform.position.x,
                                                       raycastTarget.transform.position.y + 110 - (add * (raycastTarget.transform.childCount * 100 + 140)));
        }
        else
        {
            upDownCheck = raycastTarget.transform.position.y - eventDataPositionY
                               - trViewPort[raycastTarget.transform.parent].characters[raycastTarget.transform.GetSiblingIndex()].image.raycastPadding.w < 50;
            add = Convert.ToInt32(!upDownCheck);

            defSaveTr = raycastTarget.transform;
            defSaveInt = raycastTarget.transform.GetSiblingIndex() + add;

            SetPositionCharacterBackBoard(raycastTarget.transform.parent.GetSiblingIndex(), defSaveInt);
            defObject.transform.position = new Vector2(defObject.transform.position.x, raycastTarget.transform.position.y + 100 - (add * 200));
        }
    }
    public void NoMove(GameObject gob)
    {
        defObject.SetActive(false);
        defObject.transform.position = new Vector2(defObject.transform.position.x, gob.transform.position.y);

        defSaveTr = gob.transform;
        defSaveInt = gob.transform.GetSiblingIndex();

        ReArrage();

    }

    void SetPositionTeamBackBoard(int teamSiblingIndex)
    {
        ArrangeBoard(teamSiblingIndex);

        PushBoard(teamSiblingIndex, 120);
        SetCharacterListLength(120);
    }
    void ArrangeBoard(int DestinationIndex)
    {
        for (int i = 0; i < DestinationIndex; i++)
        {
            if (!trViewPort[viewPortTransform.GetChild(i)].isMoved)
                continue;

            trViewPort[viewPortTransform.GetChild(i)].ImagePaddingMove(0);
        }
    }
    void SetPositionCharacterBackBoard(int teamSiblingIndex, int characterSiblingIndex)
    {
        ArrangeBoard(teamSiblingIndex);

        trViewPort[viewPortTransform.GetChild(teamSiblingIndex)].InterActMove(100, characterSiblingIndex);

        PushBoard(teamSiblingIndex + 1, 100);

        SetCharacterListLength(100);
    }
    void PushBoard(int teamSiblingIndex, int distance)
    {
        for (int i = teamSiblingIndex; i < viewPortTransform.childCount; i++)
        {
            trViewPort[viewPortTransform.GetChild(i)].ImagePaddingMove(distance);
        }
    }
    public void SetTeamMove(villigeInteract moveObject, Transform trParent)
    {
        string tempStr = trViewPort[trParent].characters[0].hero.keycode;


    }
    public void EndDrag(villigeInteract moveObjectTransform)
    {
        moveObjectTransform.transform.position = defObject.transform.position;
        defObject.SetActive(false);


        string tempStr = moveObjectTransform.hero.keycode;
        Transform tr = moveObjectTransform.transform.parent;

        if (defSaveTr.parent == viewPortTransform)
        {
            trViewPort[tr].characters.RemoveAt(moveObjectTransform.transform.GetSiblingIndex());
            Debug.Log(PlayerNavi.nav.PlayerCharacter[tempStr].Count);

            Image image = BackBoard(out villigeViewPort trParent);
            image.transform.position = defObject.transform.position + new Vector3(100, 10);
            image.transform.SetSiblingIndex(defSaveInt);

            string newString = PlayerNavi.nav.GetEmptyTeamString();
            PlayerNavi.nav.PlayerCharacter[tempStr].Remove(moveObjectTransform.hero.unitMove as Character);
            //null값 반환 시 옮김 실패 추가
            moveObjectTransform.ChangeTeamKey(newString);

            moveObjectTransform.transform.SetParent(trParent.transform);
            trParent.characters.Add(moveObjectTransform);
            keyToTeamsNum.Add(newString, moveObjectTransform.transform.parent);
            timeArray.Insert(defSaveInt, moveObjectTransform.transform.parent);
        }
        else
        {
            int prevIndex = moveObjectTransform.transform.GetSiblingIndex();
            moveObjectTransform.ChangeTeamKey(trViewPort[defSaveTr.parent].characters[0].hero.keycode);
            trViewPort[tr].characters.RemoveAt(moveObjectTransform.transform.GetSiblingIndex());

            moveObjectTransform.transform.SetParent(defSaveTr.parent);

            defSaveInt -= Convert.ToInt32(tr == moveObjectTransform.transform.parent && defSaveInt > prevIndex);
            moveObjectTransform.transform.SetSiblingIndex(defSaveInt);
            trViewPort[moveObjectTransform.transform.parent].characters.Insert(defSaveInt, moveObjectTransform);
        }

        CheckCount(tempStr);


        ReArrage();
    }

    void CheckCount(in string tempStr)
    {
        Transform tr = trViewPort[keyToTeamsNum[tempStr]].transform;
        if (tr.childCount <= 0)
        {
            keyToTeamsNum.Remove(tempStr);
            trViewPort.Remove(tr);
            timeArray.Remove(tr);
            tr.SetParent(null);
            
            Destroy(tr.gameObject);
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
            Debug.Log("실행");
            BackBoard(out villigeViewPort port);

            keyToTeamsNum.Add(keycode, port.transform);
            timeArray.Add(port.transform);
        }

        vh.villigeInteract.transform.SetParent(trViewPort[keyToTeamsNum[keycode]].transform);
        trViewPort[keyToTeamsNum[keycode]].characters.Add(vh.villigeInteract);
        vh.villigeInteract.ChangeTeamKey(keycode);

        CheckCount(st);
        ReArrage();
        Debug.Log(keyToTeamsNum.Count);
        Debug.Log(trViewPort.Count);
        Debug.Log(timeArray.Count);
    }
    public void ReArrage()
    {

        SetCharacterListLength();
        MovedClean();
    }
    #endregion 키보드
    #region 다른UI상호작용
    #endregion
}
