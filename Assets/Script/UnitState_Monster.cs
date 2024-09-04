using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitState_Monster : UnitState
{
    public enum StandType
    {
        Basic,
        HeadShake,
        LookAround,
        PickupEat,
        Jumping,
        Crouch,
        Max
    }
    StandType standType;
    [SerializeField] AnimationClip[] standClips;

    protected override void Start()
    {
        base.Start();
        basic_Ani.SetLayerWeight(3, 0);
    }
    public void SetStandType(StandType type)
    {
        standType = type;
        AnimatorOverrideController aOC = (AnimatorOverrideController)basic_Ani.runtimeAnimatorController;
        aOC["StandB@Loop"] = standClips[(int)standType];
    }
    public void Aggravation()
    {
        basic_Ani.SetTrigger("Skill1");
    }

    public override void FocusTarget(bool downIsTrue)
    {

    }

    public IEnumerator WardenHeadWeigh()
    {

        base.FocusTarget(false);
        float time = Mathf.Abs(turningTime);

        yield return DownWeigh(time);
    }

    IEnumerator DownWeigh(float timing)
    {
        yield return new WaitUntil(() => turningTime <= 0);
        yield return new WaitUntil(() => timing < turningTime);
        base.FocusTarget(true);

    }
}
