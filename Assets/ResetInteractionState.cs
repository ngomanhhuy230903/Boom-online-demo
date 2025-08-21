using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetInteractionState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.OnInteractionEnd();
        }
    }
}
