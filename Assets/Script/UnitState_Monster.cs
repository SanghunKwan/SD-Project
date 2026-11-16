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
}
