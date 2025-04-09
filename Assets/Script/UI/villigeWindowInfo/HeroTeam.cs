using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroTeam : MonoBehaviour
{
    [SerializeField] TeamUI[] teamuis;

    public int activeHeroTeamCount { get; private set; } = 2;

    public TeamUI GetTeamUI(int index)
    {
        return teamuis[index];
    }
    void SetTeamActive(int index, bool onoff)
    {
        if (teamuis[index].gameObject.activeSelf == onoff)
            return;

        teamuis[index].gameObject.SetActive(onoff);
        activeHeroTeamCount += (System.Convert.ToInt32(onoff) * 2) - 1;
    }
    public void SetTeamActiveCount(int Count)
    {
        for (int i = 0; i < Count; i++)
        {
            SetTeamActive(i, true);
        }
        for (int i = Count; i < teamuis.Length; i++)
        {
            SetTeamActive(i, false);
        }
    }
    public void AnimationEnd()
    {
        gameObject.SetActive(false);
    }


    public int[] GetHeroStageData()
    {
        int teamMembers = 5;
        int teamIndex = -1;
        int[] heroTeam = new int[activeHeroTeamCount * teamMembers];
        System.Array.Fill(heroTeam, -1);

        for (int i = 0; i < activeHeroTeamCount; i++)
        {
            for (int j = 0; j < teamMembers; j++)
            {
                teamIndex++;
                if (teamuis[i].villigeInteracts[j] is null)
                    continue;

                teamuis[i].villigeInteracts[j].GetCharacterListIndex();
            }
        }
        return heroTeam;
    }
}
