using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeAttack : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        GameObject thisUnit = animator.transform.parent.parent.gameObject;

        if (thisUnit.CompareTag("Player")){
            Debug.Log(thisUnit);

            Vector3 MeleeHitCenter = GameObject.Find("MeleeCheck").transform.position;

            if (Physics.CheckBox(MeleeHitCenter, new Vector3(2, 2, 2), Quaternion.identity, ~10))
            {
                if (GameObject.FindGameObjectsWithTag("Enemy") != null)
                {
                    GameObject closestEnemy = GameObject.FindGameObjectsWithTag("Enemy")[0];

                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        float thisDist = (MeleeHitCenter - enemy.transform.position).magnitude;
                        float closestDist = (MeleeHitCenter - closestEnemy.transform.position).magnitude;

                        if (thisDist < closestDist)
                        {
                            closestEnemy = enemy;
                        }
                    }

                    closestEnemy.GetComponent<CharacterParent>().meleeHit(50, 10);
                }
            }
        }
        else if(thisUnit.CompareTag("Enemy"))
        {
            Debug.Log(thisUnit);
            GameObject.Find("Player").GetComponent<CharacterParent>().meleeHit(50, 10);
        }
       
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
