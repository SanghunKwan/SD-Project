using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroTeam : InitInterface
{
    [SerializeField] TeamUI[] teamuis;
    Image backGround;
    Image frame;

    public int activeHeroTeamCount { get; private set; } = 0;
    public override void Init()
    {
        backGround = transform.GetChild(0).GetComponent<Image>();
        frame = transform.GetChild(1).GetComponent<Image>();
    }

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
}
