using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashMovement : StateMachineBehaviour
{
    GameObject player;
    float dashSpeed;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        player = animator.transform.parent.parent.gameObject;
        dashSpeed = 200;

        player.GetComponent<playerMovement>().dashing = true;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animatorStateInfo.IsName("Armature_Dash"))
        {
            player.GetComponent<CharacterController>().Move(player.transform.forward * dashSpeed * Time.deltaTime);
        }

        if (animatorStateInfo.IsName("Armature_DashEnd"))
        {
            dashSpeed -= (500 * Time.deltaTime);

            player.GetComponent<CharacterController>().Move(player.transform.forward * dashSpeed * Time.deltaTime);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        player.GetComponent<playerMovement>().dashing = false;
    }
}
