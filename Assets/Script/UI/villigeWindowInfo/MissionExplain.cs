using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionExplain : InitInterface
{
    public Animator anim { get; private set; }

    public void AnimationEnd()
    {
        gameObject.SetActive(false);
    }
    public void AnimatoionStart()
    {
        int count = transform.GetChild(1).childCount;
        for (int i = 0; i < count; i++)
            transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
    }
    public void FadeInEnd()
    {
        int count = transform.GetChild(1).childCount;
        for (int i = 0; i < count; i++)
            transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
    }

    public override void Init()
    {
        anim = GetComponent<Animator>();
    }
}
