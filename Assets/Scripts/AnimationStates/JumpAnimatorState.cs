using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimatorState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("IsInJump", true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool("IsInJump", false);
    }
}
