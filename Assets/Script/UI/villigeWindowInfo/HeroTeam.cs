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
        int tempBuildingIndex;
        BuildingComponent tempBuildingComponent;
        List<int> heroTeam = new List<int>(5);

        for (int i = 0; i < activeHeroTeamCount; i++)
        {
            for (int j = 0; j < teamMembers; j++)
            {
                teamIndex++;
                if (teamuis[i].villigeInteracts[j] is null)
                    continue;

                heroTeam.Add(teamuis[i].villigeInteracts[j].GetCharacterListIndex());
                if (teamuis[i].villigeInteracts[j].isCanLoad(out tempBuildingComponent, out tempBuildingIndex))
                {
                    tempBuildingComponent.ResetData(tempBuildingIndex);
                }
            }
            if (i == 1)
                Debug.Log("2팀 출전 미구현");
        }
        return heroTeam.ToArray();
    }
}
