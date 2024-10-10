using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDUI;

public class TeamUI : MonoBehaviour
{
    BuildSetCharacter[] buildSetCharacters;
    public villigeInteract[] villigeInteracts { get; private set; }

    [SerializeField] int characterNum;


    private void Awake()
    {
        buildSetCharacters = new BuildSetCharacter[characterNum];
        villigeInteracts = new villigeInteract[characterNum];

        for (int i = 0; i < buildSetCharacters.Length; i++)
        {
            buildSetCharacters[i] = new BuildSetCharacter(transform.GetChild(i));
        }
    }

    public void EnterCollider(int siblingIndex, villigeInteract nowVilligeInteract)
    {

        buildSetCharacters[siblingIndex].ChangeTeam(nowVilligeInteract.hero.stat.NAME, nowVilligeInteract.hero.keycode);
    }
    public void ExitCollider(int siblingIndex)
    {
        if (villigeInteracts[siblingIndex] == null)
            buildSetCharacters[siblingIndex].ResetTeam();
        else
            buildSetCharacters[siblingIndex].ChangeTeam(villigeInteracts[siblingIndex].hero.stat.NAME,
                villigeInteracts[siblingIndex].hero.keycode);
    }
    public void CharacterSave(int siblingIndex, villigeInteract villigeInteract)
    {
        villigeInteracts[siblingIndex] = villigeInteract;
    }
    public void DeleteSave(int siblingIndex, bool isSiblingIndexSame = false)
    {
        if (isSiblingIndexSame)
            return;

        villigeInteracts[siblingIndex] = null;
        buildSetCharacters[siblingIndex].ResetTeam();
    }

}
public class NowTeamUI
{
    public TeamUI teamUI { get; private set; }
    public int teamSiblingIndex { get; private set; }

    public void SaveData(TeamUI team, int sibling)
    {
        teamUI = team;
        teamSiblingIndex = sibling;
    }
    public void Remove()
    {
        teamUI = null;
        teamSiblingIndex = 0;
    }
    public bool CanLoadData(out TeamUI team, out int sibling)
    {
        team = teamUI;
        sibling = teamSiblingIndex;
        return teamUI != null;
    }
}
