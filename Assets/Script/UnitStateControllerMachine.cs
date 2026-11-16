using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateControllerMachine : StateMachineBehaviour
{
    static int IsHit = Animator.StringToHash("HitMachine");

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.SetBool(IsHit, true);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.SetBool(IsHit, false);
    }
}
