using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class VilligeNavi : PlayerNavi
{

    [SerializeField] CharacterList uiList;
    public Dictionary<GameObject, VilligeHero> dicVh { get; private set; } = new Dictionary<GameObject, VilligeHero>();
    public override void SetTeam(string keycode)
    {
        //1. lists의 item.cunit을 Heros로 캐스팅해서 keycode를 얻어 uiList의 dictionary<string,int>로 위치를 찾아 이동.
        //2. lists의 맴버를 uiLIst 전체에서 삭제한 후 uiList의 villigeViewPort에 새로 생성.

        //uiList에서의 위치는 Teams[keytoTeamNum[heros.keycode]]
        //기존 리더 캐릭터는 새로운 팀에서도 리더가 될 확률이 높음.
        Hero unit;
        foreach (var item in lists)
        {
            unit = item.cUnit as Hero;
            PlayerCharacter[unit.keycode].Remove(item);
            uiList.trViewPort[uiList.keyToTeamsNum[unit.keycode]].characters.Remove(dicVh[unit.gameObject].villigeInteract);
        }

        //기존 keycode에 있던 영웅들은 모두 =로 이동.
        PlayerCharacter["="].AddRange(PlayerCharacter[keycode]);
        Debug.Log(PlayerCharacter["="].Count);

        for (int i = 0; i < lists.Count; i++)
        {
            Debug.Log(dicVh[lists[i].gameObject].name);
            uiList.KeyboardSetTeam(dicVh[lists[i].gameObject], keycode);
        }


    }




}
