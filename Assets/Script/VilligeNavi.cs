using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

public class VilligeNavi : PlayerNavi
{

    [SerializeField] CharacterList uiList;
    public Dictionary<Transform, VilligeHero> dicVh { get; private set; } = new Dictionary<Transform, VilligeHero>();
    public override void SetTeam(string keycode)
    {
        //1. lists�� item.cunit�� Heros�� ĳ�����ؼ� keycode�� ��� uiList�� dictionary<string,int>�� ��ġ�� ã�� �̵�.
        //2. lists�� �ɹ��� uiLIst ��ü���� ������ �� uiList�� villigeViewPort�� ���� ����.

        //uiList������ ��ġ�� Teams[keytoTeamNum[heros.keycode]]
        //���� ���� ĳ���ʹ� ���ο� �������� ������ �� Ȯ���� ����.

        foreach (var item in lists)
        {
            Hero unit = item.cUnit as Hero;
            PlayerCharacter[unit.keycode].Remove(item);
            uiList.trViewPort[uiList.keyToTeamsNum[unit.keycode]].characters.Remove(dicVh[unit.transform].villigeInteract);
        }

        //���� keycode�� �ִ� �������� ��� =�� �̵�.
        PlayerCharacter["="].AddRange(PlayerCharacter[keycode]);
        Debug.Log(PlayerCharacter["="].Count);

        for (int i = 0; i < lists.Count; i++)
        {
            Debug.Log(dicVh[lists[i].transform].name);
            uiList.KeyboardSetTeam(dicVh[lists[i].transform], keycode);
        }


    }




}
